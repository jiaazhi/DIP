using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class TeacherModScript : MonoBehaviour
{

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
        
    }
}
