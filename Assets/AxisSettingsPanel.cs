using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AxisSettingsPanel : MonoBehaviour
{
    AxisSettingsView axisSettingsView;
    public AxisSettingsModel axisSettingsModel { 
        get
        {
            bool inverted = axisSettingsView.toggle.isOn;
            int? inputNumber;
            if (axisSettingsView.dropdown.value == 0)
                inputNumber = null;
            else
                inputNumber =  axisSettingsView.dropdown.value - 1;
            return (new AxisSettingsModel() { inputNumber = inputNumber, inverted = inverted });
        }
        set
        {
            axisSettingsView.dropdown.value =
                        value.inputNumber.HasValue ?
                        value.inputNumber.Value + 1 : 0; //+1 так как первый None
            axisSettingsView.toggle.isOn = value.inverted;
        }
        
    } //todo спрятать

    public event EventHandler OnChange;
    void Awake()
    {
        axisSettingsView = new AxisSettingsView(
            GetComponentInChildren<Slider>(),
            GetComponentInChildren<Toggle>(),
            GetComponentInChildren<Dropdown>());
        axisSettingsView.toggle.onValueChanged.AddListener((change) => OnChange?.Invoke(this, new EventArgs()));
        axisSettingsView.dropdown.onValueChanged.AddListener((change) => OnChange?.Invoke(this, new EventArgs()));
    }

    public void SetSlider(float value)
    {
        axisSettingsView.slider.value = value;
    }

    public class AxisSettingsModel
    {
        public float axisValue;
        public bool inverted;
        public int? inputNumber;
    }

    public class AxisSettingsView
    {
        public Slider slider;
        public Toggle toggle;
        public Dropdown dropdown;
        public AxisSettingsView(Slider slider, Toggle toggle, Dropdown dropdown)
        {
            this.slider = slider;
            this.toggle = toggle;
            this.dropdown = dropdown;
            slider.minValue = -1;
            slider.maxValue = 1;
            dropdown.ClearOptions();
            dropdown.options.Add(new Dropdown.OptionData() { text = "none" });
            foreach (string inputName in InputReader.input_names_list)
                dropdown.options.Add(new Dropdown.OptionData() { text = inputName });
        }
    }
}
