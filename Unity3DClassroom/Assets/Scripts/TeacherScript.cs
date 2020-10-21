using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class TeacherScript : MonoBehaviour
{

    public GameObject FloatingTextPrefab1;
    public Animator anim;
    private PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {

            if (Input.GetKeyDown("f1"))
            {
                GetComponent<PhotonView>().RPC("playWave", RpcTarget.AllBuffered, null);
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
    }

    [PunRPC]
    void playWave()
    {
        anim.Play("Waving");
        ShowFloatingText1();

    }

    void ShowFloatingText1()
    {
        Instantiate(FloatingTextPrefab1, transform.position, Quaternion.identity, transform);
    }

}
