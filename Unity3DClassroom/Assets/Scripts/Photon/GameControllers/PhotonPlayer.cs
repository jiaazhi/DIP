using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class PhotonPlayer : MonoBehaviour, IPunInstantiateMagicCallback {
    public static GameObject MyPlayer;
    private PhotonView PV;

    void Awake() {
        PV = GetComponent<PhotonView>();
    }

    void Start() {
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        transform.SetParent(GameObject.Find("Players").transform);
        name = PV.Owner.NickName;
        if (PV.IsMine) {
            MyPlayer = gameObject;
        }
    }

    // Update is called once per frame
    void Update() {

    }
}