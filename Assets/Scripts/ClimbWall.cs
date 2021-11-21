using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbWall : MonoBehaviour
{
    /*
     * TODO: resume gravity when die
     */
    [SerializeField] private bool removeGravity = false;
    [SerializeField] private bool resumeGravity = false;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (removeGravity)
            {
                other.rigidbody.useGravity = false;
                other.transform.GetComponent<PlayerController>().SetCanJump(false);
            }

            if (resumeGravity)
            {
                other.rigidbody.useGravity = true;
                other.transform.GetComponent<PlayerController>().SetCanJump(true);
            }
        }
    }

    // private void OnCollisionExit(Collision other)
    // {
    //     if (other.transform.tag == "Player")
    //     {
    //         other.transform.GetComponent<PlayerController>().SetCanJump(true);
    //     }
    // }
}
