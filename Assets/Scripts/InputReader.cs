using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class InputReader : MonoBehaviour
{
    //���� � ����� ��������� ��� ��� ��� ������, �������� ax1, ax2 � �.�.
    //������ 8 - ��� � ��������� �� �������, ��������� 8 - ������ ���������

    //todo �������� ����������� ����������� (��� �������� ����� ������)
    const int axes_count = 16;
    private const double MinimumAxisValueForTrue = 0.5; //����������� �������� �� ������, ������������� � ����������� �����, ����� ��� ��������� ���
    public static float[] joy_axes = new float[axes_count];

    static Transform canvas_transform;
    static GameObject[] uiSlider_arr = new GameObject[axes_count];
    static GameObject[] uiToggle_arr = new GameObject[axes_count];
    static GameObject[] uiDropdown_arr = new GameObject[axes_count];

    public static float throttle;
    public static float yaw;
    public static float pitch;
    public static float roll;
    public static bool arm;
    public static bool fire;

    //���� �������� �������
    static List<string> Input_names_list = new List<string>() {
        "Trottle",
        "Yaw",
        "Pitch",
        "Roll",
        "Arm",
        "Fire" };

    static int?[] axesInputs = new int?[axes_count] //����� ��� ������ ���
    {
        0, //"Trottle",  },
        1, //"Yaw",  },
        2, //"Pitch",  },
        3, //"Roll",  },
        null,null,null,null,null,
        4, //"Arm",  },
        5, //"Fire", },
        null,null,null,null,null
    };
    static bool[] axesInverts = new bool[axes_count]; //������ ��� ������ ���, �� ��������� ��� ���������

    static bool showingControls = true;
    static void ShowControls()
    {
        canvas_transform.gameObject.SetActive(true);
        showingControls = true;
    }
    static void HideControls()
    {
        canvas_transform.gameObject.SetActive(false);
        showingControls = false;
    }


    private void Start()
    {
        canvas_transform = transform.Find("Canvas");

        canvas_transform.Find("Button_Apply").GetComponent<Button>().onClick.AddListener(ReadUIValues);
        canvas_transform.Find("Button_SaveToFile").GetComponent<Button>().onClick.AddListener(Button_SaveToFile_Press);
        canvas_transform.Find("Button_LoadFromFile").GetComponent<Button>().onClick.AddListener(LoadFile);
        canvas_transform.Find("Button_Clear").GetComponent<Button>().onClick.AddListener(ClearValues);

        //���� �������� �������
        List<Dropdown.OptionData> m_Messages = new List<Dropdown.OptionData>();
        m_Messages.Add(new Dropdown.OptionData() { text = "None" });
        foreach (string s in Input_names_list) //��������� ��� ��������� ������
            m_Messages.Add(new Dropdown.OptionData() { text = s });

        //https://stackoverflow.com/questions/41194515/unity-create-ui-control-from-script
        DefaultControls.Resources uiResources = new DefaultControls.Resources();
        //uiResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd");
        //uiResources.checkmark = Resources.Load<Sprite>("Textures/Checkmark.png");
        Texture2D SpriteTexture = LoadTexture("Assets/Sprites/Checkmark.png");
        uiResources.checkmark = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0));


        for (int i = 0; i < axes_count; i++)
        {
            uiSlider_arr[i] = DefaultControls.CreateSlider(uiResources);
            uiSlider_arr[i].transform.SetParent(canvas_transform, false);
            uiSlider_arr[i].GetComponent<Slider>().minValue = -1;
            uiSlider_arr[i].GetComponent<Slider>().maxValue = 1;
            uiSlider_arr[i].transform.position = new Vector3(100, 1200 - i * 70, 0);

            uiToggle_arr[i] = DefaultControls.CreateToggle(uiResources);

            uiToggle_arr[i].transform.SetParent(canvas_transform, false);
            uiToggle_arr[i].GetComponent<Toggle>().transform.Find("Label").GetComponent<Text>().text = "Invert";
            uiToggle_arr[i].transform.position = new Vector3(300, 1200 - i * 70, 0);

            uiDropdown_arr[i] = DefaultControls.CreateDropdown(uiResources);
            uiDropdown_arr[i].transform.SetParent(canvas_transform, false);
            uiDropdown_arr[i].GetComponent<Dropdown>().ClearOptions();
            foreach (Dropdown.OptionData message in m_Messages)
                uiDropdown_arr[i].GetComponent<Dropdown>().options.Add(message);

            uiDropdown_arr[i].transform.position = new Vector3(400, 1200 - i * 70, 0);
        }

        UpdateUIvalues();

        LoadFile(); //��������� �� ����������� mapping_axes_Array
    }

    private void Button_SaveToFile_Press()
    {
        ReadUIValues();
        SaveFile();
    }

    private static void UpdateUIvalues()
    {
        for (int i = 0; i < axes_count; i++)
        {
            uiToggle_arr[i].GetComponent<Toggle>().isOn = axesInverts[i]; //TODO read

            //var me = Array.IndexOf(axesInputs, i); //���� �� ��� ������ ��� ���

            uiDropdown_arr[i].GetComponent<Dropdown>().value =
                axesInputs[i].HasValue ?
                axesInputs[i].Value + 1 : 0; //+1 ��� ��� ������ None
        }
    }

    private static void ClearValues()
    {
        for (int i = 0; i < axes_count; i++)
        {
            axesInputs[i] = null;
            axesInverts[i] = false;
        }
        UpdateUIvalues();
    }

    void FixedUpdate()
    {
        ReadJoystickValues();
    }
    private void Update()
    {
        for (int i = 0; i < axes_count; i++)
        {
            uiSlider_arr[i].GetComponent<Slider>().value = joy_axes[i];
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            if (showingControls)
                HideControls();
            else
                ShowControls();
    }

    private static void ReadJoystickValues()
    {
        for (int i = 0; i < axes_count; i++)
            joy_axes[i] = Input.GetAxis($"ax{i + 1}");

        throttle = GetInputValueFromAxis(0) ?? -1;
        yaw = GetInputValueFromAxis(1) ?? 0;
        pitch = GetInputValueFromAxis(2) ?? 0;
        roll = GetInputValueFromAxis(3) ?? 0;
        arm = (GetInputValueFromAxis(4) ?? 1) > MinimumAxisValueForTrue; //���� ����� �� ���������, ����� ���
        fire = (GetInputValueFromAxis(5) ?? -1) > MinimumAxisValueForTrue; //���� ����� �� ���������, ����� false                
    }

    private static float? GetInputValueFromAxis(int input_number)
    {
        float? input_value = null;
        var i = Array.IndexOf(axesInputs, input_number); //���� ����� ���, ���������� //TODO 1) ��� �������� 2) ���� ������ ��������� ����
        if (i != -1 && axesInputs[i].HasValue) //���� �� �������     
        {
            input_value = joy_axes[i];
            if (axesInverts[i]) input_value = -input_value;
        }
        return input_value;
    }

    void ReadUIValues()
    {
        for (int i = 0; i < axes_count; i++)
        {
            int input_num = uiDropdown_arr[i].GetComponent<Dropdown>().value;
            bool inverted = uiToggle_arr[i].GetComponent<Toggle>().isOn;
            axesInputs[i] = input_num - 1; //-1 �.�. ������ None               
            axesInverts[i] = inverted;
        }
    }

    // SAVE LOAD
    [System.Serializable]
    class InputParameters
    {
        public int?[] mapping_axes_Array;
        public bool[] mapping_inverts_Array;

        public InputParameters(int?[] mapping_axes_Array, bool[] mapping_inverts_Array)
        {
            this.mapping_axes_Array = mapping_axes_Array;
            this.mapping_inverts_Array = mapping_inverts_Array;
        }
    }

    public void SaveFile()
    {
        string destination = Application.persistentDataPath + "/Bindings.dat";
        Debug.Log("Saving to " + destination);
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        InputParameters data = new InputParameters(axesInputs, axesInverts);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Saved to " + destination);
    }

    public void LoadFile()
    {
        string destination = Application.persistentDataPath + "/Bindings.dat";
        Debug.Log("Loading from " + destination);


        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return;
        }

        bool file_format_is_good = true;
        BinaryFormatter bf = new BinaryFormatter();
        try
        {
            InputParameters data = (InputParameters)bf.Deserialize(file);
            axesInputs = data.mapping_axes_Array;
            axesInverts = data.mapping_inverts_Array;
            UpdateUIvalues();
        }
        catch
        {
            file_format_is_good = false;
        }

        file.Close();
        if (!file_format_is_good)
        {
            Debug.LogError("��������� ������ �����");
            File.Delete(destination);
            return;
        }

        Debug.Log("Loaded from " + destination);
        Debug.Log(axesInverts);
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