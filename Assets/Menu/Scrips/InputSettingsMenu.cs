using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputSettingsMenu : MonoBehaviour
{
    [SerializeField] InputReader inputReader;
    [SerializeField] AxisSettingsPanel prefab_axisSettingsPanel;
    [SerializeField] Transform[] joyPanels = new Transform[InputReader.joy_count];
    [SerializeField] TransmitterViewer transmitterViewer;
    [SerializeField] Text transmitterValuesText;

    AxisSettingsPanel[,] axisSettingsPanel_arr = new AxisSettingsPanel[InputReader.axes_count, InputReader.joy_count];

    void Awake()
    {
        for (int i = 0; i < InputReader.axes_count; i++)
        {
            for (int j = 0; j < InputReader.joy_count; j++)
            {
                AxisSettingsPanel axp = Instantiate(prefab_axisSettingsPanel, joyPanels[j]);
                axp.OnChange += Axp_onChange;
                axisSettingsPanel_arr[i, j] = axp;
            }
        }
        InputReader.LoadFromFile(); //на случай если инпут ридер будет Awake позже
        SetUIValues();

        transform.Find("Buttons/ButtonSave").GetComponent<Button>().onClick.AddListener(ButtonSaveClick);
        transform.Find("Buttons/ButtonLoad").GetComponent<Button>().onClick.AddListener(ButtonLoadClick);
        transform.Find("Buttons/ButtonClear").GetComponent<Button>().onClick.AddListener(ButtonClearClick);
    }

    

    private void ButtonSaveClick()
    {
        InputReader.SaveToFile();
    }

    private void ButtonLoadClick()
    {
        InputReader.LoadFromFile();
        SetUIValues();
    }
    private void ButtonClearClick()
    {
        InputReader.ClearValues();
        SetUIValues();
    }

    private void Axp_onChange(object sender, System.EventArgs e)
    {
        ReadUIValues();
    }


    void Update()
    {
        UpdateSilders();
        transmitterViewer?.setSticks(InputReader.throttle, InputReader.yaw, InputReader.pitch, InputReader.roll);
        if (transmitterValuesText != null)
            transmitterValuesText.text =
            $"throttle\t{InputReader.throttle:F3}\r\n" +
            $"yaw\t{InputReader.yaw:F3}\r\n" +
            $"pitch\t{InputReader.pitch:F3}\r\n" +
            $"roll\t{InputReader.roll:F3}";
    }

    void ReadUIValues()
    {
        for (int i = 0; i < InputReader.axes_count; i++)
        {
            for (int j = 0; j < InputReader.joy_count; j++)
            {
                var axisSettingsModel = axisSettingsPanel_arr[i, j].axisSettingsModel;
                InputReader.inputParameters.inputNum[i, j] = axisSettingsModel.inputNumber;
                InputReader.inputParameters.inverts[i, j] = axisSettingsModel.inverted;
                InputReader.inputParameters.min[i, j] = axisSettingsModel.min;
                InputReader.inputParameters.max[i, j] = axisSettingsModel.max;
                InputReader.inputParameters.center[i, j] = axisSettingsModel.center;
                InputReader.inputParameters.deadZoneSize[i, j] = axisSettingsModel.deadZoneSize;
            }
        }
    }

    void SetUIValues()
    {
        for (int i = 0; i < InputReader.axes_count; i++)
        {
            for (int j = 0; j < InputReader.joy_count; j++)
            {
                axisSettingsPanel_arr[i, j].axisSettingsModel = new AxisSettingsPanel.AxisSettingsModel()
                {
                    inputNumber = InputReader.inputParameters.inputNum[i, j],
                    inverted = InputReader.inputParameters.inverts[i, j],
                    min = InputReader.inputParameters.min[i, j],
                    max = InputReader.inputParameters.max[i, j],
                    center = InputReader.inputParameters.center[i, j],
                    deadZoneSize = InputReader.inputParameters.deadZoneSize[i, j],
                };
            }
        }
    }

    void UpdateSilders()
    {
        for (int i = 0; i < InputReader.axes_count; i++)
            for (int j = 0; j < InputReader.joy_count; j++)
                axisSettingsPanel_arr[i, j].SetSlider(InputReader.axes_value_arr[i, j]);
    }
}
