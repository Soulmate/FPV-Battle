using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class RocketNavigator : MonoBehaviour
{
    public float engine_tol = 3;
    public float tol = 10;
    public float thrust = 3; //ускорение как замедления так и торможения
    public float targeting_speed = 0.1f;
    public float dragAfterDeath = 1;
    public Transform target;



    private float timeOfStart;
    private bool engine_is_on = true;
    Rigidbody rb;


    void Start()
    {
        timeOfStart = Time.time;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (engine_is_on)
        {            
            if (target != null)
            {                
                var heading = target.position - transform.position;
                Debug.DrawLine(transform.position, target.position, Color.black);
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(heading, Vector3.up), targeting_speed);
            }
            rb.AddForce(transform.forward * thrust);
            Debug.DrawRay(transform.position, transform.forward);
        }

        //TODO пореже это делать:
        

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
        GetComponentInChildren<BoxCollider>().enabled = false;
        transform.Find("Model").gameObject.SetActive(false);
        tol = Time.time + ps.main.duration + 0.1f; //удалить когда анимация кончится с запасиком
    }

    private void OutOfFuel()
    {                
        GetComponentInChildren<TrailRenderer>().enabled = false;        
        rb.useGravity = true;
        rb.drag = dragAfterDeath;
        rb.AddTorque(new Vector3(Random.Range(-100,100), Random.Range(-100, 100), Random.Range(-100, 100)));
        engine_is_on = false;
    }

    void Destroy()
    {
        Destroy(this.gameObject);
    }

    //public GameObject FindClosestTarget()
    //{
    //    Object[] gos;
    //    gos = FindObjectsOfType(typeof(RocketTarget));
    //    GameObject closest = null;
    //    float distance = Mathf.Infinity;
    //    Vector3 position = transform.position;
    //    foreach (Object go in gos)
    //    {
    //        GameObject rt = ((RocketTarget)go).gameObject;
    //        Vector3 diff = rt.transform.position - position;
    //        float curDistance = diff.sqrMagnitude;
    //        //if (curDistance < distance && curDistance > 0.1)
    //        if (curDistance < distance)
    //        {
    //            closest = rt;
    //            distance = curDistance;
    //        }
    //    }
    //    return closest;
    //}
}
