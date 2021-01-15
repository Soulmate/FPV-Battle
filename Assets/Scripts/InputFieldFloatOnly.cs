using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class InputFieldFloatOnly : MonoBehaviour
{
    public float? Value
    {
        get => _inputValue;
        set
        {
            _inputValue = Mathf.Clamp(value.Value, minValue, maxValue);
            inputField.text = _inputValue.ToString();
        }
    }
    public float minValue = float.NegativeInfinity;
    public float maxValue = float.PositiveInfinity;
    public event EventHandler OnValueChanged;

    string decimalSeparator;
    InputField inputField;
    float? _inputValue = null;
    void Awake()
    {
        inputField = GetComponent<InputField>();
        inputField.onEndEdit.AddListener(ValidateInput);

        decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
    }

    private void ValidateInput(string arg0)
    {
        float f;
        if (float.TryParse(
            inputField.text
            .Replace(".", decimalSeparator)
            .Replace(",", decimalSeparator),
            out f))
        {
            Value = f;
            inputField.text = Value.ToString();
            OnValueChanged?.Invoke(this, new EventArgs());
        }
        else
            inputField.text = "invalid value";
    }
}
