using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTarget : MonoBehaviour
{
    public event EventHandler<OnHitEventArgs> OnShoot;
    public class OnHitEventArgs : EventArgs
    {
        public GameObject projectile;
    }

    public void Hit(GameObject proj)
    {
        OnShoot?.Invoke(this, new OnHitEventArgs() { projectile = proj }); 
    }
}
