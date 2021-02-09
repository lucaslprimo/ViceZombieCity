using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destrutable : MonoBehaviour
{
    [SerializeField] float secondsToDestroy;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, secondsToDestroy);
    }
}
