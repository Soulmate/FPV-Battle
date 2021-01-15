using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class DroneRates : MonoBehaviour
{
    public static Dictionary<string, float> rates_dict;
    public static event EventHandler OnConfigChange;
    public static string[] rate_names
    {
        get => new List<string>(rates_dict.Keys).ToArray();
    }

    public class AxisValues
    {
        public float roll;
        public float pitch;
        public float yaw;

        public AxisValues(float roll, float pitch, float yaw)
        {
            this.roll = roll;
            this.pitch = pitch;
            this.yaw = yaw;
        }
    }


    public static void Reset()
    {
        rates_dict = new Dictionary<string, float>();

        //дефолтные рейты бетафлая
        rates_dict.Add("rate_roll", 1);
        rates_dict.Add("rate_pitch", 1);
        rates_dict.Add("rate_yaw", 1);
        rates_dict.Add("super_roll", 0.7f);
        rates_dict.Add("super_pitch", 0.7f);
        rates_dict.Add("super_yaw", 0.7f);
        rates_dict.Add("expo_roll", 0);
        rates_dict.Add("expo_pitch", 0);
        rates_dict.Add("expo_yaw", 0);
    }

    private void Awake()
    {        
        Reset();
        LoadFromFile();
    }

    public static AxisValues GetAngularVelocities(AxisValues inputs)
    {
        return new AxisValues(
            bfcalc(inputs.roll, rates_dict["rate_roll"], rates_dict["expo_roll"], rates_dict["super_roll"]),
            bfcalc(inputs.pitch, rates_dict["rate_pitch"], rates_dict["expo_pitch"], rates_dict["super_pitch"]),
            bfcalc(inputs.yaw, rates_dict["rate_yaw"], rates_dict["expo_yaw"], rates_dict["super_yaw"]));
    }


    static float bfcalc(float rcCommand, float rcRate, float expo, float superRate)
    //from https://github.com/apocolipse/RotorPirates/blob/master/RotorPirates.py
    // calculate angular rate from rc command
    {
        float absRcCommand = Math.Abs(rcCommand);

        if (rcRate > 2.0)
            rcRate = rcRate + (14.54f * (rcRate - 2.0f));
        if (expo != 0)
            rcCommand = rcCommand * (float)Math.Pow(absRcCommand, 3) * expo + rcCommand * (1.0f - expo);

        float angleRate = 200.0f * rcRate * rcCommand;
        float rcSuperFactor;
        if (superRate != 0)
        {
            rcSuperFactor = 1.0f / (Mathf.Clamp(1.0f - (absRcCommand * (superRate)), 0.01f, 1.00f));
            angleRate *= rcSuperFactor;
        }
        return angleRate;
    }

    // SAVE LOAD
    public static void SaveToFile()
    {
        //string destination = Application.persistentDataPath + "/Bindings.dat";
        Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/My Games/FPV Battle");
        string destination = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/My Games/FPV Battle/Rates.dat";

        Debug.Log("Saving to " + destination);
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, rates_dict);
        file.Close();
        Debug.Log("Saved to " + destination);
    }

    public static void LoadFromFile()
    {
        string destination = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/My Games/FPV Battle/Rates.dat";
        Debug.Log("Loading from " + destination);

        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.Log("Rates file not found " + destination);
            return;
        }

        bool file_format_is_good = true;
        BinaryFormatter bf = new BinaryFormatter();
        try
        {
            rates_dict = (Dictionary<string, float>)bf.Deserialize(file);
            OnConfigChange?.Invoke(null, new EventArgs());
        }
        catch
        {
            file_format_is_good = false;
        }

        file.Close();
        if (!file_format_is_good)
        {
            Debug.LogError("Invalid file format " + destination);
            File.Delete(destination);
            return;
        }
        Debug.Log("Loaded from " + destination);
    }

}
