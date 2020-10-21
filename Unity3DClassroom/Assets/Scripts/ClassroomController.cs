using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassroomController : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public GameObject teacherComponents;
    public GameObject studentComponents;

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
        SetUpPlayer();
    }

    public static void SpawnPlayer(Vector3 position, Quaternion rotation, string avatarName) {
        PhotonNetwork.Instantiate(
            Path.Combine("PhotonPrefabs", avatarName),
            position,
            rotation,
            0
        );
        Debug.Log("Player Created");
    }

    public static void RespawnPlayer() {
        Vector3 position = PhotonPlayer.MyPlayer.transform.position;
        Quaternion rotation = PhotonPlayer.MyPlayer.transform.rotation;

        PhotonNetwork.Destroy(PhotonPlayer.MyPlayer);
        SpawnPlayer(position, rotation, PlayerPrefs.GetString("avatar"));
    }

    private void SetUpPlayer() {
        if (PhotonNetwork.NickName != "teacher") {
            if (GameObject.Find("RoomController")) {
                return;
            }

            SpawnPlayer(
                GameSetup.GS.spawnPoints[0].position,
                GameSetup.GS.spawnPoints[0].rotation,
                PlayerPrefs.GetString("avatar")
            );

            studentComponents.SetActive(true);
        } else {
            SpawnPlayer(
                GameSetup.GS.spawnPoints[1].position,
                GameSetup.GS.spawnPoints[1].rotation,
                "Teacher2"
            );

            teacherComponents.SetActive(true);
        }
    }
}
