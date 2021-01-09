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

    private int munition;

    private void Start()
    {
        munition = maxCapacity;
    }

    public int GetMunitionAvailable()
    {
        return munition;
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
                munition--;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }
    
    public int Reload(int bullets)
    {
        if(weaponType == WeaponType.RANGED)
        {
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
