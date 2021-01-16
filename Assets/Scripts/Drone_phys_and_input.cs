using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class Drone_phys_and_input : MonoBehaviour
{
    // Start is called before the first frame update
    public float drone_power = 45;

    public bool allowMovment = true;
    public bool allowRotation = true;

    //todo убрать из паблика
    public Vector3 speed = new Vector3(0, 0, 0);
    public Vector3 accsel = new Vector3(0, 0, 0);

    public float phys_gravity_g = 9.8f;
    public float phys_drag_coeff = 0.3f;
    
    public bool useMyPhys = true;
    private float? my_phys_turn_off_time = null;
    public float my_phys_turn_off_duration = 0.1f; //длительность отключения физики при ударе

    AudioSource audioSource;

    public bool fight_started = true;

    Rigidbody rb;


    void Start()
    {
        //drone_cam_transform = GameObject.Find("Drone Camera").transform; //TODO использовать угол камеры, а не сам объект
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    /*void FixedUpdate()
    {
        int layerMask = 1;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(drone_cam_transform.position, drone_cam_transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(drone_cam_transform.position, drone_cam_transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            target target_that_was_hit;
            if (hit.transform.TryGetComponent<target>(out target_that_was_hit))
                target_that_was_hit.Beat();
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(drone_cam_transform.position, drone_cam_transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }*/

    private void FixedUpdate()
    {
        
    }

    void Update()
    {
        float thr = JoystickInputReader.throttle;
        float yaw = JoystickInputReader.yaw;
        float pit = JoystickInputReader.pitch;
        float rol = JoystickInputReader.roll;
        bool arm = true;//todo InputReader.arm;//InputReader.arm;
        bool fire = JoystickInputReader.fire;//InputReader.arm;

        if (my_phys_turn_off_time.HasValue && Time.time > my_phys_turn_off_time + my_phys_turn_off_duration)
            Turn_on_my_phys();

        if (useMyPhys)
        {
            

            // управление ориентацией дрона
            if (allowRotation)
            {
                DroneRates.AxisValues ang_vel = DroneRates.GetAngularVelocities(new DroneRates.AxisValues(rol, pit, yaw));
                transform.Rotate(Time.deltaTime * (new Vector3(ang_vel.pitch, ang_vel.yaw, -ang_vel.roll)));
                //rb.MoveRotation(transform.rotation * Quaternion.Euler(Time.fixedDeltaTime * (new Vector3(ang_vel.pitch, ang_vel.yaw, -ang_vel.roll))));


            }
            if (allowMovment)
            {
                float force = (thr + 1f) / 2f * drone_power;
                Vector3 acc_gravity = new Vector3(0, -phys_gravity_g, 0);
                Vector3 acc_throtle;
                if (arm)
                    acc_throtle = transform.up * force;
                else
                    acc_throtle = Vector3.zero;
                Vector3 acc_drag = -speed * phys_drag_coeff;
                accsel = acc_gravity + acc_throtle + acc_drag;
                speed = speed + accsel * Time.deltaTime;
                transform.Translate(speed * Time.deltaTime, Space.World);
                //rb.MovePosition(transform.position + speed * Time.fixedDeltaTime);
            }
        }
        else
            print("физика отключена");





        if (Input.GetKeyDown(KeyCode.Space))
            ResetDrone();
        if (Input.GetKeyDown(KeyCode.F4))
            allowMovment = !allowMovment;
        if (Input.GetKeyDown(KeyCode.F3))
            allowRotation = !allowRotation;

        //float thr = JoystickInputReader.throttle;
        

        //audio:
        audioSource.pitch = (thr + 1) * 0.5f * 0.5f + 0.5f;
        audioSource.volume = (thr + 1) * 0.5f * 0.8f + 0.2f;

        if (fire)
        {
            print("fireing");
            GetComponentInChildren<ProjectileShooter>().Shoot();
        }
    }

    void ResetDrone()
    {
        transform.position = new Vector3(0, 5, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        speed = Vector3.zero;
    }

    void OnCollisionEnter(Collision collision)
    {
        //step back
        //rb.MovePosition(transform.position - speed * 5 * Time.fixedDeltaTime);
        
        Turn_off_my_phys(); //выключить мою физику на время


        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
        if (collision.relativeVelocity.magnitude > 2)
        {
            print("БАМ");//audioSource.Play();
        }
        else
            print("бум");//audioSource.Play();
        //ResetDrone();*/
    }


    void Turn_off_my_phys()        //выключить мою физику временно
    {
        speed = Vector3.zero;
        useMyPhys = false;
        rb.isKinematic = false;
        my_phys_turn_off_time = Time.time;        
    }

    void Turn_on_my_phys()        //выключить мою физику временно
    {
        useMyPhys = true;
        rb.isKinematic = true;
        my_phys_turn_off_time = null;
    }



    float bfcalc(float rcCommand, float rcRate, float expo, float superRate)
    //from https://github.com/apocolipse/RotorPirates/blob/master/RotorPirates.py
    // calculate angular rate from rc command
    {
        Func<float, float, float, float> clamp = (float n, float minn, float maxn) => Math.Max(Math.Min(maxn, n), minn);

        float absRcCommand = Math.Abs(rcCommand);

        if (rcRate > 2.0)
            rcRate = rcRate + (14.54f * (rcRate - 2.0f));
        if (expo != 0)
            rcCommand = rcCommand * (float)Math.Pow(absRcCommand, 3) * expo + rcCommand * (1.0f - expo);

        float angleRate = 200.0f * rcRate * rcCommand;
        float rcSuperFactor;
        if (superRate != 0)
        {
            rcSuperFactor = 1.0f / (clamp(1.0f - (absRcCommand * (superRate)), 0.01f, 1.00f));
            angleRate *= rcSuperFactor;
        }
        return angleRate;
    }
}
