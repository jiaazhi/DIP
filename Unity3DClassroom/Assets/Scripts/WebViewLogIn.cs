using UnityEngine;
using SimpleJSON;

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
using Vuplex.WebView;
using Vuplex.WebView.Demos;

public class WebViewLogIn : MonoBehaviour
{
    public CanvasWebViewPrefab _canvasWebViewPrefab;
    HardwareKeyboardListener _hardwareKeyboardListener;

    // Start is called before the first frame update
    void Start()
    {
        StandaloneWebView.EnableRemoteDebugging(8080);

        _canvasWebViewPrefab.Initialized += (sender, e) => {
            // Use the LoadProgressChanged event to determine when the page has loaded.
            _canvasWebViewPrefab.WebView.LoadProgressChanged += CanvasWebView_LoadProgressChanged;
            _canvasWebViewPrefab.WebView.MessageEmitted += CanvasWebView_MessageEmitted;
        };

        // Send keys from the hardware keyboard to the webview.
        _hardwareKeyboardListener = HardwareKeyboardListener.Instantiate();
        _hardwareKeyboardListener.InputReceived += (sender, eventArgs) => {
            _canvasWebViewPrefab.WebView.HandleKeyboardInput(eventArgs.Value);
        };

        _hardwareKeyboardListener.Enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CanvasWebView_LoadProgressChanged(object sender, ProgressChangedEventArgs eventArgs) {
        // Send a message after the page has loaded.
        if (eventArgs.Type == ProgressChangeType.Finished) {
            _canvasWebViewPrefab.WebView.PostMessage("Something changed!");
        }
    }

    void CanvasWebView_MessageEmitted(object sender, EventArgs<string> eventArgs) {
        var payload = JSON.Parse(eventArgs.Value);
        if (payload["type"].Value != "PLAYER_INFO") return;


        var player_info = payload["data"];

        string username = player_info["username"].Value;
        string roomname = player_info["roomname"].Value;
        string avatar = player_info["avatar"].Value;

        PlayerPrefs.SetString("username", username);
        PlayerPrefs.SetString("roomname", roomname);
        PlayerPrefs.SetString("avatar", avatar);

        Debug.Log("Username: " + username + ", Roomname: " + roomname);
        Debug.Log("You are logged in!");

        LogInController.Instance.ConnectToRoom(username, roomname);
    }
}
