using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : Weapon
{
    [SerializeField] Transform pfProjectile;
    public Transform target;
    public float throwOutSpeed = 20;//�������� � ��������� ���������� ��������������
    Transform gunEnd;
    public Transform projectileContainer; //������ �� �����, � ������� �������� ������
    Speedometer speedometer;
    //public List<Transform> collidersToIgnore = new List<Transform>();
       
  
    void Start()
    {        
        gunEnd = transform.Find("GunEnd");
        speedometer = GetComponent<Speedometer>();             
    }

    public override bool Shoot()
    {
        if (base.Shoot()) //������� ������, ����������� ������� � �.�. ������� ����� � ���� ����� ���������� ����������...
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
