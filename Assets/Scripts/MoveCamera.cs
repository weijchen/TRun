using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerTransform;
    
    void Update()
    {
        transform.position = cameraTransform.position;
        transform.rotation = playerTransform.rotation;
    }
}
