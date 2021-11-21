using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpAxis
{
    X,
    Y,
    Z
}

public class CollectibleLogic : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private UpAxis upAxis;
    [SerializeField] private float deleteAfterSecond = 1.0f;
    
    private Vector3 upDir;
    private ParticleSystem collectVFX;
    
    private void Start()
    {
        collectVFX = GetComponentInChildren<ParticleSystem>();
        collectVFX.Stop();
        SetUpAxis();
    }

    private void Update()
    {
        transform.Rotate(upDir * rotationSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GetComponent<MeshRenderer>().enabled = false;
            collectVFX.Play();
            SoundManager.Instance.PlaySFX(SFXIndex.Collect);
            GameManager.Instance.Collect();
            PlayerController.Instance.SpeedBost();
            Invoke(nameof(LateDestroy), deleteAfterSecond);
        }
    }

    private void SetUpAxis()
    {
        switch(upAxis)
        {
            case UpAxis.X:
                upDir = transform.right;
                break;
            case UpAxis.Y:
                upDir = transform.up;
                break;
            case UpAxis.Z:
                upDir = transform.forward;
                break;
        }
    }

    private void LateDestroy()
    {
        Destroy(gameObject);
    }
}
