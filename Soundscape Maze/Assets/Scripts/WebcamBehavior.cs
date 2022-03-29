using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WebcamBehavior : MonoBehaviour
{
    private GameObject renderView;
    private WebCamTexture webcam;
    private RawImage image;
    private AspectRatioFitter fitter;

    public Image toggleImage;

    private float defaultWidth = 640f;
    private float defaultHeight = 480f;

    void Start()
    {
        var mask = gameObject.AddComponent<Mask>();
        mask.enabled = true;

        renderView = new GameObject("Webcam Render View");
        renderView.transform.parent = gameObject.transform;
        renderView.transform.SetAsFirstSibling();
        renderView.SetActive(false);

        image = renderView.AddComponent<RawImage>();

        // Set a default aspect ratio
        fitter = renderView.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fitter.aspectRatio = defaultWidth / defaultHeight;

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(startWebcam());
        }
    }

    void OnDestroy()
    {
        stopWebcam();
    }

    private void Update()
    {
        if (webcam == null)
            return;

        fitter.aspectRatio = (float)webcam.width / (float)webcam.height;
    }

    IEnumerator startWebcam()
    {
        var devices = findWebCams();

        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.Log("webcam found");

            var rect = image.rectTransform.rect;

            webcam = new WebCamTexture(devices[0].name, (int)defaultWidth, (int)defaultHeight, 60);
            webcam.name = devices[0].name;
            image.texture = webcam;
            webcam.Play();
            renderView.SetActive(true);
        }
        else
        {
            Debug.Log("webcam not found");
        }
    }

    void stopWebcam()
    {
        renderView?.SetActive(false);
        webcam?.Stop();
        webcam = null;
    }

    WebCamDevice[] findWebCams()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        foreach (var device in devices)
        {
            Debug.Log("Name: " + device.name);
        }

        return devices;
    }

    public void toggleWebcam()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            stopWebcam();

            if (toggleImage != null)
            {
                toggleImage.enabled = false;
            }
        }
        else
        {
            if (toggleImage != null)
            {
                toggleImage.enabled = true;
            }

            gameObject.SetActive(true);
            StartCoroutine(startWebcam());
        }
    }
}
