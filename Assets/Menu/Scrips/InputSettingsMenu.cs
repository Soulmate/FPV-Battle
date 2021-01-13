using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSettingsMenu : MonoBehaviour
{
    [SerializeField] InputReader inputReader;
    [SerializeField] AxisSettingsPanel prefab_axisSettingsPanel;
    [SerializeField] Transform[] joyPanels = new Transform[InputReader.joy_count];

    AxisSettingsPanel[,] axisSettingsPanel_arr = new AxisSettingsPanel[InputReader.axes_count, InputReader.joy_count];

    void Awake()
    {
        for (int i = 0; i < InputReader.axes_count; i++)
        {
            for (int j = 0; j < InputReader.joy_count; j++)
            {
                AxisSettingsPanel axp = Instantiate(prefab_axisSettingsPanel, joyPanels[j]);

                axp.axisSettingsModel = (new AxisSettingsPanel.AxisSettingsModel()
                {
                    axisValue = 1,
                    inverted = true,
                    inputNumber = 3
                });

                axp.OnChange += Axp_onChange;

                axisSettingsPanel_arr[i, j] = axp;
            }
        }
    }

    private void Axp_onChange(object sender, System.EventArgs e)
    {
        ReadUIValues();
    }


    void Update()
    {
        UpdateSilders();
    }


    void ReadUIValues()
    {
        for (int i = 0; i < InputReader.axes_count; i++)
        {
            for (int j = 0; j < InputReader.joy_count; j++)
            {
                var axisSettingsModel = axisSettingsPanel_arr[i, j].axisSettingsModel;
                InputReader.axesInputs[i, j] = axisSettingsModel.inputNumber; 
                InputReader.axesInverts[i, j] = axisSettingsModel.inverted;
            }
        }
    }
    void UpdateSilders()
    {
        for (int i = 0; i < InputReader.axes_count; i++)
            for (int j = 0; j < InputReader.joy_count; j++)
                axisSettingsPanel_arr[i, j].SetSlider(InputReader.joy_axes[i, j]);
    }
}
