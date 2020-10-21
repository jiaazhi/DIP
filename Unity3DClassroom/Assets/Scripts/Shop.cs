using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuplex.WebView;

public class Shop : MonoBehaviour
{
    public int resolution;
    public GameObject shopCamera;
    public WebViewPrefab webViewPrefab;

    // Start is called before the first frame update
    void Start()
    {
        webViewPrefab.Initialized += (sender, e) => {
            webViewPrefab.WebView.SetResolution(resolution);
            webViewPrefab.WebView.MessageEmitted += OnAvatarChanged;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            shopCamera.SetActive(false);
        }
    }

    void OnAvatarChanged(object sender, EventArgs<string> eventArgs) {
        var avatarName = JSON.Parse(eventArgs.Value);
        print(avatarName);
        PlayerPrefs.SetString("avatar", avatarName);
        ClassroomController.RespawnPlayer();
    }
}
