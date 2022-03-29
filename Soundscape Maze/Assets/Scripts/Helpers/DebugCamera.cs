using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class DebugCamera : MonoBehaviour
{
    [SerializeField]
    private Canvas DisplayCanvas;

    [SerializeField]
    private float Width = 640.0f;

    [SerializeField]
    private float Height = 360.0f;

    [SerializeField]
    private float Padding = 40.0f;

    [SerializeField]
    private bool IncludeWebcam = false;

    [SerializeField]
    private TMP_FontAsset FontAsset;

    private static bool isShown = false;
    private RenderTexture texture;
    private GameObject debugCamera;
    private GameObject debugCameraView;

    void Start()
    {
        // Create the render texture for the debug camera
        texture = new RenderTexture((int)Width, (int)Height, 24);
        texture.name = "Debug Camera Texture";

        // Create the debug camera as a child of the main camera (so that it matches the main camera's position and rotation)
        debugCamera = new GameObject("Debug Camera");
        debugCamera.transform.parent = gameObject.GetComponent<Camera>().transform;
        debugCamera.transform.localRotation = new Quaternion();
        debugCamera.transform.localPosition = new Vector3();
        var camera = debugCamera.AddComponent<Camera>();
        camera.targetTexture = texture;

        // Create a view for the debug camera's output
        debugCameraView = new GameObject("Debug Camera View");
        debugCameraView.transform.parent = DisplayCanvas.transform;
        RawImage image = debugCameraView.AddComponent<RawImage>();
        image.texture = texture;

        // Position it on the canvas
        RectTransform rect = image.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector3(Width / 2.0f + Padding, -Height / 2.0f - Padding, 0);
        rect.sizeDelta = new Vector2(Width, Height);

        var label = AddLabel(debugCameraView, "Debug: 3D Headtracking");

        // Position it on the canvas
        RectTransform rect2 = label.GetComponent<RectTransform>();
        rect2.anchorMin = new Vector2(0, 1);
        rect2.anchorMax = new Vector2(0, 1);
        rect2.pivot = new Vector2(0.5f, 0.5f);
        rect2.anchoredPosition = new Vector3(Width / 2.0f + 10, -Height / 2.0f - 10f, 0);
        rect2.sizeDelta = new Vector2(Width, Height);

        if (IncludeWebcam)
        {
            AddWebcam(debugCameraView);
        }

        SetVisibility();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            isShown = !isShown;
            SetVisibility();
        }
    }

    private void SetVisibility()
    {
        if (isShown)
        {
            debugCamera.SetActive(true);
            debugCameraView.SetActive(true);
        }
        else
        {
            debugCamera.SetActive(false);
            debugCameraView.SetActive(false);
        }
    }

    private void AddWebcam(GameObject debugCameraView)
    {
        // Create the webcam view
        GameObject obj = new GameObject("Debug Webcam View");
        obj.transform.parent = debugCameraView.transform;
        _ = obj.AddComponent<RawImage>();
        _ = obj.AddComponent<WebcamBehavior>();

        // Position it on the canvas
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector3(Width / 2.0f, -Height / 2.0f - Padding, 0);
        rect.sizeDelta = new Vector2(Width, Height);
        rect.rotation = Quaternion.Euler(0f, 180f, 0f);

        var label = AddLabel(obj, "Debug: Webcam");

        // Position it on the canvas
        RectTransform rect2 = label.GetComponent<RectTransform>();
        rect2.anchorMin = new Vector2(0, 1);
        rect2.anchorMax = new Vector2(0, 1);
        rect2.pivot = new Vector2(0.5f, 0.5f);
        rect2.anchoredPosition = new Vector3(Width / 2.0f - 10f, -Height / 2.0f - 10f, 0);
        rect2.sizeDelta = new Vector2(Width, Height);
    }

    private GameObject AddLabel(GameObject view, string label)
    {
        GameObject obj = new GameObject("Debug Label");
        obj.transform.parent = view.transform;
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.fontSize = 24f;
        tmp.font = FontAsset;

        return obj;
    }
}
