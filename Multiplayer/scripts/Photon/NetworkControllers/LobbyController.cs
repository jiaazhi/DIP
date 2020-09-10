using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyController : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private GameObject lobbyConnectButton;
    [SerializeField]
    private GameObject lobbyPanel;
    [SerializeField]
    private GameObject mainPanel;
    [SerializeField]
    private InputField playerNameInput;

    private string roomName;
    private int roomSize;

    [SerializeField]
    private GameObject CreatePanel;
    [SerializeField]
    private InputField codeDisplay;
    [SerializeField]
    private InputField codeInputField;

    [SerializeField]
    private GameObject joinPanel;
    private string joinCode;
    [SerializeField]
    private GameObject joinButton;

    [SerializeField]
    private Text nameDisplay;
    [SerializeField]
    private Text avatarOptionChosen;

    public static LobbyController lobby;

    private void Awake()
    {
        lobby = this;
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player connected to photon master server");
        PhotonNetwork.AutomaticallySyncScene = true;
        lobbyConnectButton.SetActive(true);

        //Display nickname of player, random generate player XXXX if NA
        if (PlayerPrefs.HasKey("Nickname"))
        {
            if (PlayerPrefs.GetString("NickName") == "")
            {
                PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
                Debug.Log("Random Name Generated");
            }
            else
            {
                PhotonNetwork.NickName = PlayerPrefs.GetString("NickName");
            }
        }
        else
        {
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
            Debug.Log("Random Name Generated");
        }
        playerNameInput.text = PhotonNetwork.NickName;
    }

    public void PlayerNameUpdateInputChanged(string nameInput)
    {
        PhotonNetwork.NickName = nameInput;
        PlayerPrefs.SetString("NickName", nameInput);
    
    }
    public void JoinLobbyOnClick()

    {
        //JOIN button at menu
        Debug.Log("Join Lobby button clicked");
        mainPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        PhotonNetwork.JoinLobby();
        nameDisplay.text = "WELCOME, " + PhotonNetwork.NickName + "!";
        avatarOptionChosen.text = "Avatar chosen: " + (PlayerPrefs.GetInt("MyCharacter") + 1);

    }

    public void OnRoomSizeInputChanged(string sizeIn)
    {
        roomSize = int.Parse(sizeIn);
    }

    public void CreateRoomOnClick()
    {
        //CREATE button in lobby
        CreatePanel.SetActive(true);
        Debug.Log("Creating Room now..");
        RoomOptions roomOps = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)roomSize
        };
        int roomCode = Random.Range(1000, 10000);
        roomName = roomCode.ToString();
        PhotonNetwork.CreateRoom(roomName, roomOps);

        codeDisplay.text = roomName;
        Debug.Log("Room created. Passcode = " + roomName);
    }
    public override void OnCreateRoomFailed(short returncode, string message)
    {
        Debug.Log("Tried to create but failed, trying again");
        RoomOptions roomOps = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)roomSize
        };
        int roomCode = Random.Range(1000, 10000);
        roomName = roomCode.ToString();
        PhotonNetwork.CreateRoom(roomName, roomOps);

        codeDisplay.text = roomName;
        Debug.Log("Room created. Passcode = " + roomName);
    }

    public void CancelRoomOnClick()
    {
        //CANCEL button on create panel
        if(PhotonNetwork.IsMasterClient)
        {
            for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                PhotonNetwork.CloseConnection(PhotonNetwork.PlayerList[i]);
            }
        }
        PhotonNetwork.LeaveRoom();
        CreatePanel.SetActive(false);
        joinButton.SetActive(true);
    }

    public void OpenJoinPanel()
    {
        joinPanel.SetActive(true);
    }
    public void CodeInput(string code)
    {
        joinCode = code;
    }
    public void JoinRoomOnClick()
    {
        //START Button on join Panel
        PhotonNetwork.JoinRoom(joinCode);
    }
    public void LeaveRoomOnClick()
    {
        
        if(PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        joinButton.SetActive(true);
        joinPanel.SetActive(false);
        codeInputField.text = "";
    }

    public void MatchmakingCancelOnClick()
    {
        //Back Button on Create Panel
        mainPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        PhotonNetwork.LeaveLobby();

    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
