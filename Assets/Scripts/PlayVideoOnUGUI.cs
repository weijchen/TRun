using UnityEngine;

using UnityEngine.Video;

using UnityEngine.UI;

public class PlayVideoOnUGUI : MonoBehaviour {

    private VideoPlayer videoPlayer;

    private RawImage rawImage;

    void Start ()
    {

        //获取场景中对应的组件

        videoPlayer = GetComponent<VideoPlayer> ();

        rawImage = GetComponent<RawImage> ();

    }

    

    // Update is called once per frame

    void Update ()
    {
        if(videoPlayer.texture == null)
        {
            return;
        }
        rawImage.texture = videoPlayer.texture;

    }

}