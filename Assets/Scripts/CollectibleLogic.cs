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
    [SerializeField] float rotationSpeed;
    [SerializeField] UpAxis upAxis;
    Vector3 upDir;


    AudioSource audioSource;
    [SerializeField]
    AudioClip coinClip;

    ParticleSystem particle;
    
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
            //audioSource.PlayOneShot(coinClip);
            GetComponent<MeshRenderer>().enabled = false;
            particle.Play();
            GameManager.Instance.Collect();
            Invoke("LateDestroy", 0.3f);
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
