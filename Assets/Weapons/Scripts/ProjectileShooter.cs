using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : Weapon
{
    [SerializeField] Transform pfProjectile;
    public Transform target;
    public float throwOutSpeed = 20;//скорость с короторой прожектайл выстреливается
    Transform gunEnd;
    public Transform projectileContainer; //объект на сцене, в который пихаются ракеты
    Speedometer speedometer;
    //public List<Transform> collidersToIgnore = new List<Transform>();
       
  
    void Start()
    {        
        gunEnd = transform.Find("GunEnd");
        speedometer = GetComponent<Speedometer>();             
    }

    public override bool Shoot()
    {
        if (base.Shoot()) //вызовем ивенты, пересчитаем патроны и т.п. базовая хрень и если вроде получается выстрелить...
        {
            var projectile = Instantiate(
                pfProjectile,
                gunEnd.position,
                Quaternion.identity,
                projectileContainer);
            projectile.rotation = this.transform.rotation;
            projectile.GetComponent<Rigidbody>().velocity =
                speedometer.speed +
                transform.forward * throwOutSpeed;
            if (target != null)
                projectile.GetComponent<RocketNavigator>().target = target;
            /* if (collidersToIgnore != null)
                 foreach (var t in collidersToIgnore)
                     Physics.IgnoreCollision(
                         projectile.GetComponent<Collider>(),
                         t.GetComponent<Collider>());*/
            return true;
        }
        else
            return false;
    }
}
