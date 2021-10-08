using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbWall : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Player")
        {
            Debug.Log("player entered");
            other.rigidbody.freezeRotation = true;  // this line might be useless
            other.rigidbody.useGravity = false;
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (other.transform.tag == "Player")
        {
            Debug.Log("player exit");
            other.rigidbody.freezeRotation = false;  // this line might be useless
            other.rigidbody.useGravity = true;
        }
    }
}
