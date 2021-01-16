using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem_Weapon : MonoBehaviour
{
    public Weapon.WeaponTypes weaponType;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider collider)
    {
        print(weaponType);
        //любому, кто наткнется и кому можно сменить оружие, он его сменит
        WeaponsController weaponSelector = collider.transform.GetComponentInChildren<WeaponsController>();
        if (weaponSelector != null)
        {
            weaponSelector.SelectWeapon(weaponType);
        }
    }
}
