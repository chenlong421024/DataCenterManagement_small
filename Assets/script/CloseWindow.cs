using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#define UNITY_ERITOR
public class CloseWindow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Close()
    {
       
//#if UNITY_ERITOR
    // UnityEditor.EditorApplication.isPlaying=false ;
//#else
        Application.Quit();
//#endif
    }
}
