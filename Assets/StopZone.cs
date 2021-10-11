using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopZone : MonoBehaviour
{
    private PlayerController _playerController;
    private GameManager _gameManager;

    private void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            _playerController.atStopZone = true;
            _gameManager.SetTimeIsCounting(false);
        }
    }
}
