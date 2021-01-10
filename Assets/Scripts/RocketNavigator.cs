using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class RocketNavigator : MonoBehaviour
{
    public float engine_tol = 3;
    public float tol = 10;
    public float acc = 3;
    public float cruiseSpeed = 20;
    public float speed = 10f;

    public GameObject target;

    public bool targeting = true;
    public float targeting_speed = 0.1f;

    private float timeOfStart;
    private bool engine_is_on = true;


    void Start()
    {
        timeOfStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (engine_is_on)
        {
            speed = speed + Mathf.Sign(cruiseSpeed - speed) * acc * Time.deltaTime;
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        //TODO пореже это делать:
        if (targeting)
        {
            target = FindClosestTarget();
            var heading = target.transform.position - transform.position;            
            Debug.DrawLine(transform.position, target.transform.position, Color.black);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(heading, Vector3.up), targeting_speed);
            //transform.rotation = Quaternion.LookRotation(heading, Vector3.up);
        }

        if (engine_is_on && Time.time > timeOfStart + engine_tol)
            OutOfFuel();

        if (Time.time > timeOfStart + tol)
            Destroy();
    }

    private void OnCollisionEnter(Collision collision)
    {
        RocketTarget rt = collision.gameObject.GetComponent<RocketTarget>();
        if (rt != null)
            rt.Hit(this.gameObject);

        Kill();
    }



    private void Kill() 
    {
        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        ps.Play();
        GetComponentInChildren<TrailRenderer>().enabled = false;
        cruiseSpeed = 0; //остановить
        targeting_speed = 0; //не вращать
        transform.Find("Model").gameObject.SetActive(false);
        tol = Time.time + ps.main.duration + 0.1f; //удалить когда анимация кончится с запасиком
    }

    private void OutOfFuel()
    {
        GetComponentInChildren<ParticleSystem>().Play();        
        GetComponentInChildren<TrailRenderer>().enabled = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.velocity = speed * this.transform.forward;
        rb.AddTorque(new Vector3(Random.Range(-100,100), Random.Range(-100, 100), Random.Range(-100, 100)));
        engine_is_on = false;
        targeting = false;
    }

    void Destroy()
    {
        Destroy(this.gameObject);
    }

    public GameObject FindClosestTarget()
    {
        Object[] gos;
        gos = FindObjectsOfType(typeof(RocketTarget));
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (Object go in gos)
        {
            GameObject rt = ((RocketTarget)go).gameObject;
            Vector3 diff = rt.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            //if (curDistance < distance && curDistance > 0.1)
            if (curDistance < distance)
            {
                closest = rt;
                distance = curDistance;
            }
        }
        return closest;
    }
}
