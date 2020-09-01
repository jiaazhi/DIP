using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameSetup : MonoBehaviour
{
    public static GameSetup GS;
    public Transform[] spawnPoints;
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (GameSetup.GS == null)
        {
            GameSetup.GS = this;
        }
    }
}
