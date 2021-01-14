using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuAppearScript : MonoBehaviour
{

    public GameObject menu; // Assign in inspector
    private bool isShowing = false;

    private void Start()
    {
        menu.SetActive(isShowing);
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            isShowing = !isShowing;
            menu.SetActive(isShowing);
        }
    }
}