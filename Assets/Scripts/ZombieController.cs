using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    Animator anim;
    public float rotationSpeed = 20f;

    private int health = 100;

    public float timeToVanish = 10f;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        UpdateAgent();
        UpdateAnim();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            health = 0;
        }

        anim.SetTrigger("hit");       
        anim.SetInteger("health", health);
    }

    public void Kill()
    {
        anim.SetTrigger("die");
        this.enabled = false;
        Destroy(gameObject, timeToVanish);

        //TODO: Animation to vanish much fancy, like pixel desintagration
    }

    private void UpdateAnim()
    {
        anim.SetFloat("distance", Vector3.Distance(this.transform.position, player.transform.position));
        
    }

    private void UpdateAgent()
    {
        agent.SetDestination(player.transform.position);

      
        Vector3 targetDirection = agent.steeringTarget - transform.position;
        targetDirection.y = 0;

       
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection), rotationSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    private void OnAnimatorMove()
    {
        agent.velocity = anim.deltaPosition / Time.deltaTime;
    }
}

