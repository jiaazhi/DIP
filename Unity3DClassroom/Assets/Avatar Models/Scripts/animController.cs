using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class animController : MonoBehaviour
{
    public Animator anim;
    //public Button Text;
    //public AudioClip sound;
    //public Canvas yourcanvas;
    public bool canEmote = true;
    public SitOn sitOn;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        //Text = Text.GetComponent<Button>();
        //anim.enabled = false;
        //yourcanvas.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (sitOn.sittingOn)
        {
            canEmote = false;
        }
        else
        {
            canEmote = true;
        }

        if (canEmote)
        {
            if (Input.GetKeyDown("1"))
            {
                anim.Play("Cry");
                anim.Play("Sad face");
            }

            if (Input.GetKeyDown("1"))
            {
                anim.Play("Locomotion");
                //anim.Play("Blink");
            }
            if (Input.GetKeyDown("2"))
            {
                anim.Play("standClap");
                anim.Play("happy");
            }

            if (Input.GetKeyDown("2"))
            {
                anim.Play("Locomotion");
                anim.Play("Blink");
            }
            if (Input.GetKeyDown("3"))
            {
                anim.Play("Wave");
                anim.Play("happy");
            }

            if (Input.GetKeyDown("3"))
            {
                anim.Play("Locomotion");
                anim.Play("Blink");
            }
        } else
        {
            if (Input.GetKeyDown("2"))
            {
                anim.Play("sitClap");
                anim.Play("happy");
            }

            //if (Input.GetKeyDown("2"))
            //{
                //anim.Play("Locomotion");
                //anim.Play("Blink");
            //}
        }

    }

  
}
