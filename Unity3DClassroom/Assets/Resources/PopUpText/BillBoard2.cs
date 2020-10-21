using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard2 : MonoBehaviour
{
    private Transform mainCameraTransform;
    Vector3 textposition = new Vector3(0.0f, 0.0f, 0.0f);
    // Start is called before the first frame update
    void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.LookAt(textposition + mainCameraTransform.rotation * Vector3.forward
        , mainCameraTransform.rotation * Vector3.up);
    }
}