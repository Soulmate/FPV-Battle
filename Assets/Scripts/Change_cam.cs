using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_cam : MonoBehaviour
{
    GameObject flat_cam;
    public bool use_flat_cam = false;
     
    void Start()
    {
        flat_cam = GameObject.Find("Flat camera");
        flat_cam.SetActive(use_flat_cam);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            use_flat_cam = !use_flat_cam;
            flat_cam.SetActive(use_flat_cam);
        }
    }
}
