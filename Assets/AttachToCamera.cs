using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AttachToCamera : MonoBehaviour
{    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
#if DEBUG 

        transform.position = SceneView.lastActiveSceneView.camera.transform.position;
            transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
#endif
    }
}
