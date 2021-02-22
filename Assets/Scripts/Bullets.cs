using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    private FPController player;
    private AudioSource soundPlayer;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<FPController>();
        soundPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
      
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            soundPlayer.Play();
            if (player.bulletsAvailable + player.equipedWeapon.maxCapacity > player.maxBulletsLoad)
            {
                player.bulletsAvailable = player.maxBulletsLoad;
            }
            else
            {
                player.bulletsAvailable += player.equipedWeapon.maxCapacity;
            }
            player.equipedWeapon.UpdateUI();
            Destroy(gameObject,0.5f);
        }
    }
}
