using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEventsManager : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip shoot;
    public AudioClip magazine;
    public AudioClip cocking;
    public AudioClip trigger;
    public FPController player;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnShoot()
    {
        if (player.equipedWeapon.Shoot())
            audioSource.clip = shoot;
        else
            audioSource.clip = trigger;

        audioSource.Play();
    }

    public void OnReloadCocking()
    {
        audioSource.clip = cocking;
        audioSource.Play();
    }

    public void OnReloadMagazine()
    {
        audioSource.clip = magazine;
        audioSource.Play();

        player.equipedWeapon.Reload(player.equipedWeapon.maxCapacity);
    }
}
