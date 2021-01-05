using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanZoom : MonoBehaviour
{
    Vector3 touchStart;
    public GameObject moveAroundGO; //pos will change as mouse drag
    Vector3 moveGOStartCenter;
    Vector3 moveGOstartPos;

    public float zoomOutMin = 1, zoomOutMax = 2;
    Vector2 extents = new Vector2(9999, 9999); //extents to which the moveAroundGO can move, details see notes in Map

    public bool checkForPan, checkForZoom;

    bool lerpMoving; //whether is in motion of zooming to target scale, will ignore input when true
    Vector3 panDest;
    float zoomDest;

    public RectTransform container;

    void Start()
    {




    }

    public Vector2 recalcExtents()
    {
        Vector3 scl = moveAroundGO.transform.localScale;
        float xExt = (scl.x) * moveAroundGO.GetComponent<RectTransform>().rect.width/2 - container.GetComponent<RectTransform>().rect.width/2,
        yExt =  (scl.y) * moveAroundGO.GetComponent<RectTransform>().rect.height/2 - container.GetComponent<RectTransform>().rect.height/2;
        extents = new Vector2(xExt, yExt);
        return extents;
    }

    void Update()
    {
        if (lerpMoving)
        {
            moveAroundGO.transform.localPosition = moveAroundGO.transform.localPosition + 0.001f * panDest;
            float scl = moveAroundGO.transform.localScale.x, newScl = scl + zoomDest * 0.001f;
            moveAroundGO.transform.localScale = new Vector3(newScl, newScl, 1);

            if (checkForDestReach()) lerpMoving = false;
        }
        else if (checkForPan) //when lerp, don't check for input control
        {
            if (Input.GetMouseButtonDown(0))
            {
                moveGOstartPos = moveAroundGO.transform.localPosition;
                touchStart = Input.mousePosition;
            }

            if (Input.GetMouseButton(0)) //TODO take into account center
            {
                Vector3 direction = touchStart - Input.mousePosition;
                Vector3 dest = moveGOstartPos - direction;
                if (dest.x >= moveGOStartCenter.x - extents.x &&
                    dest.x <= moveGOStartCenter.x + extents.x &&
                    dest.y >= moveGOStartCenter.y - extents.y &&
                    dest.y <= moveGOStartCenter.y + extents.y)
                {

                    moveAroundGO.transform.localPosition = dest;
                }
                else if (dest.x >= moveGOStartCenter.x - extents.x &&
                    dest.x <= moveGOStartCenter.x + extents.x)
                {
                    Vector3 curr = moveAroundGO.transform.localPosition;
                    moveAroundGO.transform.localPosition = new Vector3(dest.x, curr.y, curr.z);
                }
                else if (dest.y >= moveGOStartCenter.y - extents.y &&
                    dest.y <= moveGOStartCenter.y + extents.y)
                {
                    Vector3 curr = moveAroundGO.transform.localPosition;
                    moveAroundGO.transform.localPosition = new Vector3(curr.x, dest.y, curr.z);
                }

            }

            if (Input.GetMouseButtonUp(0))
            {
                moveGOstartPos = moveAroundGO.transform.localPosition;
            }
        }

        if (checkForZoom) { 

            if (Input.touchCount == 2) //TODO need test on ios
            {
                Touch t0 = Input.GetTouch(0), t1 = Input.GetTouch(1);
                Vector3 prevPos0 = t0.position - t0.deltaPosition,
                    prevPos1 = t1.position - t1.deltaPosition;

                float prevMag = (prevPos0 - prevPos1).magnitude,
                    currMag = (t0.position - t1.position).magnitude;

                float diff = currMag - prevMag;

                zoom(diff * 0.01f);

            }
            else
            {
                zoom(Input.GetAxis("Mouse ScrollWheel"));
            }

        }
    }

    public void setMoveAroundGO(GameObject go)
    {
        moveGOStartCenter = go.transform.localPosition;
        moveAroundGO = go;
        recalcExtents();
    }

    void zoom(float increment)
    {
        float scl = moveAroundGO.transform.localScale.x;
        float newScl = Mathf.Clamp(scl+increment, zoomOutMin, zoomOutMax);
        moveAroundGO.transform.localScale = new Vector3(newScl, newScl, 1);

        //scale change will result in different extents (boundaries of the map), so recalculate
        extents = recalcExtents();

        if (increment != 0)
        {
            //after scaling, we could be out of boundary, need to check and nudge back
            Vector3 curr = moveAroundGO.transform.localPosition;

            if (curr.x < moveGOStartCenter.x - extents.x) curr.x = moveGOStartCenter.x - extents.x;
            else if (curr.x > moveGOStartCenter.x + extents.x) curr.x = moveGOStartCenter.x + extents.x;

            if (curr.y < moveGOStartCenter.y - extents.y) curr.y = moveGOStartCenter.y - extents.y;
            else if (curr.y > moveGOStartCenter.y + extents.y) curr.y = moveGOStartCenter.y + extents.y;

            moveAroundGO.transform.localPosition = curr;

            print("before: " + curr + " after: " + moveAroundGO.transform.localPosition+" extents: "+extents);
        }


        if(newScl > zoomOutMin)
        {
            //disable between art
            checkForPan = true;
        }
        else
        {
            checkForPan = false;
        }
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


