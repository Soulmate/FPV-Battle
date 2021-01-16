using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float tol = 10;
    private float timeOfStart;
    // Start is called before the first frame update
    void Start()
    {
        timeOfStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeOfStart + tol)
            Destroy();
    }

    void Destroy()
    {
        Destroy(this.gameObject);
    }

}

