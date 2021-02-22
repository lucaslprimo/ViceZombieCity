using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    GameObject player;
    NavMeshAgent navMeshAgent;
    Animator anim;

    [Header("Movement Settings")]
    [SerializeField] float rotationSpeed = 20f;
    [SerializeField] float patrolRadius = 10f;
    [SerializeField] float minRange = 1.5f;
    [SerializeField] float maxRange = 10f;
    [SerializeField] float visionAngle = 60f;

    [Header("Attack Settings")]
    [SerializeField] int damage = 20;
    [SerializeField] Transform colliderAttack;
    [SerializeField] float attackRadius = 5f;

    [SerializeField] float timeToVanish = 10f;

    [Header("Sound Settings")]
    [SerializeField] AudioSource mp1;
    [SerializeField] AudioSource mp2;
    [SerializeField] AudioClip step1;
    [SerializeField] AudioClip step2;
    [SerializeField] AudioClip step3;
    [SerializeField] AudioClip scream;
    [SerializeField] AudioClip hitGround;
    [SerializeField] AudioClip[] idle;

    [SerializeField] GameObject minimapTracking;

    private int health = 100;
    private PatrolAgent patrolAgent;
    private PatrolAgent.State lastState;
    private Vector3 startPosition;
    private Vector3 targetPosition, investigateTarget;
    private float limitVisionAngle;

    private void Awake()
    {   
        patrolAgent = new PatrolAgent(minRange, maxRange, maxRange);
        startPosition = this.transform.position;
        targetPosition = startPosition;
        anim = GetComponent<Animator>();
        limitVisionAngle = visionAngle;
        player = GameObject.FindGameObjectWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        InvokeRepeating("PlayIdleSound", Random.Range(0f, 2), Random.Range(2f, 5));
    }

    private void resetAnimations(PatrolAgent.State lastState, PatrolAgent.State currentState)
    {
        if(lastState != currentState)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
        }
    }

    void Update()
    {
        resetAnimations(lastState, patrolAgent.CurrentState);
        lastState = patrolAgent.CurrentState;
        switch (patrolAgent.CurrentState)
        {
            case PatrolAgent.State.PATROL:
                anim.SetBool("isWalking", true);
                Patrol();
                break;
            case PatrolAgent.State.INVESTIGATE:
                anim.SetBool("isRunning", true);
                Investigate();
                break;
            case PatrolAgent.State.CHASE:
                anim.SetBool("isRunning", true);
                Chase();
                break;
            case PatrolAgent.State.ATTACK:
                anim.SetTrigger("attack");
                break;
        }

        LookAround();
    }

    public void PlayIdleSound()
    {
        mp2.clip = idle[Random.Range(0, idle.Length)];
        mp2.Play();
    }

    private void LookAround()
    {
        if (player)
        {
            Vector3 playerDirection = player.transform.position - transform.position;
            float angle = Vector3.Angle(playerDirection, transform.forward);

            if (angle <= visionAngle)
            {
                RaycastHit hit;
                Physics.Raycast(this.transform.position, playerDirection * 100, out hit);
                if (hit.collider.CompareTag("Player"))
                {
                    patrolAgent.OnChangeTargetVision(Vector3.Distance(player.transform.position, this.transform.position));
                }
            }

            patrolAgent.OnChangeRange(Vector3.Distance(player.transform.position, this.transform.position));
        }
    }

    private void Patrol()
    {
        visionAngle = limitVisionAngle;
        if (Vector3.Distance(this.transform.position, targetPosition) <= 3)
        {
            Vector3 randomPoint = startPosition + Random.insideUnitSphere * patrolRadius;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas);
            targetPosition = hit.position;
        }

        NavMeshGoTo(targetPosition);
    }

    private void Investigate()
    {
        if (Vector3.Distance(this.transform.position, investigateTarget) > 1)
        {
            NavMeshGoTo(investigateTarget);
        }
        else
        {
            patrolAgent.InvestigationOver();
        }
    }

    public void ListenSound(Vector3 positionSound, float soundPower)
    {
        investigateTarget = positionSound;
        patrolAgent.OnHearSound(soundPower, Vector3.Distance(transform.position, positionSound));
    }

    public void TakeDamage(int damage)
    {
        visionAngle = 360;

        health -= damage;
        if(health <= 0)
        {
            Kill();
        }
        else
        {
            anim.SetTrigger("hit");
        }
    }

    public void Kill()
    {
        minimapTracking.SetActive(false);
        CancelInvoke("PlayIdleSound");
        anim.SetTrigger("die");
        this.enabled = false;
        Destroy(gameObject, timeToVanish);
    }

    public void Desitegrate()
    {
        minimapTracking.SetActive(false);
        this.enabled = false;
        Destroy(gameObject);
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

    private void Chase()
    {
        Vector3 playerDirection = player.transform.position - transform.position;

        RaycastHit hit;
        Physics.Raycast(this.transform.position, playerDirection * 100, out hit);
        if (hit.collider.CompareTag("Player"))
        {
            NavMeshGoTo(player.transform.position);
        }
        else
        {
            targetPosition = player.transform.position;
            patrolAgent.LostTargetChase();
        }
    }

    private void NavMeshGoTo(Vector3 position)
    {
        navMeshAgent.SetDestination(position);

        Vector3 targetDirection = navMeshAgent.steeringTarget - transform.position;
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
        if(Time.deltaTime > 0)
        {
            navMeshAgent.velocity = anim.deltaPosition / Time.deltaTime;
        }
    }

    public void OnStep1()
    {
        mp1.clip = step1;
        mp1.Play();
    }

    public void OnStep2()
    {
        mp1.clip = step2;
        mp1.Play();
    }

    public void OnStep3()
    {
        mp1.clip = step3;
        mp1.Play();
    }

    public void OnScream()
    {
        mp2.clip = scream;
        mp2.Play();
    }

    public void OnHitGround()
    {
        mp1.clip = hitGround;
        mp1.Play();
    }
}

