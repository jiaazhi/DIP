using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;


public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks {
    //room info
    public static PhotonRoom room;
    private PhotonView PV;
    public bool isGameLoaded;
    public int currentScene;

    //player info
    Player[] photonPlayers;
    public int playersInRoom;
    public int playerInGame;

    //display number of players in the room
    [SerializeField]
    private Text playerCountCreatePanel;
    [SerializeField]
    private Text playerCountJoinPanel;




    private void Awake() {
        if (PhotonRoom.room == null) {
            PhotonRoom.room = this;
        } else {
            if (PhotonRoom.room != this) {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable() {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable() {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    // Start is called before the first frame update
    void Start() {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update() {

    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        Debug.Log("now in room");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;

        if (playerCountCreatePanel != null) {
            playerCountCreatePanel.text = playersInRoom + "  User(s) in room";
        }

        if (playerCountJoinPanel != null) {
            playerCountJoinPanel.text = playersInRoom + "  User(s) in room";
        }

    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("new player entered");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
        if (playerCountCreatePanel != null) {
            playerCountCreatePanel.text = playersInRoom + "  User(s) in room";
        }

        if (playerCountJoinPanel != null) {
            playerCountJoinPanel.text = playersInRoom + "  User(s) in room";
        }
    }

    public void StartGameOnclick() {
        isGameLoaded = true;
        if (!PhotonNetwork.IsMasterClient)
            return;
        PhotonNetwork.LoadLevel(MultiPlayerSettings.multiplayersettings.multiplayerScene);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        currentScene = scene.buildIndex;
        if (currentScene == MultiPlayerSettings.multiplayersettings.multiplayerScene) {
            isGameLoaded = true;
            if (PhotonNetwork.NickName != "teacher")
            {
                CreatePlayer();
            }
        }
    }

    private void CreatePlayer() {
        string[] characters = {
            "vBasicController_VBOT_LOD",
            "av_emilia",
            "av_ariel",
            "av_allen",
            "av_allie",
            "av_michelle",
            "vBasicController_Mr Bad Boy"
        };

        int spawnPicker = 0;
        PhotonNetwork.Instantiate(
            Path.Combine("PhotonPrefabs", characters[PlayerInfo.PI.mySelectedCharacter]),
            GameSetup.GS.spawnPoints[spawnPicker].position,
            GameSetup.GS.spawnPoints[spawnPicker].rotation,
            0
        );
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log(otherPlayer.NickName + " has left the room");
        playersInRoom--;

        if (playerCountCreatePanel != null) {
            playerCountCreatePanel.text = playersInRoom + "  User(s) in room";
        }

        if (playerCountJoinPanel != null) {
            playerCountJoinPanel.text = playersInRoom + "  User(s) in room";
        }
    }
}
