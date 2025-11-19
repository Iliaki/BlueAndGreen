using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer;    // Arrástralo desde el Inspector
    public string nextSceneName;       // Nombre de la escena del juego

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = FindObjectOfType<VideoPlayer>();
        }

        // Cuando el video termine, se llama a OnVideoFinished
        videoPlayer.loopPointReached += OnVideoFinished;

        // Por si no tienes Play On Awake
        if (!videoPlayer.isPlaying)
        {
            videoPlayer.Play();
        }
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
