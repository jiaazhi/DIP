using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public string role;

    public GameObject defaultPlayer;
    public GameObject defaultPlayerController;
    public GameObject teacherView;
    public GameObject teacherViewCamera;


    // Start is called before the first frame update
    void Start()
    {
        if (role == "student") {
            SwitchToStudent();
        } else if (role == "teacher") {
            SwitchToTeacher();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SwitchToStudent() {
        // Instantiate student player in PUN ??
        // Show only 3rd person POV
        // Can buy things in shop, etc

        CreateStudent();
    }

    private void SwitchToTeacher() {
        // Instantiate teacher player in PUN ??
        // Show Webview by default (TeachingCanvas)
        // able to toggle the webview on and off with 3rd person POV
        OpenTeacherView(); 

    }

    private void SetCameraPOV(string role) {
        defaultPlayerController.SetActive(false);
        teacherViewCamera.SetActive(false);

        if (role == "student") {
            defaultPlayerController.SetActive(true);
        } else if (role == "teacher") {
            teacherViewCamera.SetActive(true);
        }
    }

    private void CreateStudent() {
        defaultPlayer.SetActive(true);
        SetCameraPOV("student");
    }

    private void OpenTeacherView() {
        teacherView.SetActive(true);
        SetCameraPOV("teacher");
    }

}
