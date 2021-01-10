using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform pfBullet;
    [SerializeField] float engine_tol = 3; //сколько работает двигатель и таргетинг
    [SerializeField] float tol = 10;//время жизни в секундах
    [SerializeField] float acc = 3;
    [SerializeField] float initialSpeed = 20;
    [SerializeField] float cruiseSpeed = 30;
    [SerializeField] bool targeting = true;
    [SerializeField] float targeting_speed = 0.1f;

    [SerializeField] float shootPerSecond = 2;
    float? previousShotTime = null;

    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs
    {
        public int powerLevel = 1;
        public Vector3 gunEndPosition;
    }
    void Start()
    {
        OnShoot += ProjectileShooter_OnShoot;
    }

    public void Shoot()
    {
        if (!previousShotTime.HasValue || Time.time - previousShotTime.Value > 1 / shootPerSecond)
        {
            previousShotTime = Time.time;
            OnShoot?.Invoke(
                this,
                new OnShootEventArgs() { gunEndPosition = transform.Find("Cube").position }
                );
            
        }
    }
    private void ProjectileShooter_OnShoot(object sender, OnShootEventArgs e)
    {
        var bullet = Instantiate(pfBullet, e.gunEndPosition, Quaternion.identity);
        bullet.GetComponent<RocketNavigator>().acc = acc;
        bullet.GetComponent<RocketNavigator>().speed = initialSpeed;
        bullet.GetComponent<RocketNavigator>().targeting_speed = targeting_speed;
        bullet.GetComponent<RocketNavigator>().cruiseSpeed = cruiseSpeed;
        bullet.GetComponent<RocketNavigator>().targeting = targeting;
        bullet.GetComponent<RocketNavigator>().tol = tol;
        bullet.GetComponent<RocketNavigator>().engine_tol = engine_tol;
        bullet.rotation = this.transform.rotation;
    }
}
