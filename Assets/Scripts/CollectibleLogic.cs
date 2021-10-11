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
    [SerializeField] private AudioClip coinClip;
    [SerializeField] private float deleteAfterSecond = 1.0f;
    
    private AudioSource audioSource;
    private Vector3 upDir;
    private ParticleSystem particle;
    
    void Start()
    {
        particle = GetComponentInChildren<ParticleSystem>();
        particle.Stop();
        audioSource = GetComponent<AudioSource>();
        SetUpAxis();
    }

    void Update()
    {
        transform.Rotate(upDir * rotationSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            audioSource.PlayOneShot(coinClip);
            GetComponent<MeshRenderer>().enabled = false;
            particle.Play();
            GameManager.Instance.Collect();
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
