using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        MELEE,
        RANGED
    }

    public int damage;
    public int maxCapacity;
    public WeaponType weaponType;
    public Transform firePoint;

    AimingPoint crossHair;
    private FPController player;
    private int munition;
    private Animator anim;


    private void Start()
    {
        munition = maxCapacity;
        crossHair = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<AimingPoint>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<FPController>();
        anim = GetComponent<Animator>();

    }

    public int GetMunitionAvailable()
    {
        return munition;
    }

    public void Cock()
    {
        anim.SetTrigger("reload");
    }

    public bool IsEmpty()
    {
        return munition == 0;
    }

    public bool Shoot()
    {
        if (weaponType == WeaponType.RANGED)
        {
            if (munition > 0)
            {
                anim.SetTrigger("shoot");
                VerifyIfHits();
                munition--;
                return true;
            }
            else
            {
                anim.SetBool("empty",true);
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    private void VerifyIfHits()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(Camera.main.ScreenToWorldPoint(crossHair.transform.position), player.fpsCamera.transform.forward * 1000, out hit))
        {
            if (hit.collider.CompareTag("Head"))
            {
                ZombieController zombie = hit.collider.GetComponentInParent<ZombieController>();
                zombie.Kill();
            }
            else if (hit.collider.CompareTag("Body"))
            {
                ZombieController zombie = hit.collider.GetComponentInParent<ZombieController>();
                zombie.TakeDamage(damage);
            }
        }
    }

    public int Reload(int bullets)
    {
        if(weaponType == WeaponType.RANGED)
        {
            anim.SetBool("empty", false);
            if (munition == maxCapacity)
            {
                return bullets;
            }
            else
            {
                int spareBullets = 0;
                if (bullets > maxCapacity)
                {
                    spareBullets = bullets - maxCapacity;
                    munition = maxCapacity;
                }
                else
                {
                    munition = bullets;
                }


                return spareBullets;
            }
        }
        else
        {
            return 0;
        }
       
    }
}
