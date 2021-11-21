using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnDetection : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(GameManager.Instance.PlayerDeath());
        }
    }
}
