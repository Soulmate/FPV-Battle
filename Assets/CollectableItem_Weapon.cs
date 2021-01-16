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
        //������, ��� ��������� � ���� ����� ������� ������, �� ��� ������
        WeaponsController weaponSelector = collider.transform.GetComponentInChildren<WeaponsController>();
        if (weaponSelector != null)
        {
            weaponSelector.SelectWeapon(weaponType);
        }
    }
}
