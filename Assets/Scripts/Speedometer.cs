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
    void FixedUpdate()
    {
        speed = (transform.position - prevPosition) / Time.fixedDeltaTime;
        prevPosition = transform.position;
    }
}
