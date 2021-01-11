using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [SerializeField] Transform pfBullet;
    public Transform target;
    public float throwOutSpeed = 20;//скорость с короторой прожектайл выстреливается
    public Vector3 gunEndPosition;
    float? previousShotTime = null;
    Speedometer speedometer;

    public event EventHandler OnShoot;
    public class OnShootEventArgs : EventArgs
    {
        public Vector3 gunEndPosition;
        public Transform projectile;
    }
    void Start()
    {
        gunEndPosition = transform.Find("Cube").localPosition;
        speedometer = GetComponent<Speedometer>();
    }

    public void Shoot()
    {
        var projectile = Instantiate(pfBullet, transform.position + gunEndPosition, Quaternion.identity);
        projectile.rotation = this.transform.rotation;
        projectile.GetComponent<Rigidbody>().velocity = speedometer.speed + transform.forward * throwOutSpeed;
        projectile.GetComponent<RocketNavigator>().target = target;
        OnShoot?.Invoke(this, new OnShootEventArgs() { gunEndPosition = gunEndPosition, projectile = projectile });
    }

}
