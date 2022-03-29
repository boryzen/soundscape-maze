using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class VideoHelper : MonoBehaviour
{
    [SerializeField]
    private bool playOnPrepared = true;

    [SerializeField]
    private PlayableDirector director;

    [SerializeField]
    private UnityEvent OnVideoFinished;

    [SerializeField]
    private UnityEvent OnVideoPrepared;

    private VideoPlayer video;

    // Use this for initialization
    void Start()
    {
        video = GetComponent<VideoPlayer>();

        video.prepareCompleted += Video_prepareCompleted;
        video.Prepare();

        video.loopPointReached += Video_loopPointReached;
    }

    private void Video_loopPointReached(VideoPlayer source)
    {
        OnVideoFinished?.Invoke();
    }

    private void Video_prepareCompleted(VideoPlayer source)
    {
        if (playOnPrepared)
        {
            video.Play();
            director?.Play(director.playableAsset);
        }

        OnVideoPrepared?.Invoke();
    }
}
