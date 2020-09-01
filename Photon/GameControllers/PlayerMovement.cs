using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    private PhotonView PV;
    private CharacterController myCC;
    public float movementSpeed;
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        myCC = GetComponent<CharacterController>();
        Debug.Log("player movement");
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            BasicMovement();
            BasicRotation();
        }
    }

    void BasicMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("W");
            myCC.Move(transform.forward * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("A");
            myCC.Move(-transform.right * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("S");
            myCC.Move(-transform.forward * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Debug.Log("D");
            myCC.Move(transform.right * Time.deltaTime * movementSpeed);
        }
    }

    void BasicRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed;
        transform.Rotate(new Vector3(0, mouseX, 0));
    }
}
