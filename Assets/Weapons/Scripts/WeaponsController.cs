using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponsController : MonoBehaviour
{   
    public Weapon selectedWeapon;
    List<Weapon> weapons;
    public AudioSource sSwitch;
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
        selectedWeapon = weapon;
        selectedWeapon.Reload();
        sSwitch?.Play();        
    }

    private void Update()
    {
        if (selectedWeapon != null)
        {
            //пытаемся стрелять если зажата стрельба
            selectedWeapon.triggerPressed = JoystickInputReader.fire;

            //перезаряжаемся если перевернулись
            if (
                selectedWeapon.ammoCount == 0 && // перезаряжаем только пустую обойму
                Vector3.Angle(transform.up, -Vector3.up) < 60 //upside down 
                )
                selectedWeapon.Reload();
        }
    }
}
