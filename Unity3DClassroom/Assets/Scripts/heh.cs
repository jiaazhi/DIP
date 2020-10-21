using System.Collections;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Vuplex.WebView;

public class heh : MonoBehaviour
{
    public CanvasWebViewPrefab canvasWebViewPrefab;

    private TMP_InputField input;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<TMP_InputField>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeURL() {
        canvasWebViewPrefab.WebView.LoadUrl(input.text);
    }
}
