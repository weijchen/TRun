using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            PlayerController.Instance.atSlowZone = false;
        }
    }
}
