using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textAppear : MonoBehaviour
{
    public Camera cam;
    private bool status; //when true, is showing
    private Animator myAnim;
    private SpriteRenderer textSprite;

    void Start()
    {
        myAnim = GetComponent<Animator>();
        textSprite = GetComponent<SpriteRenderer>();
        textSprite.color = new Color(0, 0, 0, 0); //hide
    }

    void Update()
    {
        if(status && outDist())
        {
            myAnim.Play("transparent");
            status = false;
        }else if (!status && inDist())
        {
            myAnim.Play("textShow");
            status = true;
        }
    }

    public bool inDist()
    {
        return (Vector3.Distance(transform.position, cam.transform.position) < 10);
    }

    public bool outDist()
    {
        return (Vector3.Distance(transform.position, cam.transform.position) > 14);
    }
}
