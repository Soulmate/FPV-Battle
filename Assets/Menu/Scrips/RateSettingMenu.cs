using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RateSettingMenu : MonoBehaviour
{
    Dictionary<string, InputFieldFloatOnly> inputFieldD = new Dictionary<string, InputFieldFloatOnly>();
    InputField vel_roll__if;
    InputField vel_pitch__if;
    InputField vel_yaw__if;


    void Start()
    {
        foreach (string rate_name in DroneRates.rate_names)
        {
            InputFieldFloatOnly inF = transform.Find($"Grid/{rate_name}").GetComponent<InputFieldFloatOnly>();
            if (inF == null)
                Debug.LogError($"No {rate_name}");
            inputFieldD.Add(rate_name, inF);
            inF.OnValueChanged += InF_OnValueChanged;
        }

        vel_roll__if  = transform.Find($"Grid/vel_roll").GetComponent<InputField>();
        vel_pitch__if = transform.Find($"Grid/vel_pitch").GetComponent<InputField>();
        vel_yaw__if   = transform.Find($"Grid/vel_yaw").GetComponent<InputField>();

        writeUIValues();

        //transform.Find("ButtonSave").GetComponent<Button>().onClick.AddListener(ButtonSaveClick);
        transform.Find("ButtonReset").GetComponent<Button>().onClick.AddListener(ButtonResetClick);
    }

    private void InF_OnValueChanged(object sender, EventArgs e)
    {
        ReadUIValues();
    }

    private void ButtonResetClick()
    {
        DroneRates.Reset();
        DroneRates.SaveToFile();
        writeUIValues();
    }
    /*
    private void ButtonSaveClick()
    {        
        DroneRates.SaveToFile();
    }*/

    void writeUIValues()
    {
        foreach (string rate_name in DroneRates.rate_names)        
            inputFieldD[rate_name].Value = DroneRates.rates_dict[rate_name];        
        //вычисляем максимальную скорость и выводим ее
        var max_vel = DroneRates.GetAngularVelocities(new DroneRates.AxisValues(1, 1, 1));
        vel_roll__if.text = max_vel.roll.ToString("F1");
        vel_pitch__if.text = max_vel.pitch.ToString("F1");
        vel_yaw__if.text = max_vel.yaw.ToString("F1");
    }

    void ReadUIValues()
    {
        foreach (string rate_name in DroneRates.rate_names)
            if (inputFieldD[rate_name].Value.HasValue)
                DroneRates.rates_dict[rate_name] = inputFieldD[rate_name].Value.Value;
        DroneRates.SaveToFile();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
