using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuplex.WebView;

public class Whiteboard : MonoBehaviour, IOnEventCallback
{
    public int resolution;
    WebViewPrefab webViewPrefab;
    // Start is called before the first frame update
    void Start()
    {
        webViewPrefab = GetComponent<WebViewPrefab>();
        webViewPrefab.ClickingEnabled = false;
        webViewPrefab.Initialized += (sender, e) => {
            webViewPrefab.WebView.SetResolution(resolution);
        };
    }

    private void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent) {
        byte eventCode = photonEvent.Code;
        if (eventCode == PhotonEventCodes.ChangeWhiteboardUrlEventCode) {
            string url = (string)photonEvent.CustomData;
            webViewPrefab.WebView.LoadUrl(url);
        }
    }
}
