using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonLobby : MonoBehaviourPunCallbacks
{

    public static PhotonLobby lobby;

    public GameObject battlebutton;
    public GameObject cancelbutton;

    private void Awake()
    {
        lobby = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player connected to photon master server");
        PhotonNetwork.AutomaticallySyncScene = true;
        battlebutton.SetActive(true);
    }

    public void OnBattleButtonClicked()
    {
        Debug.Log("battlebutton clicked");
        battlebutton.SetActive(false);
        cancelbutton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returncode, string message)
    {
        Debug.Log("Tried to join random but failed");
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("creating room");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiPlayerSettings.multiplayersettings.maxPlayer };
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
    }

    public override void OnCreateRoomFailed(short returncode, string message)
    {
        Debug.Log("Tried to create but failed, trying again");
        CreateRoom();
    }

    public void OnCancelButtonClicked()
    {
        Debug.Log("cancelbutton clicked");
        battlebutton.SetActive(true);
        cancelbutton.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }



    // Update is called once per frame
    void Update()
    {

    }


}
