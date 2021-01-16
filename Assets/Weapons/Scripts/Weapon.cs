using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponTypes { RocketLauncher, MachineGun };
    public WeaponTypes weaponType;
    public event EventHandler OnShoot;
    public event EventHandler OnReload;
    public event EventHandler OnEmptyShot; //����� �������� �������� ��� ��������

    public int ammoClipSize = 10;
    public int ammoCount = 10;

    public bool autoFire = false; //�������� ���������
    public float shootPerSecond = 2;
    public bool triggerPressed = false; //������ �����
    private bool triggerPressed_prevValue = false;

    float previousShotTime = float.NegativeInfinity;

    public AudioSource sShoot;
    public AudioSource sEmpty;
    public AudioSource sReload;

    public virtual bool Shoot() //���������� ���� ���, ��� ���� ����������
    {
        if (ammoCount > 0)
        {
            ammoCount--;
            OnShoot?.Invoke(this, new EventArgs());
            if (sShoot != null)
                sShoot.Play();
            return true;
        }
        else
        {
            OnEmptyShot?.Invoke(this, new EventArgs());
            if (sEmpty != null)
                sEmpty.Play();
            return false;
        }
    }

    public void Reload()
    {

        ammoCount = ammoClipSize;
        if (sReload != null)
            sReload.Play();
        OnReload?.Invoke(this, new EventArgs());
    }


    void Update()
    {
        if (
            (autoFire && triggerPressed) || //� �������������� ������ ������ ��������� ��������
            (!autoFire && triggerPressed && !triggerPressed_prevValue)) //� ��������� �������� ���� ������ ������ �� �������

            if (Time.time > previousShotTime + 1 / shootPerSecond) //���� � ����������� �������� ������ ���������� �������
            {
                previousShotTime = Time.time;
                Shoot();
            }
        triggerPressed_prevValue = triggerPressed;
    }
}

