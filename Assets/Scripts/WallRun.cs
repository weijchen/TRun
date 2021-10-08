using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Player Rotation")]
    [SerializeField] private float groundDetectDistance = 10.0f;
    [SerializeField] private float playerRotationTime = 1.5f;
    
    [Header("Camera")] 
    [SerializeField] private Camera cam;
    [SerializeField] private float fov;
    [SerializeField] private float wallRunfov;
    [SerializeField] private float wallRunfovTime;
    
    public void RotateBody()
    {
        cam.transform.rotation = transform.rotation;
        LayerMask layer_mask = LayerMask.GetMask("Ground");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundDetectDistance, layer_mask))
        {
            StartWallRun();
            Quaternion hitObjectRotation = hit.transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, hitObjectRotation, Time.deltaTime/playerRotationTime);
        }
        else
        {
            StopWallRun();
        }
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
