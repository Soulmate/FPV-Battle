using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class InputSettingUI : MonoBehaviour
{
    [SerializeField] InputReader inputReader;

    Transform scrollableContent;
    GameObject[,] uiSlider_arr;
    GameObject[,] uiToggle_arr;
    GameObject[,] uiDropdown_arr;

    private void Start()
    {
        if (inputReader == null)
            Debug.LogError("InputReader is null");

        uiSlider_arr = new GameObject[InputReader.axes_count, InputReader.joy_count];
        uiToggle_arr = new GameObject[InputReader.axes_count, InputReader.joy_count];
        uiDropdown_arr = new GameObject[InputReader.axes_count, InputReader.joy_count];


        scrollableContent = transform.Find("SlidersContainer");

        transform.Find("Button_Apply").GetComponent<Button>().onClick.AddListener(Button_Apply_Press);
        transform.Find("Button_Clear").GetComponent<Button>().onClick.AddListener(Button_Clear_Press);
        transform.Find("Button_SaveToFile").GetComponent<Button>().onClick.AddListener(Button_SaveToFile_Press);
        transform.Find("Button_LoadFromFile").GetComponent<Button>().onClick.AddListener(Button_LoadFromFile_Press);


        //лист воможных инпутов для uiDropdown
        List<Dropdown.OptionData> m_Messages = new List<Dropdown.OptionData>();
        m_Messages.Add(new Dropdown.OptionData() { text = "None" });
        foreach (string s in inputReader.input_names_list) //Добавляем все возможные инпуты
            m_Messages.Add(new Dropdown.OptionData() { text = s });

        //https://stackoverflow.com/questions/41194515/unity-create-ui-control-from-script
        DefaultControls.Resources uiResources = new DefaultControls.Resources();
        //uiResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd");
        //uiResources.checkmark = Resources.Load<Sprite>("Textures/Checkmark.png");
        Texture2D SpriteTexture = LoadTexture("Assets/Sprites/Checkmark.png");
        uiResources.checkmark = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0));


        for (int i = 0; i < InputReader.axes_count; i++)
        {
            for (int j = 0; j < InputReader.joy_count; j++)
            {
                const int y_px_period = 35;

                uiSlider_arr[i, j] = DefaultControls.CreateSlider(uiResources);
                uiSlider_arr[i, j].transform.SetParent(scrollableContent, false);
                uiSlider_arr[i, j].GetComponent<Slider>().minValue = -1;
                uiSlider_arr[i, j].GetComponent<Slider>().maxValue = 1;
                uiSlider_arr[i, j].GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                uiSlider_arr[i, j].GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                uiSlider_arr[i, j].GetComponent<RectTransform>().anchoredPosition = new Vector2(130 + j * 500, -50 - i * y_px_period);

                uiToggle_arr[i, j] = DefaultControls.CreateToggle(uiResources);
                uiToggle_arr[i, j].transform.SetParent(scrollableContent, false);
                uiToggle_arr[i, j].GetComponent<Toggle>().transform.Find("Label").GetComponent<Text>().text = "Invert";
                uiToggle_arr[i, j].GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                uiToggle_arr[i, j].GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                uiToggle_arr[i, j].GetComponent<RectTransform>().anchoredPosition = new Vector3(310 + j * 500, -50 - i * y_px_period);

                uiDropdown_arr[i, j] = DefaultControls.CreateDropdown(uiResources);
                uiDropdown_arr[i, j].transform.SetParent(scrollableContent, false);
                uiDropdown_arr[i, j].GetComponent<Dropdown>().ClearOptions();
                uiDropdown_arr[i, j].GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                uiDropdown_arr[i, j].GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                uiDropdown_arr[i, j].GetComponent<RectTransform>().anchoredPosition = new Vector3(400 + j * 500, -50 - i * y_px_period);
                foreach (Dropdown.OptionData message in m_Messages)
                    uiDropdown_arr[i, j].GetComponent<Dropdown>().options.Add(message);
            }
        }

        UpdateUIvalues();
    }

    private void Update()
    {
        for (int i = 0; i < InputReader.axes_count; i++)
            for (int j = 0; j < InputReader.joy_count; j++)
                uiSlider_arr[i, j].GetComponent<Slider>().value = inputReader.joy_axes[i, j];

    }


    private void Button_Apply_Press()
    {
        ReadUIValues();
    }
    private void Button_Clear_Press()
    {
        inputReader.ClearValues();
    }

    private void Button_SaveToFile_Press()
    {
        inputReader.SaveToFile();
    }
    private void Button_LoadFromFile_Press()
    {
        inputReader.LoadFromFile();
        UpdateUIvalues();
    }


    void ReadUIValues()
    {
        for (int i = 0; i < InputReader.axes_count; i++)
        {
            for (int j = 0; j < InputReader.joy_count; j++)
            {
                int input_num = uiDropdown_arr[i, j].GetComponent<Dropdown>().value;
                bool inverted = uiToggle_arr[i, j].GetComponent<Toggle>().isOn;
                inputReader.axesInputs[i, j] = input_num - 1; //-1 т.к. первый None               
                inputReader.axesInverts[i, j] = inverted;
            }
        }
    }



    private void UpdateUIvalues()
    {
        for (int i = 0; i < InputReader.axes_count; i++)
            for (int j = 0; j < InputReader.joy_count; j++)
            {
                uiToggle_arr[i, j].GetComponent<Toggle>().isOn = inputReader.axesInverts[i, j]; //TODO read
                uiDropdown_arr[i, j].GetComponent<Dropdown>().value =
                    inputReader.axesInputs[i, j].HasValue ?
                    inputReader.axesInputs[i, j].Value + 1 : 0; //+1 так как первый None
            }
    }

    public Texture2D LoadTexture(string FilePath)
    {
        //https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/
        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }

}
