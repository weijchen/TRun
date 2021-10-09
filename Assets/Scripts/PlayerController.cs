using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using Tobii.Gaming;
using UnityEditor.Experimental.GraphView;

/*
 * TODO: add car tilt when left and right
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
    [SerializeField] private float airDrag = 1.0f;
    
    [Header("On Slope")]
    [SerializeField] private Transform slopeDetector;
    [SerializeField] private float slopeRotationMulti = 3.0f;
    
    private bool isForwarding = true;
    private bool isCrossing = true;
    private bool isJump = false;
    private bool isGrounded = true;
    private Vector3 moveInputVal = Vector3.zero;
    private Vector3 slopeInputVal = Vector3.zero;

    private Rigidbody _rigidbody;
    private WallRun _wallRun;
    private RaycastHit slopeHit;

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
        slopeInputVal = Vector3.ProjectOnPlane(moveInputVal, slopeHit.normal);
        _wallRun.RotateBody();
    }
    
    private void FixedUpdate()
    {
        if (moveInputVal != Vector3.zero)
        {
            if (OnSlope())
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, slopeHit.transform.rotation, Time.deltaTime * slopeRotationMulti);
                _rigidbody.MovePosition(_rigidbody.position + slopeInputVal * Time.fixedDeltaTime);
            }
            else
            {
                _rigidbody.MovePosition(_rigidbody.position + moveInputVal * Time.fixedDeltaTime);
            }
        }
        
        // Set Speed Maximum Limit
        if (_rigidbody.velocity.magnitude > maxSpeed)
        { 
            _rigidbody.velocity *= maxSpeed;
        }
        
        // Jump Operation
        if (isJump)
        {
            _rigidbody.AddForce(transform.up * Mathf.Sqrt(jumpForce * -2.0f * Physics.gravity.y), ForceMode.Impulse);
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

    private bool OnSlope()
    {
        if (Physics.Raycast(slopeDetector.position, Vector3.forward, out slopeHit, playerHeight / 2 + 1.0f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            return false;
        }
        return false;
    }
}
