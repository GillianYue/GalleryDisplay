using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PanZoom : MonoBehaviour
{
    Vector3 touchStart;
    public GameObject moveAroundGO; //pos will change as mouse drag
    Vector3 moveGOStartCenter; //, extentsOffset;
    Vector3 moveGOstartPos;

    public float zoomOutMin = 1, zoomOutMax = 3;
    Vector2 extentsX, extentsY = new Vector2(9999, 9999); //extents to which the moveAroundGO can move, details see notes in Map

    public bool checkForPan, checkForZoom;
    public float zMin, zMax;

    bool lerpMoving; //whether is in motion of zooming to target scale, will ignore input when true
    Vector3 panDest;
    float zoomDest;

    public RectTransform container;
    public Canvas mainCanvas; //plane distance will be set to toggle distance to sound sources

    public Vector2 extentsXMin, extentsXMax, //extentsXMin.x is left bound, .y is right bound; same for max
        extentsYMin, extentsYMax;  //.x is for top bound, .y is for bottom


    void Start()
    {

        recalcExtents();
    }

    public void recalcExtents()
    {
        float percent = (moveAroundGO.transform.position.z - zMin) / (zMax - zMin); //1% is at zMax
        extentsX = Vector2.Lerp(extentsXMax, extentsXMin, percent); extentsY = Vector2.Lerp(extentsYMax, extentsYMin, percent);
    }

    void Update()
    {
        if (lerpMoving)
        {
            moveAroundGO.GetComponent<RectTransform>().localPosition = moveAroundGO.GetComponent<RectTransform>().localPosition + 0.001f * panDest;
            float scl = moveAroundGO.GetComponent<RectTransform>().localScale.x, newScl = scl + zoomDest * 0.001f;
            moveAroundGO.GetComponent<RectTransform>().localScale = new Vector3(newScl, newScl, 1);

            if (checkForDestReach()) lerpMoving = false;
        }
        else if (checkForPan) //when lerp, don't check for input control
        {
            if (Input.GetMouseButtonDown(0))
            {
                moveGOstartPos = moveAroundGO.GetComponent<RectTransform>().anchoredPosition;
                touchStart = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 direction = touchStart - Input.mousePosition;
                Vector3 dest = moveGOstartPos - direction;
                //print("dest" + dest + " max x "+ (moveGOStartCenter.x + extents.x + extentsOffset.x));

                if (dest.x >= Mathf.Min(extentsX.x, extentsX.y) &&
                    dest.x <= Mathf.Max(extentsX.x, extentsX.y) &&
                    dest.y >= Mathf.Min(extentsY.x, extentsY.y) &&
                    dest.y <= Mathf.Max(extentsY.x, extentsY.y))
                {
                    //print("proper panning" + dest);
                    moveAroundGO.GetComponent<RectTransform>().anchoredPosition = dest;
                }
                else if (dest.x >= Mathf.Min(extentsX.x, extentsX.y) && dest.x <= Mathf.Max(extentsX.x, extentsX.y))
                {
                    //print("only moving x" + dest);
                    Vector3 curr = moveAroundGO.GetComponent<RectTransform>().anchoredPosition;
                    moveAroundGO.GetComponent<RectTransform>().anchoredPosition = new Vector3(dest.x, curr.y, curr.z);
                }
                else if (dest.y >= Mathf.Min(extentsY.x, extentsY.y) && dest.y <= Mathf.Max(extentsY.x, extentsY.y))
                {
                    //print("only moving y" + dest);
                    Vector3 curr = moveAroundGO.GetComponent<RectTransform>().anchoredPosition;
                    moveAroundGO.GetComponent<RectTransform>().anchoredPosition = new Vector3(curr.x, dest.y, curr.z);
                }

            }

            if (Input.GetMouseButtonUp(0))
            {
                moveGOstartPos = moveAroundGO.GetComponent<RectTransform>().anchoredPosition;
            }
        }

        if (checkForZoom)
        {

            if (Input.touchCount == 2) //TODO need test on ios
            {
                Touch t0 = Input.GetTouch(0), t1 = Input.GetTouch(1);
                Vector3 prevPos0 = t0.position - t0.deltaPosition,
                    prevPos1 = t1.position - t1.deltaPosition;

                float prevMag = (prevPos0 - prevPos1).magnitude,
                    currMag = (t0.position - t1.position).magnitude;

                float diff = currMag - prevMag;

                zoom(diff * 0.01f, Vector2.Lerp(t0.position, t1.position, 0.5f));

            }
            else
            {
                zoom(Input.GetAxis("Mouse ScrollWheel"), Input.mousePosition);
            }

        }
    }

    public void setMoveAroundGO(GameObject go)
    {
        //moveGOStartCenter = go.GetComponent<RectTransform>().localPosition;
        moveAroundGO = go;
        recalcExtents();
    }

    void zoom(float increment, Vector2 center)
    {
        //float scl = moveAroundGO.GetComponent<RectTransform>().localScale.x;
        //float newScl = Mathf.Clamp(scl + increment, zoomOutMin, zoomOutMax);
        //moveAroundGO.GetComponent<RectTransform>().localScale = new Vector3(newScl, newScl, 1);

        //if (Mathf.Abs(scl - newScl) > 0)
        //{
            //also move x and y after scaling
            Vector2 mouseScreenPos = center;
            Ray mouseWorldRay = Camera.main.ScreenPointToRay(mouseScreenPos);

            Vector3 newPos = mouseWorldRay.origin + (mouseWorldRay.direction * increment * 100);

            Vector3 newSetPos = Vector3.MoveTowards(Camera.main.transform.position,
                newPos, increment * 500f * Time.deltaTime);
            Vector3 deltaPos = Camera.main.transform.position - newSetPos;

            Vector3 finalDest = moveAroundGO.transform.position + deltaPos * (increment > 0 ? 1 : -1);

            if (finalDest.z < zMax && finalDest.z > zMin)
        {
            moveAroundGO.transform.position = finalDest;
        }
        //}

        //scale change will result in different extents (boundaries of the map), so recalculate
        recalcExtents();

        
        if (increment != 0)
        {
            //after scaling, we could be out of boundary, need to check and nudge back
            Vector3 curr = moveAroundGO.GetComponent<RectTransform>().anchoredPosition;


            if (curr.x < Mathf.Min(extentsX.x, extentsX.y)) curr.x = Mathf.Min(extentsX.x, extentsX.y);
            else if (curr.x > Mathf.Max(extentsX.x, extentsX.y)) curr.x = Mathf.Max(extentsX.x, extentsX.y);

            if (curr.y < Mathf.Min(extentsY.x, extentsY.y)) curr.y = Mathf.Min(extentsY.x, extentsY.y);
            else if (curr.y > Mathf.Max(extentsY.x, extentsY.y)) curr.y = Mathf.Max(extentsY.x, extentsY.y);

            moveAroundGO.GetComponent<RectTransform>().anchoredPosition = curr;
        }
        


        /*
        if(newScl > zoomOutMin)
        {
            //disable between art
            checkForPan = true;
        }
        else
        {
            checkForPan = false;
        }
        */

        //mainCanvas.planeDistance = Mathf.Lerp(planeDistMax, planeDistMin, (float)(newScl / zoomOutMax));
        // Debug.Log(Mathf.Lerp(planeDistMax, planeDistMin, (float)(newScl / zoomOutMax)));
    }

    public bool isNotScaled()
    {
        return moveAroundGO.GetComponent<RectTransform>().localScale.x == 1;
    }

    public void smoothLerpTo(Vector3 dest)
    {
        smoothLerpTo(dest, moveAroundGO.transform.localScale.x);
    }

    /// <summary>
    /// use local position for the item to zoom on
    /// </summary>
    /// <param name="scl"></param>
    public void smoothLerpTo(float zoomScl)
    {
        smoothLerpTo(moveAroundGO.transform.localPosition, zoomScl);
    }

    /// <summary>
    /// setting both panning and zooming destinations for lerp movement
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="scl"></param>
    public void smoothLerpTo(Vector3 dest, float zoomScl)
    {
        lerpMoving = true;
        zoomDest = zoomScl;
        panDest = dest;
    }

    bool checkForDestReach()
    {
        float currScl = moveAroundGO.transform.localScale.x;
        Vector3 currPos = moveAroundGO.transform.localPosition;

        return (Mathf.Abs(currScl - zoomDest) < 0.2f && findVectorDist(currPos, panDest) < 20);
    }


    public static float findVectorDist(Vector2 v1, Vector2 v2)
    {
        float d = Mathf.Sqrt(Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.y - v2.y), 2));
        return d;
    }
}
