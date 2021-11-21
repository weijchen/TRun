using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BGMIndex : int
{
    Intro = 0,
    Main = 1
}
        
public enum SFXIndex : int
{
    Bouncing = 0,
    Collect = 1,
    Landing = 2,
    Obstacle = 3,
    Start = 4,
    Restart = 5
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;
        
    [SerializeField] private AudioClip[] bgmList;
    [SerializeField] private AudioClip[] sfxList;
    
    private AudioSource _audioSource;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
        
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    public void PlayBGM(BGMIndex ind)
    {
        _audioSource.clip = bgmList[(int) ind];
        _audioSource.Play();
    }
    
    public void PlaySFX(SFXIndex ind, float volume = 1f)
    {
        _audioSource.PlayOneShot(sfxList[(int) ind], volume);
    }
}