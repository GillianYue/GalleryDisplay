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

    void Start()
    {
        LerpToIndex(0);
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

    public void LerpToIndex(int i)
    {
        currViewArt = i;
        LerpTo(scrollRect, rectTransforms[i]);
        panZoom.setMoveAroundGO(rectTransforms[i].GetChild(0).gameObject); //pan the actual image object
    }

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

    
}
