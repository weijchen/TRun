using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 1.5f;
    [SerializeField] float clearSpeed = 5.0f;
    [SerializeField] RawImage rawImage;
    [SerializeField] GameObject player;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;

    private int score = 0;
    private bool isFading = false;
    private bool isClearing = true;
    private static GameManager _instance;
    private float timeCounter = 0;
    private bool timeIsCounting = true;
    
    private Vector3 lastPos;
    private Quaternion lastRot;
    private AudioSource _audioSource;

    private bool isRespawning = false;

    float t = 0f;
    
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        lastPos = player.transform.position;
        lastRot = player.transform.rotation;
    }

    void Update()
    {
        if (timeIsCounting)
        {
            timeCounter += Time.deltaTime;
        }
        timeText.text = GetTime();
        scoreText.text = Getscore();
    }

    public string GetTime()
    {
        string timeStr = timeCounter.ToString().Length >= 5 ? timeCounter.ToString().Substring(0, 5) : timeCounter.ToString().Substring(0, 2);
        return timeStr + " seconds";
    }

    public string Getscore()
    {
        string scoreStr = score.ToString();
        return "Score: " + scoreStr;
    }

    public void Collect()
    {
        score++;
    }

    public void StorePlayerPosition(Vector3 newPos, Quaternion newRot)
    {
        lastPos = newPos;
        lastRot = newRot;
    }

    public IEnumerator RespawnPlayer()
    {
        
        _audioSource.Play();
        player.GetComponent<PlayerController>().enabled = true;
        player.transform.position = lastPos;
        player.transform.rotation = lastRot;

        while(true)
        {
            FadeToClear();
            if (rawImage.color.a < 0.05f)
            {
                rawImage.color = Color.clear;
                rawImage.enabled = false;
                rawImage.gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }
    }

    public void ForceRespawnPlayer()
    {
        if(!isRespawning)
        {
            StartCoroutine(PlayerDeath());
        }
        
    }

    public IEnumerator PlayerDeath()
    {
        if(isRespawning)
        {
            yield break;
        }
        t = 0.0f;
        player.GetComponent<PlayerController>().enabled = false;
        rawImage.gameObject.SetActive(true);
        rawImage.enabled = true;
        isRespawning = true;
        while(t < 5)
        {
            FadeToBlack();
            t += Time.deltaTime;
            if (rawImage.color.a > 0.95f)
            {
                t = 0.0f;
                StartCoroutine(RespawnPlayer());
                isRespawning = false;
                yield break;
            }

            yield return null;

        }
        t = 0.0f;
        StartCoroutine(RespawnPlayer());
        yield break;
    }

    private void FadeToClear()
    {
        rawImage.color = Color.Lerp(rawImage.color, Color.clear, clearSpeed * Time.deltaTime);
    }

    private void FadeToBlack()
    {
        rawImage.color = Color.Lerp(rawImage.color, Color.black, fadeSpeed * Time.deltaTime);
        Debug.Log(rawImage.color.a);
    }

    public void SetTimeIsCounting(bool state)
    {
        timeIsCounting = state;
    }
} 
