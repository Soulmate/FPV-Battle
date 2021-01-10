using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movmentVector;
    [Range (0,1)] [SerializeField] float movmentFactor;

    [Range(0.0001f, 10)] [SerializeField] float period = 1;

    Vector3 startingPosition;
    float startingTime;

    void Start()
    {
        startingPosition = transform.position;
        startingTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon)
            movmentFactor = 0;
        else
            movmentFactor = (float) Math.Sin( 2 * 3.14f / period * (Time.time - startingTime));


        Vector3 offset = movmentVector * movmentFactor;
        transform.position = startingPosition + offset;
    }
}
