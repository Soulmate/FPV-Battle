using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AxisSettingsPanel : MonoBehaviour
{   
    AxisSettingsView axisSettingsView;

    private bool supressOnChange = false;

    public AxisSettingsModel axisSettingsModel { 
        get
        {
            bool inverted = axisSettingsView.toggle.isOn;
            int? inputNumber;
            if (axisSettingsView.dropdown.value == 0)
                inputNumber = null;
            else
                inputNumber =  axisSettingsView.dropdown.value - 1;
            float min = axisSettingsView.slider_min.value;
            float max = axisSettingsView.slider_max.value;
            float center = axisSettingsView.slider_center.value;
            double deadZoneSize = 0;
            string text = axisSettingsView.inputField.text.Replace('.',','); //todo локаль
            if (!double.TryParse(text, out deadZoneSize) ||
                deadZoneSize < 0 || deadZoneSize > 1)
            {
                deadZoneSize = 0;                
            }            
            return (new AxisSettingsModel()
            {
                inputNumber = inputNumber,
                inverted = inverted,
                min = Mathf.Min(min, max),
                max = Mathf.Max(min, max),
                center = center,
                deadZoneSize = (float)deadZoneSize
            }) ;
        }
        set
        {
            supressOnChange = true;
            axisSettingsView.dropdown.value =
                        value.inputNumber.HasValue ?
                        value.inputNumber.Value + 1 : 0; //+1 так как первый None
            axisSettingsView.toggle.isOn = value.inverted;
            axisSettingsView.slider_min.value = value.min;
            axisSettingsView.slider_max.value = value.max;
            axisSettingsView.slider_center.value = value.center;
            supressOnChange = false;
        }
        
    } //todo спрятать

    public event EventHandler OnChange;
    void Awake()
    {
        axisSettingsView = new AxisSettingsView(
            transform.Find("Slider").GetComponent<Slider>(),
            transform.Find("Slider Min").GetComponent<Slider>(),
            transform.Find("Slider Max").GetComponent<Slider>(),
            transform.Find("Slider Center").GetComponent<Slider>(),
            GetComponentInChildren<Toggle>(),
            GetComponentInChildren<Dropdown>(),
            GetComponentInChildren<InputField>());
        axisSettingsView.toggle.onValueChanged.AddListener((change) => RaiseOnChange());
        axisSettingsView.dropdown.onValueChanged.AddListener((change) => RaiseOnChange());
        axisSettingsView.slider_max.onValueChanged.AddListener((change) => RaiseOnChange());
        axisSettingsView.slider_min.onValueChanged.AddListener((change) => RaiseOnChange());
        axisSettingsView.slider_center.onValueChanged.AddListener((change) => RaiseOnChange());
        axisSettingsView.inputField.onValueChanged.AddListener((change) => RaiseOnChange()); //todo validate
    }

    public void RaiseOnChange()
    {
        if (!supressOnChange)
            OnChange?.Invoke(this, new EventArgs());
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
        public float min;
        public float max;
        public float center;
        public float deadZoneSize;
    }

    public class AxisSettingsView
    {
        public Slider slider;
        public Slider slider_min;
        public Slider slider_max;
        public Slider slider_center;
        public Toggle toggle;
        public Dropdown dropdown;
        public InputField inputField;
        public AxisSettingsView(Slider slider, Slider slider_min, Slider slider_max, Slider slider_center, Toggle toggle, Dropdown dropdown, InputField inputField)
        {
            this.slider = slider;
            this.slider_min = slider_min;
            this.slider_max = slider_max;
            this.slider_center = slider_center;
            this.toggle = toggle;
            this.dropdown = dropdown;
            this.inputField = inputField;
            slider.minValue = -1;
            slider.maxValue = 1;
            slider_min.minValue = -1;
            slider_min.maxValue = 1;
            slider_max.minValue = -1;
            slider_max.maxValue = 1;
            slider_center.minValue = -1;
            slider_center.maxValue = 1;
            dropdown.ClearOptions();
            dropdown.options.Add(new Dropdown.OptionData() { text = "none" });
            foreach (string inputName in InputReader.input_names_list)
                dropdown.options.Add(new Dropdown.OptionData() { text = inputName });
        }
    }
}
