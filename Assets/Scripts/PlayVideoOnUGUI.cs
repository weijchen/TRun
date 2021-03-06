using UnityEngine;

using UnityEngine.Video;

using UnityEngine.UI;

public class PlayVideoOnUGUI : MonoBehaviour {

    private VideoPlayer videoPlayer;
    private RawImage rawImage;

    void Start ()
    {
        videoPlayer = GetComponent<VideoPlayer> ();
        rawImage = GetComponent<RawImage> ();
    }

    void Update ()
    {
        if(videoPlayer.texture == null)
        {
            return;
        }
        rawImage.texture = videoPlayer.texture;
    }
}