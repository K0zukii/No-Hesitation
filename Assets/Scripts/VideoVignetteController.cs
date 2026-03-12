using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Video;

public class VideoVignetteController : MonoBehaviour
{
    [Header("Composants")]
    [SerializeField] private RawImage vignetteRawImage;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private string videoFileName = "SpeedLines.mp4";

    [Header("Opacity Settings")]
    [SerializeField] [Range(0f, 1f)] private float maxOpacity = 0.6f;

    [Header("Video Speed Settings")]
    [SerializeField] private float minPlaybackSpeed = 0.5f;
    [SerializeField] private float maxPlaybackSpeed = 3.0f;

    [Header("Game Limit")]
    [SerializeField] private float minSpeedThreshold = 1f;
    [SerializeField] private float maxSpeedThreshold = 3f;

    void Start()
    {
        if (videoPlayer != null)
        {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            videoPlayer.url = videoPath;

            videoPlayer.Play();
        }
    }

    void Update()
    {
        //Verifiy that all the necessary object exist
        if (gameManager == null || vignetteRawImage == null || videoPlayer == null) return;

        if (gameManager.isPlaying && !gameManager.isPaused)
        {
            float currentSpeed = gameManager.speedMultiplier;

            //Calcul of the current percentage speed of the game
            float normalizedSpeed = Mathf.InverseLerp(minSpeedThreshold, maxSpeedThreshold, currentSpeed);

            //Update the video opacity based on the current percentage speed
            float targetAlpha = Mathf.Lerp(0f, maxOpacity, normalizedSpeed);
            Color newColor = vignetteRawImage.color;
            newColor.a = targetAlpha;
            vignetteRawImage.color = newColor;

            //Update the playback speed based on the current percentage speed
            videoPlayer.playbackSpeed = Mathf.Lerp(minPlaybackSpeed, maxPlaybackSpeed, normalizedSpeed);
        }
        else
        {
            //If the game is paused or it's game over,, hide the video
            Color c = vignetteRawImage.color;
            c.a = 0f;
            vignetteRawImage.color = c;
        }
    }
}