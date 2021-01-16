using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPController : MonoBehaviour
{
    [Header("Objecs Required")]
    public GameObject fpsCamera;
    public Camera headCamera;
    public Animator playerAnim;
    public Animator camAnim;
    public Transform head;
    public Weapon equipedWeapon;

    [Header("Move Settings")]
    public float speed = 0.1f;
    [Range(1,10)] public float Xsensitivity = 2;
    [Range(1,10)] public float Ysensitivity = 2;
    public int jumpForce = 300;
    public float stepsInterval = 0.5f;

    [Header("Head Bobbing")]
    public float bobFrequency = 5f;
    public float bobHorizontalAmplitude = 0.1f;
    public float bobHVerticalAmplitude = 0.1f;
    [Range(0, 1)] public float headBobSmoothing;

    [Header("Sounds")]
    public AudioClip jump;
    public AudioClip land;
    public AudioClip[] steps;

    private int health = 100;

    AudioSource soundPlayer;

    float MinimumX = -90;
    float MaximumX = 90;
    float xMove;
    float zMove;
    float xRot;
    float yRot;
    
    Rigidbody rb;
    CapsuleCollider capsule;
    Quaternion cameraRot;
    Quaternion characterRot;

    bool cursorIsLocked = true;
    bool lockCursor = true;
    bool isWalking;
    bool steping = false;
    int stepIndex = 0;
    float walkingTime = 0;
    Vector3 headBobbingTargetPosition;
    bool isAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        capsule = this.GetComponent<CapsuleCollider>();
        soundPlayer = this.GetComponent<AudioSource>();
        cameraRot = fpsCamera.transform.localRotation;
        characterRot = this.transform.localRotation;

    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive) {
            GetKeyInputs();
            GetMouseInput();
        }
        else
        {
            fpsCamera.transform.LookAt(fpsCamera.transform.position + this.transform.forward);
            playerAnim.SetTrigger("die");
            this.enabled = false;
        }
      
    }

    private void GetKeyInputs()
    {
        if (Input.GetMouseButtonDown(1))
            Aim();

        if (Input.GetKeyDown(KeyCode.R))
            Reload();

        if (Input.GetMouseButtonDown(0))
            Fire();

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            Jump();

        xMove = Input.GetAxis("Horizontal") * speed;
        zMove = Input.GetAxis("Vertical") * speed;

        if (Mathf.Abs(zMove) > 0 || Mathf.Abs(xMove) > 0)
            isWalking = true;
        else isWalking = false;


        playerAnim.SetBool("isRunning", isWalking);
    }

    internal void TakeDamage(int damage)
    {
        health -= damage;     
        if(health <= 0)
        {
            isAlive = false;
            //fpsCamera.transform.LookAt(this.transform.forward + transform.position);
            playerAnim.applyRootMotion = true;
            fpsCamera.GetComponent<Camera>().enabled = false;
            headCamera.enabled  = true;
            //playerAnim.SetTrigger("die");
            //this.enabled = false;
        }
        else
        {
            playerAnim.SetTrigger("hit");
        }
    }

    private void GetMouseInput()
    {
        yRot = Input.GetAxis("Mouse X") * Ysensitivity;
        xRot = Input.GetAxis("Mouse Y") * Xsensitivity;
    }

    private void Aim()
    {
        playerAnim.SetBool("isAiming", !playerAnim.GetBool("isAiming"));
        camAnim.SetBool("isAiming", !camAnim.GetBool("isAiming"));
    }

    private void Fire()
    {
        playerAnim.SetTrigger("fire");
    }

    private void Reload()
    {
        if(equipedWeapon.GetMunitionAvailable() < equipedWeapon.maxCapacity)
            playerAnim.SetTrigger("reload");
    }

    private void LateUpdate()
    {
        if (isAlive)
        {
            Move();
            Rotate();
            UpdateCursorLock();
        }
    }

    void FixedUpdate()
    {
       
       
    }

    private void Rotate()
    {
        cameraRot *= Quaternion.Euler(-xRot, 0, 0);
        characterRot *= Quaternion.Euler(0, yRot, 0);

        cameraRot = ClampRotationAroundXAxis(cameraRot);

        this.transform.localRotation = characterRot;
        fpsCamera.transform.localRotation = cameraRot;
    }

    private void HeadBobbing()
    {
        if (!isWalking) walkingTime = 0;
        else walkingTime += Time.deltaTime;

        headBobbingTargetPosition = head.transform.position + CalculateHeadBobOffset(walkingTime);

        fpsCamera.transform.position = Vector3.Lerp(fpsCamera.transform.position, headBobbingTargetPosition, headBobSmoothing);

        if ((fpsCamera.transform.position - headBobbingTargetPosition).magnitude <= 0.001)
            fpsCamera.transform.position = headBobbingTargetPosition;
    }

    private Vector3 CalculateHeadBobOffset(float time)
    {
        float horizontalOffset;
        float verticalOffset;

        Vector3 offset = Vector3.zero;

        if(time > 0)
        {
            horizontalOffset = Mathf.Cos(time * bobFrequency) * bobHorizontalAmplitude;
            verticalOffset = Mathf.Sin(time * bobFrequency * 2) * bobHVerticalAmplitude;

            offset = head.right * horizontalOffset + head.up * verticalOffset;
        }

        return offset;
    }

    private void Move()
    {
        if (isWalking)
        {
            transform.position += this.transform.forward * zMove + this.transform.right * xMove;
            HeadBobbing();
            if (!steping) {
                InvokeRepeating("PlayStepSound", 0, stepsInterval);
                steping = true;
            }
            
        }
        else
        {
            CancelInvoke("PlayStepSound");
            steping = false;
        }
    }

    private void PlayStepSound()
    {
        soundPlayer.clip = steps[stepIndex];
        soundPlayer.Play();

        stepIndex++;
        if (stepIndex == steps.Length)
            stepIndex = 0;
    }

    private void Jump()
    {
        CancelInvoke("PlayStepSound");

        rb.AddForce(0, jumpForce, 0);
        soundPlayer.clip = jump;
        soundPlayer.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            soundPlayer.clip = land;
            soundPlayer.Play();

            if (steping)
            {
                InvokeRepeating("PlayStepSound", 0.5f, stepsInterval);
            }
        } 
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    bool IsGrounded()
    {
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, capsule.radius, Vector3.down, out hitInfo,
                (capsule.height / 2f) - capsule.radius + 0.1f))
        {
            return true;
        }
        return false;
    }

    public void SetCursorLock(bool value)
    {
        lockCursor = value;
        if (!lockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        if (lockCursor)
            InternalLockUpdate();
    }

    public void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            cursorIsLocked = false;
        else if ( Input.GetMouseButtonUp(0) )
            cursorIsLocked = true;

        if (cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}