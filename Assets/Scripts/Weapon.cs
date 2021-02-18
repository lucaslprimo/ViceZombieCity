using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject muzzle;
    public GameObject blood;
    public float hitForce;
    public Text uiTextBullets;

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
        UpdateUI();
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

    public void UpdateUI()
    {
        uiTextBullets.text = string.Format("{0} / {1}", munition, player.bulletsAvailable);
    }

    public bool Shoot()
    {
        if (weaponType == WeaponType.RANGED)
        {
            if (munition > 0)
            {
                VerifyIfHits();
                Instantiate(muzzle, firePoint.position, firePoint.transform.rotation);
                StartCoroutine(AlertNearbyListeners());
                anim.SetTrigger("shoot");
                munition--;
                UpdateUI();
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

    private IEnumerator AlertNearbyListeners()
    {
        ZombieController[] zombies = FindObjectsOfType<ZombieController>();

        foreach(ZombieController zombie in zombies)
        {
            zombie.ListenSound(player.transform.position, 60);
        }
        yield return null;
    }

    private void VerifyIfHits()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(Camera.main.ScreenToWorldPoint(crossHair.transform.position), player.fpsCamera.transform.forward * 1000, out hit))
        {
            if (hit.collider.CompareTag("Head"))
            {
                Instantiate(blood, hit.point, Quaternion.identity);
                ZombieController zombie = hit.collider.GetComponentInParent<ZombieController>();
                zombie.Kill();
                applyForce(hit, zombie);
            }
            else if (hit.collider.CompareTag("Body"))
            {
                Instantiate(blood, hit.point, Quaternion.identity);
                ZombieController zombie = hit.collider.GetComponentInParent<ZombieController>();
                zombie.TakeDamage(damage);
                applyForce(hit, zombie);
            }
        }
    }

    private void applyForce(RaycastHit hit, ZombieController zombie)
    {
        if (hit.rigidbody != null)
        {
            Vector3 hitDirection = zombie.transform.position - player.transform.position;
            hit.rigidbody.AddForce(hitDirection.normalized * hitForce);
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
                if (bullets >= maxCapacity)
                {
                    spareBullets = bullets - (maxCapacity - munition);
                    munition = maxCapacity;
                }
                else
                {
                    munition = bullets;
                }

                uiTextBullets.text = string.Format("{0} / {1}", munition, spareBullets);

                return spareBullets;
            }
        }
        else
        {
            return 0;
        }
       
    }
}
