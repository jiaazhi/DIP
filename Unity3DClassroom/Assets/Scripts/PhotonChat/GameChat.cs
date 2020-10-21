using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.EventSystems;
using System.Reflection;

public class GameChat : MonoBehaviourPun, IDeselectHandler
{
    bool isChatting = false;
    string chatInput = "";

    [System.Serializable]
    public class ChatMessage
    {
        public string sender = "";
        public string message = "";
        public float timer = 0;
    }

    List<ChatMessage> chatMessages = new List<ChatMessage>();

    void Start()
    {
        if(gameObject.GetComponent<PhotonView>() == null)
        {
            PhotonView photonView = gameObject.AddComponent<PhotonView>();
            photonView.ViewID = 1;
        }
        else
        {
            photonView.ViewID = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T) && !isChatting && EventSystem.current.currentSelectedGameObject == null)
        {
            isChatting = true;
            chatInput = "";
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        //Hide messages after timer is expired
        for (int i = 0; i < chatMessages.Count; i++)
        {
            if (chatMessages[i].timer > 0)
            {
                chatMessages[i].timer -= Time.deltaTime;
            }
        }
    }

    void OnGUI()
    {
        GUIStyle myTextStyle = new GUIStyle();
        myTextStyle.normal.textColor = Color.white;
        myTextStyle.fontSize = 18;

        if (!isChatting)
        {
            GUI.Label(new Rect(5, Screen.height - 25, 200, 25), "Press 'T' to chat", myTextStyle);
        }
        else
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                isChatting = false;
                if(chatInput.Replace(" ", "") != "")
                {
                    //Send message
                    photonView.RPC("SendChat", RpcTarget.All, PhotonNetwork.LocalPlayer, chatInput);
                }
                chatInput = "";

                EventSystem.current.SetSelectedGameObject(null);
            }

            GUI.SetNextControlName("ChatField");
            GUI.Label(new Rect(5, Screen.height - 25, 200, 25), "Say:", myTextStyle);
            GUIStyle inputStyle = GUI.skin.GetStyle("box");
            inputStyle.alignment = TextAnchor.MiddleLeft;
            inputStyle.fontSize = 18;
            chatInput = GUI.TextField(new Rect(50, Screen.height - 27, 400, 22), chatInput, 60, inputStyle);

            GUI.FocusControl("ChatField");
        }


        //Show messages
        for (int i = 0; i < chatMessages.Count; i++)
        {
            if(chatMessages[i].timer > 0 || isChatting)
            {
                GUI.Label(new Rect(5, Screen.height - 50 - 25 * i, 500, 25), chatMessages[i].sender + ": " + chatMessages[i].message, myTextStyle);
            }
        } 
    }
    [PunRPC]
    void SendChat(Player sender, string message)
    {
        ChatMessage m = new ChatMessage();
        m.sender = sender.NickName;
        m.message = message;
        m.timer = 15.0f;

        chatMessages.Insert(0, m);
        if(chatMessages.Count > 8)
        {
            chatMessages.RemoveAt(chatMessages.Count - 1);
        }
    }

    public void OnDeselect(BaseEventData eventData) {
        isChatting = false;
        chatInput = "";
    }
}

