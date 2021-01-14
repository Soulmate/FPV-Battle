using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    Transform leftMenu;
    Transform inputSettings;
    
    void Start()
    {
        leftMenu = transform.Find("Panel/LeftMenu");
        inputSettings = transform.Find("Panel/InputSettings");
        leftMenu.Find("Return").GetComponent<Button>().onClick.AddListener(ButtonReturn);
        leftMenu.Find("Input mapping").GetComponent<Button>().onClick.AddListener(ButtonInput);
        leftMenu.Find("Rates").GetComponent<Button>().onClick.AddListener(ButtonRates);
        leftMenu.Find("Exit").GetComponent<Button>().onClick.AddListener(ButtonExit);

        inputSettings.gameObject.SetActive(false);
    }
    private void ButtonReturn()
    {
        this.gameObject.SetActive(false);
    }   
    private void ButtonInput()
    {
        inputSettings.gameObject.SetActive(true);
    }
    private void ButtonRates()
    {
        
    }
    private void ButtonExit()
    {
        Application.Quit();
    }




}
