using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            PlayerController.Instance.atStopZone = true;
            GameManager.Instance.SetTimeIsCounting(false);
        }
    }
}
