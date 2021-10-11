using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    Vector3 lastPos;
    Quaternion lastRot;

    [SerializeField]
    float fadeSpeed = 1.5f;
    [SerializeField]
    float clearSpeed = 5.0f;
    [SerializeField]
    RawImage rawImage;

    [SerializeField]
    GameObject player;

    bool isFading = false;
    bool isClearing = true;
    private static GameManager _instance;
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

    int score = 0;
    // Start is called before the first frame update
    void Start()
    {
        lastPos = player.transform.position;
        lastRot = player.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Collect()
    {
        score++;
        Debug.Log(score);
    }

    public void StorePlayerPosition(Vector3 newPos, Quaternion newRot)
    {
        lastPos = newPos;
        lastRot = newRot;
    }

    public IEnumerator RespawnPlayer()
    {
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
                yield break;
            }
            yield return null;
        }
        
    }

    public IEnumerator PlayerDeath()
    {
        player.GetComponent<PlayerController>().enabled = false;
        rawImage.enabled = true;
        while(true)
        {
            FadeToBlack();
            if (rawImage.color.a > 0.95f)
            {
                StartCoroutine(RespawnPlayer());
                yield break;
            }
            yield return null;
        }
        
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


}
