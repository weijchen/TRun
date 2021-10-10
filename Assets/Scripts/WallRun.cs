using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Player Rotation")]
    [SerializeField] private float groundDetectDistance = 10.0f;
    [SerializeField] private float playerRotationTime = 1.5f;
    [SerializeField] private Transform groundDetectposition;
    [SerializeField] private LayerMask rotateLayer;
    [SerializeField] private Transform playerOrientation;
    
    [Header("Camera")] 
    [SerializeField] private Camera cam;
    [SerializeField] private float fov;
    [SerializeField] private float wallRunfov;
    [SerializeField] private float wallRunfovTime;
    
    public void RotateBody()
    {
        RotateCamera();
        RotateOnLayer();
    }

    private void RotateOnLayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(groundDetectposition.position, Vector3.down, out hit, groundDetectDistance, rotateLayer))
        {
            StartWallRun();
            Quaternion hitObjectRotation = hit.transform.rotation;
            playerOrientation.rotation =
                Quaternion.Slerp(playerOrientation.rotation, hitObjectRotation, Time.deltaTime / playerRotationTime);
            groundDetectposition.rotation = Quaternion.Slerp(groundDetectposition.rotation, hitObjectRotation,
                Time.deltaTime / playerRotationTime);
        }
        else
        {
            StopWallRun();
        }
    }

    private void RotateCamera()
    {
        // cam.transform.rotation = transform.rotation;
    }

    private void StartWallRun()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, wallRunfov, wallRunfovTime * Time.deltaTime);
    }

    private void StopWallRun()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, wallRunfovTime * Time.deltaTime);
    }
}
