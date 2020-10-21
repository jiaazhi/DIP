using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuplex.WebView;

public class ToggleScreen : MonoBehaviour
{
    public WebViewPrefab webViewPrefab;

    // Start is called before the first frame update
    void Start()
    {
        webViewPrefab.Visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleWebView()
    {
        webViewPrefab.Visible = !webViewPrefab.Visible;
    }
}
