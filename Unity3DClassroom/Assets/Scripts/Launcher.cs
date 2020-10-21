using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using Vuplex.WebView;

public class Launcher : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start() {
        //PlayerPrefs.DeleteAll();

        //Debug.Log("Connecting to Photon Network");

        //ConnectToPhoton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //void ConnectToPhoton() {
    //    PhotonNetwork.GameVersion = "1";
    //    PhotonNetwork.ConnectUsingSettings();
    //}

    public void JoinRoom() {
        //if (PhotonNetwork.IsConnected) {
        //    PhotonNetwork.LocalPlayer.NickName = playerName; //1
        //    Debug.Log("PhotonNetwork.IsConnected! | Trying to Create/Join Room " + roomNameField.text);
        //    RoomOptions roomOptions = new RoomOptions(); //2
        //    TypedLobby typedLobby = new TypedLobby(roomName, LobbyType.Default); //3
        //    PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby); //4
        //}
    }

    void OnJoinedRoom() {
        //Debug.Log("Player joined!");
    }

}
