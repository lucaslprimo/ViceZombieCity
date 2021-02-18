using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    private FPController player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<FPController>();
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
      
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(player.bulletsAvailable + player.equipedWeapon.maxCapacity > player.maxBulletsLoad)
            {
                player.bulletsAvailable = player.maxBulletsLoad;
            }
            else
            {
                player.bulletsAvailable += player.equipedWeapon.maxCapacity;
            }
            player.equipedWeapon.UpdateUI();
            Destroy(gameObject);
        }
    }
}
