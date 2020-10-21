using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SitOn : MonoBehaviour
{
    Animator anim;
    bool isWalkingTowards = false;
    public bool sittingOn = false;
    public float WaitTime = 1000.0f;

    protected virtual void Start()
    {
    }

    void OnMouseDown()
    {
        var character = PhotonPlayer.MyPlayer;
        if (!sittingOn)
        {
            anim = character.GetComponent<Animator>();
            //anim.SetTrigger("isWalking");
            isWalkingTowards = true;
        }
        if (sittingOn)
        {
            //anim.SetTrigger("isStanding");
            character.GetComponent<PhotonView>().RPC("playSittoStand", RpcTarget.AllBuffered, null);
            sittingOn = false;
            //character = null;
            //anim = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        var character = PhotonPlayer.MyPlayer;
        if (isWalkingTowards)
        {
            Vector3 targetDir;
            targetDir = new Vector3(transform.position.x - character.transform.position.x, 0f,
                transform.position.z - character.transform.position.z);
            Quaternion rot = Quaternion.LookRotation(targetDir);
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, rot, 0.05f);
            character.transform.Translate(Vector3.forward * 0.01f);

            if (Vector3.Distance(character.transform.position, this.transform.position) < 0.7)
            {

                //anim.SetTrigger("isSitting");
                character.GetComponent<PhotonView>().RPC("playStandtoSit", RpcTarget.AllBuffered, null);
                StartCoroutine("Reset", 2.15f);
                //yield return new WaitForSeconds(1.0f);
                //character.GetComponent<PhotonView>().RPC("playSitting", RpcTarget.AllBuffered, null);
                //turn character around to align forward vector
                //with object's vector
                character.transform.rotation = this.transform.rotation;
                isWalkingTowards = false;
                sittingOn = true;
            }

        }

    }

    IEnumerator Reset(float Count)
    {
        yield return new WaitForSeconds(Count); //Count is the amount of time in seconds that you want to wait.
                                                //And here goes your method of resetting the game...
        PhotonPlayer.MyPlayer.GetComponent<PhotonView>().RPC("playSitting", RpcTarget.AllBuffered, null);
    }


}
