using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponSelector : MonoBehaviour
{   
    public Weapon.WeaponTypes weaponSelected;
    List<Weapon> weapons;
    // Start is called before the first frame update
    void Start()
    {        
        weapons = GetComponentsInChildren<Weapon>().ToList(); //todo слишком много выделяется, еще и наследники
        SelectWeapon(Weapon.WeaponTypes.RocketLauncher);
    }

    public void SelectWeapon(Weapon.WeaponTypes weaponSelected)
    {
        foreach (var w in weapons) w.gameObject.SetActive(false);
        var weapon = weapons.Find((w) => w.weaponType == weaponSelected);
        weapon.gameObject.SetActive(true);
    }
}
