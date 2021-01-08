using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class Laps : MonoBehaviour
{
    public Transform infoText;

    public static List<Transform> checkPointArray = new List<Transform>();
    public static int currentCheckpoint = 0;
    public static int currentLap = 0;

    static float? lapStartTime = null;
    static List<float> lapTimeList = new List<float>();
    static float? currentLapTime = null;


    void Start()
    {
        currentCheckpoint = 0;
        currentLap = 0;

        foreach (Transform child in transform.Find("Checkpoints"))
            if (child.gameObject.activeSelf)
            {
                print(child.name + "active");
                checkPointArray.Add(child.transform);
            }
        else
                print(child.name + "inactive");

        print(checkPointArray);

        if (checkPointArray.Count == 0)
            Debug.LogError("No checkpoints");

        HighlightCurrentCheckpoint();
    }

    private static void HighlightCurrentCheckpoint()
    {
        foreach (var cp in checkPointArray)
            cp.GetComponent<MeshRenderer>().enabled = false;
        checkPointArray[currentCheckpoint].GetComponent<MeshRenderer>().enabled = true;
    }

    public static void HitCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoint.transform == checkPointArray[currentCheckpoint]) //TODO нормальная проверка
        {
            if (currentCheckpoint == 0)
            {
                EndTimer();
                StartTimer();
            }

            print("Checkpoint");
            if (currentCheckpoint == checkPointArray.Count - 1)
            {
                currentCheckpoint = 0;
                SwitchToNextLap();
            }
            else
            {
                currentCheckpoint++;
            }
            HighlightCurrentCheckpoint();
        }
    }

    private static void SwitchToNextLap()
    {
        print("Switch to next lap");
        currentLap++;
    }

    private static void StartTimer()
    {
        lapStartTime = Time.time;
    }
    private static void EndTimer()
    {
        if (lapStartTime.HasValue)
            lapTimeList.Add(Time.time - lapStartTime.Value);
    }

    private void Update()
    {
        if (lapStartTime.HasValue)
            currentLapTime = Time.time - lapStartTime.Value;
        else
            currentLapTime = null;

        infoText.GetComponent<Text>().text =
            $"{currentLapTime}\r\n";
        if (lapTimeList.Count > 0)
        {
            infoText.GetComponent<Text>().text += $"Last lap: {lapTimeList[lapTimeList.Count - 1]}\r\n";
            infoText.GetComponent<Text>().text += $"Best lap: {lapTimeList.Max()}\r\n";
        }
    }
}
