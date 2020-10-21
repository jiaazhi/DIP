using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent onInteract;
    public UnityEvent onEnterRange;
    public UnityEvent onExitRange;
    public float minDistance;

    private bool hasEntered;

    private void Update()
    {
        if (!hasEntered && InsideRange()) {
            hasEntered = true;
            onEnterRange.Invoke();
        } else if (hasEntered && !InsideRange()) {
            hasEntered = false;
            onExitRange.Invoke();
        }

        if (InsideRange() && Input.GetButton("Interact")) {
            onInteract.Invoke();
        }
    }

    private bool InsideRange() {
        if (PhotonNetwork.NickName == "teacher") {
            return false;
        }
        Vector2 self = new Vector2(transform.position.x, transform.position.z);
        Vector2 playerPos = new Vector2(PhotonPlayer.MyPlayer.transform.position.x, PhotonPlayer.MyPlayer.transform.position.z);
        return Vector2.Distance(self, playerPos) < minDistance;
    }
}
