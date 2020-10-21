using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassroomController : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public GameObject teacherView;
    public GameObject teacherViewCamera;

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

    private void SetUpPlayer() {
        if (PhotonNetwork.NickName != "teacher")
        {
            //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "vBasicController_allie chibi"), new Vector3(7.0f, 0.5f, -12.0f), Quaternion.identity, 0);
            //Debug.Log("Player Created");
        }
        else
        {
            int spawnPicker = 1;
            PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "Teacher2"),
                GameSetup.GS.spawnPoints[spawnPicker].position,
                GameSetup.GS.spawnPoints[spawnPicker].rotation,
                0
            );
    

            OpenTeacherView();
        }

    }

    private void OpenTeacherView() {
        teacherView.SetActive(true);
        teacherViewCamera.SetActive(true);
    }
}
