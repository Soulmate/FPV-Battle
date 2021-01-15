using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    Transform mainPanel;
    Transform leftMenu;
    Transform inputSettings;
    Transform rateSettings;
    Drone_phys_and_input Mydrone;


    public bool isShowing
    {
        get => _isShowing;
        set
        {
            _isShowing = value;
            mainPanel.gameObject.SetActive(isShowing);
        }
    }
    static bool _isShowing = false;

    void Start()
    {
        mainPanel = transform.Find("Panel");
        leftMenu = transform.Find("Panel/LeftMenu");
        inputSettings = transform.Find("Panel/InputSettings");
        rateSettings = transform.Find("Panel/RateSettings");
        Mydrone = GameObject.FindGameObjectWithTag("Player").GetComponent< Drone_phys_and_input>();

        leftMenu.Find("Return").GetComponent<Button>().onClick.AddListener(ButtonReturn);
        leftMenu.Find("Input mapping").GetComponent<Button>().onClick.AddListener(ButtonInput);
        leftMenu.Find("Rates").GetComponent<Button>().onClick.AddListener(ButtonRates);
        leftMenu.Find("Exit").GetComponent<Button>().onClick.AddListener(ButtonExit);

        inputSettings.gameObject.SetActive(true);
        rateSettings.gameObject.SetActive(false);
        isShowing = false;
    }
    private void ButtonReturn()
    {
        isShowing = false;
    }   
    private void ButtonInput()
    {
        inputSettings.gameObject.SetActive(true);
        rateSettings.gameObject.SetActive(false);
    }
    private void ButtonRates()
    {
        inputSettings.gameObject.SetActive(false);
        rateSettings.gameObject.SetActive(true);
    }
    private void ButtonExit()
    {
        Application.Quit();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isShowing = !isShowing;
            Mydrone.allowMovment = !isShowing; //запретить двигаться дрону
            if (!isShowing) //если закрылись выкидываем все несохраненные данные
            {
                DroneRates.LoadFromFile();
                JoystickInputReader.LoadFromFile();
            }

        }
    }
}
