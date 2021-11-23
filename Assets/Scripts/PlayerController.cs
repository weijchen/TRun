using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

/*
 * TODO: add car tilt when left and right
 * TODO: re-calibration at the beginning?
 * TODO: probably make an auto jump edge for jumping operations
 */

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance = null;

    [Header("General")]
    [SerializeField] private float playerHeight = 2.0f;
    [SerializeField] public bool atSlowZone = false;
    [SerializeField] public bool atStopZone = false;
    
    [Header("Movement")]
    [SerializeField] private float verticalForceMulti = 10.0f;
    [SerializeField] private float verticalForceMultiBoosted = 20.0f;
    [SerializeField] private float horizontalForceMulti = 500.0f;
    [SerializeField] private float horizontalForceMultiKB = 100.0f;
    [SerializeField] private GameObject boardCenter;
    [SerializeField] private GameObject boardFront;
    [SerializeField] private float maxSpeed = 100.0f;
    [SerializeField] private float maxSpeedBoosted = 150.0f;
    [SerializeField] private float groundDrag = 2.0f;
    [SerializeField] private float airDrag = 1.0f;
    [SerializeField] private float airMultiplier = 0.6f;
    [SerializeField] private float rotationMulti = 0.15f;
    [SerializeField] private float rotationMultiKB = 0.05f;
    [SerializeField] private float boostDuration = 3.0f;

    [Header("Rotation")]
    [SerializeField] private Transform rotationBody;
    [SerializeField] private float controlRollFactor = -20.0f;

    [Header("Jump")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpForceKB = 5f;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpThreshold = 0.3f;
    
    [Header("On Slope")]
    [SerializeField] private Transform slopeDetector;
    [SerializeField] private float slopeRotationMulti = 3.0f;

    private bool isForwarding = true;
    private bool isCrossing = true;
    private bool isJump = false;
    private bool isGrounded = true;
    private bool canJump = true;
    private Vector3 moveInputVal = Vector3.zero;
    private Vector3 slopeInputVal = Vector3.zero;
    private bool haveLanding = true;
    private bool isBoosting = false;
    private float timer;

    private Rigidbody _rigidbody;
    private WallRun _wallRun;
    private RaycastHit slopeHit;
    
    private float xThrow;
    private float yThrow;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _wallRun = GetComponent<WallRun>();
    }

    void Update()
    {
        GroundCheck();
        if (isForwarding)
        {
            AddVerticalForce();
        }
        
        if (GameManager.Instance.isUsingEyeTracking)
        {
            GetEyeTrackingConfig();
        } 
        else
        {
            if (isCrossing)
            {
                AddHorizontalForce();
            }
        }
        
        CheckJump();
        
        ControlDrag();
        slopeInputVal = Vector3.ProjectOnPlane(moveInputVal, slopeHit.normal);
        _wallRun.RotateBody();
        
        if (atSlowZone)
        {
            EnterSlowZone();
        }
        else
        {
            EnterNormalZone();
        }
        
        if (atStopZone)
        {
            EnterStopZone();
        }
        
        if (isBoosting)
        {
            timer += Time.deltaTime;
            if (timer > boostDuration)
            {
                timer = 0f;
                isBoosting = false;
                
            }
        }
    }
    
    private void FixedUpdate()
    {
        if (moveInputVal != Vector3.zero)
        {
            if (OnSlope())
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, slopeHit.transform.rotation, Time.deltaTime * slopeRotationMulti);
                _rigidbody.AddForce(slopeInputVal, ForceMode.Acceleration);
            }
            else
            {
                _rigidbody.AddForce(moveInputVal, ForceMode.Acceleration);
            }
        }

        if (isBoosting)
        {
            if (_rigidbody.velocity.magnitude > maxSpeedBoosted)
            { 
                _rigidbody.velocity = _rigidbody.velocity.normalized * maxSpeedBoosted;
            }
        }
        else
        {
            if (_rigidbody.velocity.magnitude > maxSpeed)
            { 
                _rigidbody.velocity = _rigidbody.velocity.normalized * maxSpeed;
            }
        }
        
        if (canJump)
        {
            if (isJump)
            {
                if (GameManager.Instance.isUsingEyeTracking)
                {
                    _rigidbody.AddForce(transform.up * jumpForce * -2.0f * Physics.gravity.y * (float) Screen.width / Screen.height, ForceMode.Impulse);
                }
                else
                {
                    _rigidbody.AddForce(transform.up * jumpForceKB * -2.0f * Physics.gravity.y * (float) Screen.width / Screen.height, ForceMode.Impulse);
                }
                isJump = false;
                haveLanding = true;
                SoundManager.Instance.PlaySFX(SFXIndex.Bouncing);
            }    
        }
    }

    private void GetEyeTrackingConfig()
    {
        GazePoint gazePointRaw = TobiiAPI.GetGazePoint();
        if (gazePointRaw.IsValid)
        {
            float eyeX = gazePointRaw.Viewport.x - .5f;
            float eyeY = gazePointRaw.Viewport.y - .5f;
            xThrow = eyeX * horizontalForceMulti;
            moveInputVal += xThrow * transform.right;

            float row = xThrow * controlRollFactor;
            rotationBody.localRotation = Quaternion.Euler(row,-90.0f, 0);
        }
    }

    public void SpeedBost()
    {
        isBoosting = true;
    }

    private void ProcessRotation()
    {
        float row = xThrow * controlRollFactor;
        rotationBody.localRotation = Quaternion.Euler(row,-90.0f, 0);
        transform.Rotate(0.0f, Input.GetAxisRaw("Horizontal") * rotationMultiKB, 0.0f);
    }

    private void ControlDrag()
    {
        _rigidbody.drag = isGrounded ? groundDrag : airDrag;
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, groundLayer,
            QueryTriggerInteraction.Ignore);
        
        if (haveLanding)
        {
            SoundManager.Instance.PlaySFX(SFXIndex.Landing);
            haveLanding = false;
        }
    }

    private void AddVerticalForce()
    {
        Vector3 verticalForce = boardFront.transform.position - boardCenter.transform.position;

        if (isGrounded)
        {
            moveInputVal = isBoosting ? verticalForce * verticalForceMultiBoosted : verticalForce * verticalForceMulti;
            
        }
        else
        {
            moveInputVal = isBoosting ? verticalForce * verticalForceMultiBoosted * airMultiplier : verticalForce * verticalForceMulti * airMultiplier;
        }
    }

    private void AddHorizontalForce()
    {
        xThrow = Input.GetAxisRaw("Horizontal") * horizontalForceMultiKB;
        moveInputVal += xThrow * transform.right;
        ProcessRotation();
    }

    private void CheckJump()
    {
        if (Input.GetKey(jumpKey) && isGrounded)
        {
            isJump = true;
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(slopeDetector.position, Vector3.forward, out slopeHit, playerHeight / 2 + 1.0f, groundLayer))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            return false;
        }
        return false;
    }

    public void SetCanJump(bool state)
    {
        canJump = state;
    }

    private void EnterSlowZone()
    {
        verticalForceMulti = 0;
        Quaternion newAngle = Quaternion.Euler(transform.localRotation.x + 70, transform.localRotation.y, transform.localRotation.z);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, newAngle, Time.deltaTime / 1.5f);
    }
    
    private void EnterNormalZone()
    {
       verticalForceMulti = 10;
    }
    
    private void EnterStopZone()
    {
        verticalForceMulti = 0;
    }
}
