using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    public Vector3 speed = Vector3.zero;

    private Vector3 prevPosition;
    // Start is called before the first frame update
    void Start()
    {
        prevPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        speed = (transform.position - prevPosition) / Time.deltaTime;
        prevPosition = transform.position;
    }
}
