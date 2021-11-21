using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpAxis
{
    x,
    y,
    z
}

public class CollectibleLogic : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private UpAxis upAxis;
    [SerializeField] private float deleteAfterSecond = 1.0f;
    
    private Vector3 upDir;
    private ParticleSystem particle;
    
    void Start()
    {
        particle = GetComponentInChildren<ParticleSystem>();
        particle.Stop();
        SetUpAxis();
    }

    void Update()
    {
        transform.Rotate(upDir * rotationSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GetComponent<MeshRenderer>().enabled = false;
            particle.Play();
            SoundManager.Instance.PlaySFX(SFXIndex.Collect);
            GameManager.Instance.Collect();
            PlayerController.Instance.SpeedBost();
            // Destroy(gameObject, 2.0f);
            Invoke("LateDestroy", deleteAfterSecond);
        }
    }

    private void SetUpAxis()
    {
        switch(upAxis)
        {
            case UpAxis.x:
                upDir = transform.right;
                break;
            case UpAxis.y:
                upDir = transform.up;
                break;
            case UpAxis.z:
                upDir = transform.forward;
                break;
        }
    }

    void LateDestroy()
    {
        Destroy(gameObject);
    }
}
