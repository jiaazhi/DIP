using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomController : MonoBehaviourPunCallbacks, IInRoomCallbacks
{  

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // KIV: Add OnPlayerEnteredRoom

    public override void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "VClassroom") {
            CreatePlayer();
        }
    }

    [PunRPC]
    private void CreatePlayer() {
        PhotonNetwork.Instantiate("Characters/Default", new Vector3(7.0f, 0.5f, -12.0f), Quaternion.identity, 0);
        Debug.Log("Player Created");
    }
}
