using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
public class NameTag : MonoBehaviour
{
    private PhotonView PV;
    [SerializeField]
    private TextMeshProUGUI nameTag;
    // Start is called before the first frame update
    private void Start()
    {
        PV = GetComponent<PhotonView>();

        setName();
    }

    // Update is called once per frame
    private void setName()
    {
        nameTag.text = PV.Owner.NickName;
    }
}
