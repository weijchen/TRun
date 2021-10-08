using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using Tobii.Gaming;
using UnityEditor.Experimental.GraphView;

/*
 * TODO: re-calibration at the beginning?
 * TODO: probably make an auto jump edge for jumping operations
 */

public class PlayerController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private float playerHeight = 2.0f;
    [SerializeField] private Camera cam;
    
    [Header("Movement")]
    [SerializeField] private float verticalForceMulti = 10.0f;
    [SerializeField] private float horizontalForceMulti = 5.0f;
    [SerializeField] private GameObject boardCenter;
    [SerializeField] private GameObject boardFront;
    [SerializeField] private float maxSpeed = 100.0f;
    [SerializeField] private float groundDrag = 2.0f;
    [SerializeField] private float airMultiplier = 0.6f;

    [Header("Jump")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private float jumpForce = .5f;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isUsingEyeTracking = false;
    [SerializeField] private float jumpThreshold = 0.3f;
    [SerializeField] private Canvas viewPort;
    [SerializeField] private float airDrag = 1.0f;
    
    private bool isForwarding = true;
    private bool isCrossing = true;
    private bool isJump = false;
    private bool isGrounded = true;
    private Vector3 moveInputVal = Vector3.zero;

    private Rigidbody _rigidbody;
    private WallRun _wallRun;

    private float horizontalMovement;
    private float verticalMovement;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _wallRun = GetComponent<WallRun>();
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
        _wallRun.RotateBody();
    }
    
    private void FixedUpdate()
    {
        if (moveInputVal != Vector3.zero)
        {
            _rigidbody.MovePosition(_rigidbody.position + moveInputVal * Time.fixedDeltaTime);
        }
        
        // Set Speed Maximum Limit
        if (_rigidbody.velocity.magnitude > maxSpeed)
        { 
            _rigidbody.velocity *= maxSpeed;
        }
        
        // Jump Operation
        if (isJump)
        {
            _rigidbody.AddForce(Vector3.up * Mathf.Sqrt(jumpForce * -2.0f * Physics.gravity.y), ForceMode.VelocityChange);
            isJump = false;
        }
    }

    private void GetEyeTrackingConfig()
    {
        GazePoint gazePointRaw = TobiiAPI.GetGazePoint();
        if (gazePointRaw.IsRecent())
        {
            float eyeX = gazePointRaw.Viewport.x - .5f;
            float eyeY = gazePointRaw.Viewport.y - .5f;
            moveInputVal.x = eyeX * horizontalForceMulti;

            if (eyeY >= jumpThreshold && isGrounded)
            {
                isJump = true;
            }
        }
    }

    private void ControlDrag()
    {
        _rigidbody.drag = isGrounded ? groundDrag : airDrag;
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, groundLayer,
            QueryTriggerInteraction.Ignore);
    }

    private void AddVerticalForce()
    {
        Vector3 verticalForce = boardFront.transform.position - boardCenter.transform.position;
        if (isGrounded)
        {
            moveInputVal = verticalForce.normalized * verticalForceMulti;
        }
        else
        {
            moveInputVal = verticalForce.normalized * verticalForceMulti * airMultiplier;
        }
    }

    private void AddHorizontalForce()
    {
        moveInputVal += Input.GetAxisRaw("Horizontal") * horizontalForceMulti * transform.right;
    }

    private void CheckJump()
    {
        if (Input.GetKey(jumpKey) && isGrounded)
        {
            isJump = true;
        }
    }
}
