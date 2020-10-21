using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Transform mainCameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        mainCameraTransform = Camera.main.transform;
        Debug.LogWarning(Camera.main);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward
        , mainCameraTransform.rotation * Vector3.up);
    }
}
