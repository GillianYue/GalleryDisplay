using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textAppear : MonoBehaviour
{
    public Camera cam;
    private bool status; //when true, is showing
    private Animator myAnim;
    private SpriteRenderer textSprite;
    public float inDistt;

    void Start()
    {
        myAnim = GetComponent<Animator>();
        textSprite = GetComponent<SpriteRenderer>();
        textSprite.color = new Color(0, 0, 0, 0); //hide

if(inDistt == 0) inDistt = 10;
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
        return (Vector3.Distance(transform.position, cam.transform.position) < inDistt);
    }

    public bool outDist()
    {
        return (Vector3.Distance(transform.position, cam.transform.position) > inDistt+4);
    }
}
