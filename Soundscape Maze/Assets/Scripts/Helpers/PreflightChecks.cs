using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PreflightChecks : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingDots;

    [SerializeField]
    private TextMeshProUGUI loadingText;

    [SerializeField]
    private GameObject startButton;

    [SerializeField]
    private HeadTrackedCameraBehavior headTracker;

    private bool headTrackerIsReady = false;
    private bool webcamIsReady = false;
    private bool awaitingWebcamPermissions = false;

    // Update is called once per frame
    void Update()
    {
        if (!headTrackerIsReady)
        {
            if (HeadTrackedCameraBehavior.Listener.isConnected)
            {
                headTrackerIsReady = true;
                loadingText.text = "Checking webcam permissions";
            }

            return;
        }

        if (!webcamIsReady && !awaitingWebcamPermissions)
        {
            awaitingWebcamPermissions = true;

            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                StartCoroutine(RequestWebcamPermissions());
            }
            else
            {
                webcamIsReady = true;
                ShowReady();
            }
        }
    }

    IEnumerator RequestWebcamPermissions()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        webcamIsReady = true;
        ShowReady();
    }

    void ShowReady()
    {
        loadingDots.SetActive(false);
        loadingText.text = "Everything is ready! Click the button below to begin.";
        startButton.SetActive(true);
    }
}
