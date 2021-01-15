using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputSettingsMenu : MonoBehaviour
{
    [SerializeField] JoystickInputReader inputReader;
    [SerializeField] AxisSettingsPanel prefab_axisSettingsPanel;
    [SerializeField] Transform[] joyPanels = new Transform[JoystickInputReader.joy_count];
    [SerializeField] TransmitterViewer transmitterViewer;
    [SerializeField] Text transmitterValuesText;

    AxisSettingsPanel[,] axisSettingsPanel_arr = new AxisSettingsPanel[JoystickInputReader.axes_count, JoystickInputReader.joy_count];

    void Awake()
    {
        for (int i = 0; i < JoystickInputReader.axes_count; i++)
        {
            for (int j = 0; j < JoystickInputReader.joy_count; j++)
            {
                AxisSettingsPanel axp = Instantiate(prefab_axisSettingsPanel, joyPanels[j]);
                axp.OnChange += Axp_onChange;
                axisSettingsPanel_arr[i, j] = axp;
            }
        }
        JoystickInputReader.LoadFromFile(); //на случай если инпут ридер будет Awake позже
        SetUIValues();

        //transform.Find("Buttons/ButtonSave").GetComponent<Button>().onClick.AddListener(ButtonSaveClick);
        //transform.Find("Buttons/ButtonLoad").GetComponent<Button>().onClick.AddListener(ButtonLoadClick);
        transform.Find("Buttons/ButtonClear").GetComponent<Button>().onClick.AddListener(ButtonClearClick);
    }

    
    /*
    private void ButtonSaveClick()
    {
        JoystickInputReader.SaveToFile();
    }

    private void ButtonLoadClick()
    {
        JoystickInputReader.LoadFromFile();
        SetUIValues();
    }*/
    private void ButtonClearClick()
    {
        JoystickInputReader.ClearValues();
        DroneRates.SaveToFile();
        SetUIValues();
    }

    private void Axp_onChange(object sender, System.EventArgs e)
    {
        ReadUIValues();
    }


    void Update()
    {
        UpdateSilders();
        transmitterViewer?.setSticks(JoystickInputReader.throttle, JoystickInputReader.yaw, JoystickInputReader.pitch, JoystickInputReader.roll);
        if (transmitterValuesText != null)
            transmitterValuesText.text =
            $"throttle\t{JoystickInputReader.throttle:F3}\r\n" +
            $"yaw\t{JoystickInputReader.yaw:F3}\r\n" +
            $"pitch\t{JoystickInputReader.pitch:F3}\r\n" +
            $"roll\t{JoystickInputReader.roll:F3}";
    }

    void ReadUIValues()
    {
        for (int i = 0; i < JoystickInputReader.axes_count; i++)
        {
            for (int j = 0; j < JoystickInputReader.joy_count; j++)
            {
                var axisSettingsModel = axisSettingsPanel_arr[i, j].axisSettingsModel;
                JoystickInputReader.inputParameters.inputNum[i, j] = axisSettingsModel.inputNumber;
                JoystickInputReader.inputParameters.inverts[i, j] = axisSettingsModel.inverted;
                JoystickInputReader.inputParameters.min[i, j] = axisSettingsModel.min;
                JoystickInputReader.inputParameters.max[i, j] = axisSettingsModel.max;
                JoystickInputReader.inputParameters.center[i, j] = axisSettingsModel.center;
                JoystickInputReader.inputParameters.deadZoneSize[i, j] = axisSettingsModel.deadZoneSize;
            }
        }
        JoystickInputReader.SaveToFile();
    }

    void SetUIValues()
    {
        for (int i = 0; i < JoystickInputReader.axes_count; i++)
        {
            for (int j = 0; j < JoystickInputReader.joy_count; j++)
            {
                axisSettingsPanel_arr[i, j].axisSettingsModel = new AxisSettingsPanel.AxisSettingsModel()
                {
                    inputNumber = JoystickInputReader.inputParameters.inputNum[i, j],
                    inverted = JoystickInputReader.inputParameters.inverts[i, j],
                    min = JoystickInputReader.inputParameters.min[i, j],
                    max = JoystickInputReader.inputParameters.max[i, j],
                    center = JoystickInputReader.inputParameters.center[i, j],
                    deadZoneSize = JoystickInputReader.inputParameters.deadZoneSize[i, j],
                };
            }
        }
    }

    void UpdateSilders()
    {
        for (int i = 0; i < JoystickInputReader.axes_count; i++)
            for (int j = 0; j < JoystickInputReader.joy_count; j++)
                axisSettingsPanel_arr[i, j].SetSlider(JoystickInputReader.axes_value_arr[i, j]);
    }
}
