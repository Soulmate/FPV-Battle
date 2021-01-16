using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponTypes { RocketLauncher, MachineGun };
    public WeaponTypes weaponType;
    public event EventHandler OnShoot;

    public int ammoClipSize = 10;
    public int ammoCount = 10;
    public virtual void Shoot() 
    {
        OnShoot?.Invoke(this, new EventArgs());
    }

    public void Reload()
    {

    }
}
