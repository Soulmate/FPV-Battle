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
    public const string currentVersion = "v1";
    public const int axes_count = 48;
    public const int joy_count = 3; //0 - все, 1 - первый, 2 - второй
    public static InputParameters inputParameters = new InputParameters();

    private const double MinimumAxisValueForTrue = 0.5; //минимальное значение на канале, подключенному к логическому входу, чтобы там считалось тру
    public static float[,] axes_value_arr = new float[axes_count, joy_count];

    public static float throttle;
    public static float yaw;
    public static float pitch;
    public static float roll;
    public static bool arm;
    public static bool fire;

    public static event EventHandler OnConfigChange;

    //лист воможных инпутов
    public static List<string> input_names_list = new List<string>() {
        "Trottle",
        "Yaw",
        "Pitch",
        "Roll",
        "Arm",
        "Fire" };


    private void Awake()
    {
        LoadFromFile();    
    }
    private void FixedUpdate()
    {
        ReadJoystickValues();
    }



    public static void ClearValues()
    {
        inputParameters = new InputParameters();
        OnConfigChange?.Invoke(null, new EventArgs());
    }
    
    private static void ReadJoystickValues()
    {
        for (int i = 0; i < axes_count; i++)
            for (int j = 0; j < joy_count; j++)
                axes_value_arr[i, j] = Input.GetAxis($"j{j}a{i}");
        throttle = GetInputValueFromAxis(0) ?? -1;
        yaw = GetInputValueFromAxis(1) ?? 0;
        pitch = GetInputValueFromAxis(2) ?? 0;
        roll = GetInputValueFromAxis(3) ?? 0;
        arm = (GetInputValueFromAxis(4) ?? 1) > MinimumAxisValueForTrue; //если канал не подключен, будет тру
        fire = (GetInputValueFromAxis(5) ?? -1) > MinimumAxisValueForTrue; //если канал не подключен, будет false                
    }

    private static float? GetInputValueFromAxis(int input_number)
    {
        float? input_value = null;
        var ij = CoordinatesOf<int?>(inputParameters.inputNum, input_number); //ищем номер оси, подходящий //TODO 1) это медленно 2) если забито несколько осей
        int i = ij.Item1;
        int j = ij.Item2;
        if (i != -1 && inputParameters.inputNum[i, j].HasValue) //если нашлось и забито
        {
            //исходное
            float v = axes_value_arr[i, j];
            //ограниченное            
            float min = Mathf.Min(inputParameters.min[i, j], inputParameters.max[i, j]);
            float max = Mathf.Max(inputParameters.min[i, j], inputParameters.max[i, j]);
            float center = inputParameters.center[i, j];
            float dz = inputParameters.deadZoneSize[i, j];
            if (min < center && center < max) //если валидное состояние мин макс и центр
                if (v <= min)
                    input_value = -1;
                else if (v < center - dz / 2)
                {
                    float A = 1 / (-min + center - dz / 2);
                    float B = - (center - dz / 2) * A;
                    input_value = A * v + B;
                }
                else if (v <= center + dz / 2)
                    input_value = 0;
                else if (v < max)
                {
                    float A = 1 / (max + center + dz / 2);
                    float B = - (center + dz / 2) * A;
                    input_value = A * v + B;
                }
                else  //v >=max
                    input_value = 1;
            else
                input_value = 0;            
            //инвертированное
            if (inputParameters.inverts[i, j]) input_value = -input_value;
        }
        return input_value;
    }



    [System.Serializable]
    public class InputParameters
    {
        public string version = currentVersion;
        public int?[,] inputNum = new int?[axes_count, joy_count]; //инпут для каждой оси
        public bool[,] inverts = new bool[axes_count, joy_count];  //инверт для каждой оси, по умолчанию все выключены
        public float[,] min = new float[axes_count, joy_count]; //мин  для каждой оси
        public float[,] max = new float[axes_count, joy_count]; //макс для каждой оси
        public float[,] center = new float[axes_count, joy_count]; //центральное положение
        public float[,] deadZoneSize = new float[axes_count, joy_count]; //размер мертвой зоны

        public InputParameters()
        {
            for (int i = 0; i < axes_count; i++)
                for (int j = 0; j < joy_count; j++)
                {
                    min[i, j] = -1;
                    max[i, j] = 1;
                    deadZoneSize[i, j] = 0.01f;
                }
        }
    }


    // SAVE LOAD
    public static void SaveToFile()
    {
        //string destination = Application.persistentDataPath + "/Bindings.dat";
        Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/My Games/FPV Battle");
        string destination = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/My Games/FPV Battle/Bindings.dat";

        Debug.Log("Saving to " + destination);
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, inputParameters);
        file.Close();
        Debug.Log("Saved to " + destination);
    }

    public static void LoadFromFile()
    {
        string destination = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)  + "/My Games/FPV Battle/Bindings.dat";
        Debug.Log("Loading from " + destination);

        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.Log("Bindings file not found " + destination);
            return;
        }

        bool file_format_is_good = true;
        BinaryFormatter bf = new BinaryFormatter();
        try
        {
            inputParameters = (InputParameters)bf.Deserialize(file);
            OnConfigChange?.Invoke(null, new EventArgs());
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
        if (!inputParameters.version.Equals(currentVersion))
        {
            Debug.LogError($"Версия файла различается {inputParameters.version} vs. {currentVersion} {destination}");
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
