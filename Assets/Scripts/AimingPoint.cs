using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimingPoint : MonoBehaviour
{
    FPController player;
    Image image;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<FPController>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        
        Debug.DrawRay(Camera.main.ScreenToWorldPoint(this.transform.position), player.cam.transform.forward * 1000, Color.red);
        if (Physics.Raycast(Camera.main.ScreenToWorldPoint(this.transform.position), player.cam.transform.forward * 1000, out hit))
        {
            if(hit.collider.CompareTag("Body") || hit.collider.CompareTag("Head"))
                image.color = Color.red;
            else
                image.color = Color.white;
        }
        else
        {
            image.color = Color.white;
        }
    }

    public RaycastHit GetHitObject() {
        return hit;
    }

}
 