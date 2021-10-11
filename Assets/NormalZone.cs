using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZone : MonoBehaviour
{
    private PlayerController _playerController;

    private void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            _playerController.atSlowZone = false;
        }
    }
}
