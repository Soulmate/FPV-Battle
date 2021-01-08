using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag("Player"))
            Laps.HitCheckpoint(this);
    }
}