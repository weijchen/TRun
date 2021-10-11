using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpReminder : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private Material collideMaterial;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
             targetObject.GetComponent<MeshRenderer>().material = collideMaterial;
        }
    }
}
