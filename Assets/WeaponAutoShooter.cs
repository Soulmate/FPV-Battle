using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAutoShooter : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        foreach (var ps in GetComponentsInChildren<ProjectileShooter>())
            if (ps.gameObject.activeSelf)
                ps.Shoot();
    }
}
