using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEventsManager : MonoBehaviour
{
    [Header("Sound Settings")]

    public AudioSource mpShoot;
    public AudioSource mpReload;
    public AudioClip shoot;
    public AudioClip punchHit;
    public AudioClip punchAir;
    public AudioClip magazine;
    public AudioClip cocking;
    public AudioClip trigger;
    
    public FPController player;

    public void OnShoot()
    {
        if (player.equipedWeapon.Shoot())
            mpShoot.clip = shoot;
        else
            mpShoot.clip = trigger;

        mpShoot.Play();
    }

    public void OnReloadCocking()
    {
        player.equipedWeapon.Cock();
        mpReload.clip = cocking;
        mpReload.Play();
    }

    public void OnReloadMagazine()
    {
        mpReload.clip = magazine;
        mpReload.Play();

        player.equipedWeapon.Reload(player.equipedWeapon.maxCapacity);
    }

    public void OnPunch()
    {
        if (player.CheckPunchHits())
        {
            mpShoot.clip = punchHit;
        }
        else
        {
            mpShoot.clip = punchAir;
        }

        mpShoot.Play();
    }

    public void Die()
    {
        player.OnPlayerDie();
    }

    public void HitGround()
    {
        player.OnHitGround();
    }
}
