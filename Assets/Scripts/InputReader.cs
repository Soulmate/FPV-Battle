using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;


//туду сделать синглтоном
public class InputReader : MonoBehaviour
{
    public const int axes_count = 48;
    public const int joy_count = 3; //0 - все, 1 - первый, 2 - второй
    private const double MinimumAxisValueForTrue = 0.5; //минимальное значение на канале, подключенному к логическому входу, чтобы там считалось тру
    public float[,] joy_axes = new float[axes_count, joy_count];

    public float throttle;
    public float yaw;
    public float pitch;
    public float roll;
    public bool arm;
    public bool fire;

    public event EventHandler OnConfigChange;

    //лист воможных инпутов
    public List<string> input_names_list = new List<string>() {
        "Trottle",
        "Yaw",
        "Pitch",
        "Roll",
        "Arm",
        "Fire" };
    public int?[,] axesInputs = new int?[axes_count, joy_count]; //инпут для каждой оси
    public bool[,] axesInverts = new bool[axes_count, joy_count]; //инверт для каждой оси, по умолчанию все выключены
    private void Start()
    {
        LoadFromFile();    
    }
    public void ClearValues()
    {
        for (int i = 0; i < axes_count; i++)
        {
            for (int j = 0; j < joy_count; j++)
            {
                axesInputs[i,j] = null;
                axesInverts[i,j] = false;
            }
        }
        OnConfigChange?.Invoke(this, new EventArgs());
    }
    void FixedUpdate()
    {
        ReadJoystickValues();
    }
    private void ReadJoystickValues()
    {
        for (int i = 0; i < axes_count; i++)
            for (int j = 0; j < joy_count; j++)
                joy_axes[i, j] = Input.GetAxis($"j{j}ax{i}");
        throttle = GetInputValueFromAxis(0) ?? -1;
        yaw = GetInputValueFromAxis(1) ?? 0;
        pitch = GetInputValueFromAxis(2) ?? 0;
        roll = GetInputValueFromAxis(3) ?? 0;
        arm = (GetInputValueFromAxis(4) ?? 1) > MinimumAxisValueForTrue; //если канал не подключен, будет тру
        fire = (GetInputValueFromAxis(5) ?? -1) > MinimumAxisValueForTrue; //если канал не подключен, будет false                
    }

    private float? GetInputValueFromAxis(int input_number)
    {
        float? input_value = null;
        var ij = CoordinatesOf<int?>(axesInputs, input_number); //ищем номер оси, подходящий //TODO 1) это медленно 2) если забито несколько осей
        if (ij.Item1 != -1 && axesInputs[ij.Item1, ij.Item2].HasValue) //если не нашлось     
        {
            input_value = joy_axes[ij.Item1, ij.Item2];
            if (axesInverts[ij.Item1, ij.Item2]) input_value = -input_value;
        }
        return input_value;
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

    public void SaveToFile()
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

    public void LoadFromFile()
    {
        string destination = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)  + "/My Games/FPV Battle/Bindings.dat";
        Debug.Log("Loading from " + destination);


        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found " + destination);
            return;
        }

        bool file_format_is_good = true;
        BinaryFormatter bf = new BinaryFormatter();
        try
        {
            InputParameters data = (InputParameters)bf.Deserialize(file);
            axesInputs = data.mapping_axes_Array;
            axesInverts = data.mapping_inverts_Array;
            OnConfigChange?.Invoke(this, new EventArgs());
        }
        catch
        {
            file_format_is_good = false;
        }

        file.Close();
        if (!file_format_is_good)
        {
            Debug.LogError("Невернвый формат файла " + destination);
            File.Delete(destination);
            return;
        }
        Debug.Log("Loaded from " + destination);
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
}
