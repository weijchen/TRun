using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbWall : MonoBehaviour
{
    [SerializeField] private bool removeGravity = false;
    [SerializeField] private bool resumeGravity = false;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Player")
        {
            Debug.Log("player entered");

            if (removeGravity)
            {
                // other.rigidbody.AddForce(-Physics.gravity * 100f, ForceMode.Force);
            }

            if (resumeGravity)
            {
                other.rigidbody.useGravity = true;
            }
        }
    }
    //
    // private void OnCollisionExit(Collision other)
    // {
    //     if (other.transform.tag == "Player")
    //     {
    //         Debug.Log("player exit");
    //         other.rigidbody.freezeRotation = false;  // this line might be useless
    //         other.rigidbody.useGravity = true;
    //     }
    // }
}
