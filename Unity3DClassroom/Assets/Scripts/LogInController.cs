using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogInController: MonoBehaviourPunCallbacks
{
    private static LogInController _instance;

    public static LogInController Instance { get { return _instance; } }


    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        Debug.Log("Player connected to photon master server");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void ConnectToRoom(string username, string roomname) {
        PhotonNetwork.NickName = username;
        PhotonNetwork.JoinOrCreateRoom(
            roomname,
            new RoomOptions(),
            new TypedLobby(roomname, LobbyType.Default)
        );
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined room (" + PhotonNetwork.CurrentRoom.Name + ")");

        if (!PhotonNetwork.IsMasterClient) {
            return;
        }
        PhotonNetwork.LoadLevel("Classroom");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
