using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTriggerAuto : MonoBehaviour
{
    public float shootPerSecond = 2;
    public bool isShooting = false;

    float? previousShotTime = null;
    public Weapon ps;
    public bool armed = true;
    public bool autoshoot = false;

    void Update()
    {
        if (armed)
        {
            isShooting =
                JoystickInputReader.fire ||
                autoshoot;

            if (isShooting)
                if (!previousShotTime.HasValue || Time.time - previousShotTime.Value > 1 / shootPerSecond)
                {
                    previousShotTime = Time.time;
                    ps.Shoot();
                }
        }
    }
}

