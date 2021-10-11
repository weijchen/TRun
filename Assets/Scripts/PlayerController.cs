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
    [Header("General")]
    [SerializeField] private float playerHeight = 2.0f;
    [SerializeField] public bool atSlowZone = false;
    
    [Header("Movement")]
    [SerializeField] private float verticalForceMulti = 10.0f;
    [SerializeField] private float horizontalForceMulti = 5.0f;
    [SerializeField] private GameObject boardCenter;
    [SerializeField] private GameObject boardFront;
    [SerializeField] private float maxSpeed = 100.0f;
    [SerializeField] private float groundDrag = 2.0f;
    [SerializeField] private float airMultiplier = 0.6f;
    [SerializeField] private float rotationMulti = 0.15f;

    [Header("Rotation")]
    [SerializeField] private Transform rotationBody;
    [SerializeField] private float controlRollFactor = -20.0f;

    [Header("Jump")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private float jumpForce = .5f;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isUsingEyeTracking = false;
    [SerializeField] private float jumpThreshold = 0.3f;
    [SerializeField] private float airDrag = 1.0f;
    
    [Header("On Slope")]
    [SerializeField] private Transform slopeDetector;
    [SerializeField] private float slopeRotationMulti = 3.0f;

    [Header("SFX")] 
    [SerializeField] private AudioClip bounceClip;
    [SerializeField] private AudioClip landingClip;
    
    
    private bool isForwarding = true;
    private bool isCrossing = true;
    private bool isJump = false;
    private bool isGrounded = true;
    private bool canJump = true;
    private Vector3 moveInputVal = Vector3.zero;
    private Vector3 slopeInputVal = Vector3.zero;
    private bool haveLanding = true;

    private Rigidbody _rigidbody;
    private WallRun _wallRun;
    private RaycastHit slopeHit;
    private AudioSource _audioSource;
    
    private float xThrow;
    private float yThrow;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _wallRun = GetComponent<WallRun>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        GroundCheck();
        
        // vertical movement
        if (isForwarding)
        {
            AddVerticalForce();
        }

        // horizontal movement
        if (isUsingEyeTracking)
        {
            GetEyeTrackingConfig();
        } 
        else
        {
            if (isCrossing)
            {
                AddHorizontalForce();
                CheckJump();
            }
        }

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
        
        // Set Speed Maximum Limit
        if (_rigidbody.velocity.magnitude > maxSpeed)
        { 
            _rigidbody.velocity = _rigidbody.velocity.normalized * maxSpeed;
        }

        // Jump Operation
        if (canJump)
        {
            if (isJump)
            {
                _rigidbody.AddForce(transform.up * Mathf.Sqrt(jumpForce * -2.0f * Physics.gravity.y), ForceMode.Impulse);
                isJump = false;
                haveLanding = true;
                _audioSource.PlayOneShot(bounceClip);
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

            // Debug.Log(eyeX);
            // if (eyeX >= 0)
            // {
            //     Debug.Log("here" + rotationMulti);
            //     transform.Rotate(0.0f, rotationMulti, 0.0f);
            // }
            // else
            // {
            //     transform.Rotate(0.0f, -rotationMulti, 0.0f);
            // }
            //
            if (eyeY >= jumpThreshold && isGrounded)
            {
                isJump = true;
            }
        }
    }

    private void ProcessRotation()
    {
        float row = xThrow * controlRollFactor;
        rotationBody.localRotation = Quaternion.Euler(row,-90.0f, 0);
        transform.Rotate(0.0f, Input.GetAxisRaw("Horizontal") * rotationMulti, 0.0f);
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
            _audioSource.PlayOneShot(landingClip);
            haveLanding = false;
        }
    }

    private void AddVerticalForce()
    {
        Vector3 verticalForce = boardFront.transform.position - boardCenter.transform.position;
        if (isGrounded)
        {
            Debug.Log("ground");
            moveInputVal = verticalForce * verticalForceMulti;
        }
        else
        {
            Debug.Log("air");
            moveInputVal = verticalForce * verticalForceMulti * airMultiplier;
        }
    }

    private void AddHorizontalForce()
    {
        xThrow = Input.GetAxisRaw("Horizontal") * horizontalForceMulti;
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
}
