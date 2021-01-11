using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTriggerAuto : MonoBehaviour
{
    public float shootPerSecond = 2;
    public bool isShooting = false;

    float? previousShotTime = null;
    ProjectileShooter ps;
    void Start()
    {
        ps = GetComponent<ProjectileShooter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isShooting)
        if (!previousShotTime.HasValue || Time.time - previousShotTime.Value > 1 / shootPerSecond)
        {
            previousShotTime = Time.time;
            ps.Shoot();
        }
    }
}
