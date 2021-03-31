using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollControl : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform[] rectTransforms;
    public float scrollToTime;
    public bool horizontal, vertical;

    float timeElapsed;
    float lerpDuration;
    Vector3 startLerpPos, endLerpPos;

    public int currViewArt;
    public PanZoom panZoom;

    private void Awake()
    {
        //SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
    }

    void Start()
    {
        InitializeContentSize();
        LerpToIndex(0);
    }

    void InitializeContentSize()
    {
        panZoom.setMoveAroundGO(panZoom.moveAroundGO); //initialize center record

        panZoom.moveAroundGO.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(rectTransforms.Length * (rectTransforms[0].rect.width-5), rectTransforms[0].rect.height);


        for (int r=0; r<rectTransforms.Length; r++)
        {
            rectTransforms[r].offsetMin = new Vector2(r * (rectTransforms[0].rect.width-5), 0);
            rectTransforms[r].sizeDelta = new Vector2((rectTransforms[0].rect.width), rectTransforms[0].rect.height);
            //rectTransforms[r].transform.localPosition = new Vector2(r * (rectTransforms[0].rect.width), 0);
        }
    }

    void Update()
    {
        if(timeElapsed < lerpDuration)
        {
            float currX = horizontal? Mathf.SmoothStep(startLerpPos.x, endLerpPos.x, timeElapsed / lerpDuration) : startLerpPos.x;
            float currY = vertical? Mathf.SmoothStep(startLerpPos.y, endLerpPos.y, timeElapsed / lerpDuration) : startLerpPos.y;
            scrollRect.content.anchoredPosition = new Vector2(currX, currY);
            
            timeElapsed += Time.deltaTime;
        }
    }

    //assumes the arts have the same dimensions
    public void LerpToIndex(int i)
    {
        currViewArt = i;
        //LerpTo(scrollRect, rectTransforms[i]);
        //panZoom.setMoveAroundGO(rectTransforms[i].gameObject); 
        LerpTo(scrollRect, i);
    }

    //use when moveAroundGO is the RectTransform children
    public void LerpTo(ScrollRect scroller, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();

        var contentPos = (Vector2)scroller.transform.InverseTransformPoint(scroller.content.position);
        var childPos = (Vector2)scroller.transform.InverseTransformPoint(child.position);
        var endPos = contentPos - childPos;

        // If no horizontal scroll, then don't change contentPos.x
        if (!horizontal) endPos.x = contentPos.x;
        // If no vertical scroll, then don't change contentPos.y
        if (!vertical) endPos.y = contentPos.y;

        //start lerp movement
        startLerpPos = scrollRect.content.anchoredPosition;
        endLerpPos = endPos;
        lerpDuration = scrollToTime;
        timeElapsed = 0;
    }

    //use when moveAroundGO is the greater scroll
    public void LerpTo(ScrollRect scroller, int index)
    {
        //start lerp movement
        startLerpPos = scroller.content.anchoredPosition;
        endLerpPos = new Vector2(0 - index * rectTransforms[0].rect.width, startLerpPos.y);
        lerpDuration = scrollToTime;
        timeElapsed = 0;
    }

    private void SwipeDetector_OnSwipe(SwipeData data)
    {
        if (panZoom.isNotScaled())
        {
            switch (data.Direction)
            {
                case SwipeDirection.Right:
                    if (currViewArt > 0) LerpToIndex(currViewArt - 1);
                    break;
                case SwipeDirection.Left:
                    if (currViewArt < rectTransforms.Length - 1) LerpToIndex(currViewArt + 1);
                    break;
            }
        }
    }
}
