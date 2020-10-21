using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class animController : MonoBehaviour
{
    public GameObject FloatingTextPrefab1, FloatingTextPrefab2, FloatingTextPrefab3;
    Vector3 textposition = new Vector3(0.8f, 1.2f, 0.0f);
    public Animator anim;
    //public Button Text;
    //public AudioClip sound;
    //public Canvas yourcanvas;
    public bool canEmote = true;
    public GameObject[] chairs;
    private SitOn sitOn, sitOnChair;
    private PhotonView PV;
    private int chairNum;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
        chairs = GameObject.FindGameObjectsWithTag("Chair");
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            for (int i = 0; i < chairs.Length; i++)
            {
                sitOn = chairs[i].GetComponent<SitOn>();
                if (sitOn.sittingOn == true)
                {
                    chairNum = i;
                    break;
                }
                else
                {
                    continue;
                }
            }
            sitOnChair = chairs[chairNum].GetComponent<SitOn>();
            if (sitOnChair.sittingOn == true)
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
                    GetComponent<PhotonView>().RPC("playCry", RpcTarget.AllBuffered, null);
                }
                if (Input.GetKeyDown("2"))
                {
                    GetComponent<PhotonView>().RPC("playClap", RpcTarget.AllBuffered, null);
                }
                if (Input.GetKeyDown("3"))
                {
                    GetComponent<PhotonView>().RPC("playWave", RpcTarget.AllBuffered, null);
                }
            }
            else
            {
                if (Input.GetKeyDown("2"))
                {
                    GetComponent<PhotonView>().RPC("playSitClap", RpcTarget.AllBuffered, null);
                    
                }

                if (Input.GetKeyDown("3"))
                {
                    GetComponent<PhotonView>().RPC("playSitRaise", RpcTarget.AllBuffered, null);
                }

            }
        }

    }

    [PunRPC]
    void playSitClap()
    {
        anim.Play("sitClap");
        anim.Play("happy");
        ShowFloatingText1();
    }

    [PunRPC]
    void playSitRaise()
    {
        anim.Play("Raise");
        anim.Play("happy");
        ShowFloatingText3();
    }

    [PunRPC]
    void playCry()
    {
        anim.Play("Cry");
        anim.Play("Sad face");
    }

    [PunRPC]
    void playClap()
    {
        anim.Play("standClap");
        anim.Play("happy");
    }

    [PunRPC]
    void playWave()
    {
        anim.Play("Wave");
        anim.Play("happy");
        ShowFloatingText2();
    }

    [PunRPC]
    void playSittoStand()
    {
        anim.Play("Sit to Stand");
    }

    [PunRPC]
    void playSitting()
    {
        anim.Play("Sit");
    }

    [PunRPC]
    void playStandtoSit()
    {
        anim.Play("Stand to Sit");
    }

    void ShowFloatingText1()
    {
        var go = Instantiate(FloatingTextPrefab1, transform.position, Quaternion.identity, transform);
    }

    void ShowFloatingText2()
    {
        var go = Instantiate(FloatingTextPrefab2, transform.position, Quaternion.identity, transform);
    }
    void ShowFloatingText3()
    {
        var go = Instantiate(FloatingTextPrefab3, transform.position + textposition, Quaternion.identity);
    }

}
