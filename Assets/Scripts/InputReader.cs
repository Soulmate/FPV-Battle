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
    const int joy_count = 3; //0 - ���, 1 - ������, 2 - ������
    private const double MinimumAxisValueForTrue = 0.5; //����������� �������� �� ������, ������������� � ����������� �����, ����� ��� ��������� ���
    public static float[,] joy_axes = new float[axes_count, joy_count];    

    static Transform canvas_transform;
    static Transform scrollableContent;
    static GameObject[,] uiSlider_arr = new GameObject[axes_count, joy_count];
    static GameObject[,] uiToggle_arr = new GameObject[axes_count, joy_count];
    static GameObject[,] uiDropdown_arr = new GameObject[axes_count, joy_count];

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
    static int?[,] axesInputs = new int?[axes_count, joy_count]; //����� ��� ������ ���
    static bool[,] axesInverts = new bool[axes_count, joy_count]; //������ ��� ������ ���, �� ��������� ��� ���������

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
        scrollableContent = transform.Find("Canvas/ScrollableContent");

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
            for (int j = 0; j < joy_count; j++)
            {
                const int y_px_period = 50;

                uiSlider_arr[i,j] = DefaultControls.CreateSlider(uiResources);
                uiSlider_arr[i,j].transform.SetParent(scrollableContent, false);
                uiSlider_arr[i,j].GetComponent<Slider>().minValue = -1;
                uiSlider_arr[i,j].GetComponent<Slider>().maxValue = 1;
                uiSlider_arr[i,j].GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                uiSlider_arr[i,j].GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                uiSlider_arr[i,j].GetComponent<RectTransform>().anchoredPosition = new Vector2(130 + j*500, -50 - i * y_px_period);

                uiToggle_arr[i,j] = DefaultControls.CreateToggle(uiResources);

                uiToggle_arr[i,j].transform.SetParent(scrollableContent, false);
                uiToggle_arr[i,j].GetComponent<Toggle>().transform.Find("Label").GetComponent<Text>().text = "Invert";
                uiToggle_arr[i,j].GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                uiToggle_arr[i,j].GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                uiToggle_arr[i,j].GetComponent<RectTransform>().anchoredPosition = new Vector3(310 + j * 500, -50 - i * y_px_period);

                uiDropdown_arr[i,j] = DefaultControls.CreateDropdown(uiResources);
                uiDropdown_arr[i,j].transform.SetParent(scrollableContent, false);
                uiDropdown_arr[i,j].GetComponent<Dropdown>().ClearOptions();
                uiDropdown_arr[i,j].GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                uiDropdown_arr[i,j].GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                uiDropdown_arr[i,j].GetComponent<RectTransform>().anchoredPosition = new Vector3(400 + j * 500, -50 - i * y_px_period);
                foreach (Dropdown.OptionData message in m_Messages)
                    uiDropdown_arr[i,j].GetComponent<Dropdown>().options.Add(message);
            }
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
            for (int j = 0; j < joy_count; j++)
            {
                uiToggle_arr[i,j].GetComponent<Toggle>().isOn = axesInverts[i, j]; //TODO read

                //var me = Array.IndexOf(axesInputs, i); //���� �� ��� ������ ��� ���

                uiDropdown_arr[i, j].GetComponent<Dropdown>().value =
                    axesInputs[i, j].HasValue ?
                    axesInputs[i, j].Value + 1 : 0; //+1 ��� ��� ������ None
            }
        }
    }

    private static void ClearValues()
    {
        for (int i = 0; i < axes_count; i++)
        {
            for (int j = 0; j < joy_count; j++)
            {
                axesInputs[i,j] = null;
                axesInverts[i,j] = false;
            }
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
            for (int j = 0; j < joy_count; j++)
            {
                uiSlider_arr[i,j].GetComponent<Slider>().value = joy_axes[i,j];
            }
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
            joy_axes[i,0] = Input.GetAxis($"ax{i + 1}");
        for (int i = 0; i < axes_count; i++)
            joy_axes[i, 1] = Input.GetAxis($"j1ax{i + 1}");
        for (int i = 0; i < axes_count; i++)
            joy_axes[i, 2] = Input.GetAxis($"j2ax{i + 1}");

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
        var ij = CoordinatesOf<int?>(axesInputs, input_number); //���� ����� ���, ���������� //TODO 1) ��� �������� 2) ���� ������ ��������� ����
        if (ij.Item1 != -1 && axesInputs[ij.Item1, ij.Item2].HasValue) //���� �� �������     
        {
            input_value = joy_axes[ij.Item1, ij.Item2];
            if (axesInverts[ij.Item1, ij.Item2]) input_value = -input_value;
        }
        return input_value;
    }

    void ReadUIValues()
    {
        for (int i = 0; i < axes_count; i++)
        {
            for (int j = 0; j < joy_count; j++)
            {
                int input_num = uiDropdown_arr[i,j].GetComponent<Dropdown>().value;
                bool inverted = uiToggle_arr[i, j].GetComponent<Toggle>().isOn;
                axesInputs[i, j] = input_num - 1; //-1 �.�. ������ None               
                axesInverts[i, j] = inverted;
            }
        }
    }

    // SAVE LOAD
    [System.Serializable]
    class InputParameters
    {
        public int?[,] mapping_axes_Array;
        public bool[,] mapping_inverts_Array;

        public InputParameters(int?[,] mapping_axes_Array, bool[,] mapping_inverts_Array)
        {
            this.mapping_axes_Array = mapping_axes_Array;
            this.mapping_inverts_Array = mapping_inverts_Array;
        }
    }

    public void SaveFile()
    {
        //string destination = Application.persistentDataPath + "/Bindings.dat";
        Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/My Games/FPV Battle");
        string destination = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/My Games/FPV Battle/Bindings.dat";

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
        //string destination = Application.persistentDataPath + "/Bindings.dat";
        
        string destination = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)  + "/My Games/FPV Battle/Bindings.dat";
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



    public static Tuple<int, int> CoordinatesOf<T>(T[,] matrix, T value)
    {
        int w = matrix.GetLength(0); // width
        int h = matrix.GetLength(1); // height

        for (int x = 0; x < w; ++x)
        {
            for (int y = 0; y < h; ++y)
            {
                if (matrix[x, y].Equals(value))
                    return Tuple.Create(x, y);
            }
        }

        return Tuple.Create(-1, -1);
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