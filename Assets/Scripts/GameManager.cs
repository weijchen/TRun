using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [Header("General")]
    [SerializeField] public bool isUsingEyeTracking = false;

    [SerializeField] float fadeSpeed = 1.5f;
    [SerializeField] float clearSpeed = 5.0f;
    [SerializeField] RawImage rawImage;
    [SerializeField] GameObject player;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;

    private int score = 0;
    private bool isFading = false;
    private bool isClearing = true;
    private float timeCounter = 0;
    private bool timeIsCounting = true;
    private bool isRespawning = false;
    
    private Vector3 lastPos;
    private Quaternion lastRot;

    private float timer = 0f;
    
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
        Time.timeScale = 1.25f;
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
        SoundManager.Instance.PlaySFX(SFXIndex.Restart);
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
        timer = 0.0f;
        player.GetComponent<PlayerController>().enabled = false;
        rawImage.gameObject.SetActive(true);
        rawImage.enabled = true;
        isRespawning = true;
        while(timer < 5)
        {
            FadeToBlack();
            timer += Time.deltaTime;
            if (rawImage.color.a > 0.95f)
            {
                timer = 0.0f;
                StartCoroutine(RespawnPlayer());
                isRespawning = false;
                yield break;
            }
            yield return null;

        }
        timer = 0.0f;
        StartCoroutine(RespawnPlayer());
    }

    private void FadeToClear()
    {
        rawImage.color = Color.Lerp(rawImage.color, Color.clear, clearSpeed * Time.deltaTime);
    }

    private void FadeToBlack()
    {
        rawImage.color = Color.Lerp(rawImage.color, Color.black, fadeSpeed * Time.deltaTime);
    }

    public void SetTimeIsCounting(bool state)
    {
        timeIsCounting = state;
    }
} 
