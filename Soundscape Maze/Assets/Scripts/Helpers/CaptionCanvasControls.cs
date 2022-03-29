using UnityEngine;
using System.Collections;
using TMPro;

public class CaptionCanvasControls : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI caption;

    [SerializeField]
    private GameObject captionBackground;

    [SerializeField]
    private bool HideCaptionOnStart = true;

    private void Start()
    {
        if (HideCaptionOnStart)
        {
            HideCaption();
        }
    }

    public void ShowCaption(string text)
    {
        caption.text = text;
        captionBackground.SetActive(true);
        caption.gameObject.SetActive(true);
    }

    public void HideCaption()
    {
        caption.gameObject.SetActive(false);
        captionBackground.SetActive(false);
        caption.text = "";
    }
}
