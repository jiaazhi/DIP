using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Vuplex.WebView;
using Vuplex.WebView.Demos;

public class TeacherView : MonoBehaviour, ISelectHandler, IDeselectHandler, IInRoomCallbacks
{
    public CanvasWebViewPrefab canvasWebViewPrefab;
    HardwareKeyboardListener _hardwareKeyboardListener;

    public TMP_InputField urlInput;
    private string currentUrl;

    private void Start() {
        PhotonNetwork.AddCallbackTarget(this);

        // Send keys from the hardware keyboard to the webview.
        _hardwareKeyboardListener = HardwareKeyboardListener.Instantiate();
        _hardwareKeyboardListener.InputReceived += (sender, eventArgs) => {
            canvasWebViewPrefab.WebView.HandleKeyboardInput(eventArgs.Value);
        };

        canvasWebViewPrefab.Initialized += (x, y) => {
            canvasWebViewPrefab.WebView.UrlChanged += (sender, eventArgs) => {
                ChangeURL(eventArgs.Url);
            };

            canvasWebViewPrefab._webViewPrefab.Clicked += (sender, eventArgs) => {
                EventSystem.current.SetSelectedGameObject(gameObject);
            };
        };
    }

    private void OnDestroy() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void ChangeURL(string url) {
        Debug.LogWarning(url);
        currentUrl = url;
        PhotonNetwork.RaiseEvent(
            PhotonEventCodes.ChangeWhiteboardUrlEventCode,
            (object)currentUrl,
            new RaiseEventOptions {
                Receivers = ReceiverGroup.All
            },
            SendOptions.SendReliable);
    }

    public void OnEndEditURL(string url) {
        canvasWebViewPrefab.WebView.LoadUrl(url);
    }

    public void OnSelect(BaseEventData eventData) {
        _hardwareKeyboardListener.Enabled = true;
    }

    public void OnDeselect(BaseEventData eventData) {
        _hardwareKeyboardListener.Enabled = false;
    }

    IEnumerator DelayChangeUrl() {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);
        ChangeURL(currentUrl);
    }

    #region IInRoomCallbacks
    void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer) {
        StartCoroutine(DelayChangeUrl());
    }

    void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer) {
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) {
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
    }

    void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient) {
    }
    #endregion
}
