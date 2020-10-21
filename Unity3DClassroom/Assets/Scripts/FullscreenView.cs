using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullscreenView : MonoBehaviour
{
    public GameObject PlayerCamera;
    public GameObject WhiteboardCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.NickName == "teacher") {
            gameObject.SetActive(false);
        } else {
            PlayerCamera.SetActive(true);
            WhiteboardCamera.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F)) {
            PlayerCamera.SetActive(!PlayerCamera.activeSelf);
            WhiteboardCamera.SetActive(!WhiteboardCamera.activeSelf);
        }
    }
}
