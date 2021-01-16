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
    public float timeToVanish = 10f;
    public int damage = 20;
    public Transform colliderAttack;
    public float attackRadius = 5f;

    private int health = 100;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if(player != null)
        {
            UpdateAgent();
            UpdateAnim();
            CheckAttack();
        }
       
    }

    private void CheckAttack()
    {
        if(anim.GetFloat("distance") <= 1.5f){
            anim.SetTrigger("attack");
        }
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

    public void VerifyHitsPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(colliderAttack.position, attackRadius);
        if (colliders != null && colliders.Length > 0)
        {          
            FPController playerController = player.GetComponent<FPController>();
            playerController.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(colliderAttack.position, attackRadius);
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

