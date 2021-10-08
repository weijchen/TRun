using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform playerTransform;
    
    void Update()
    {
        transform.position = cameraPosition.position;
        transform.rotation = playerTransform.rotation;
    }
}
