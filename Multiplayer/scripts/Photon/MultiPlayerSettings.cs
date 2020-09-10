using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MultiPlayerSettings : MonoBehaviour
{
    public static MultiPlayerSettings multiplayersettings;
    public int maxPlayer;
    public int menuScene;
    public int multiplayerScene;

    private void Awake()
    {
        if (MultiPlayerSettings.multiplayersettings == null)
        {
            MultiPlayerSettings.multiplayersettings = this;
        }
        else
        {
            if (MultiPlayerSettings.multiplayersettings != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
