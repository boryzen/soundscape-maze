using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class CaptionBehaviour : PlayableBehaviour
{
    [SerializeField]
    private string caption = "";

    private CaptionCanvasControls tmp;
    private bool firstFrameHappened = false;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        tmp = playerData as CaptionCanvasControls;

        if (tmp == null)
            return;

        if (!firstFrameHappened)
        {
            tmp.ShowCaption(caption);
            firstFrameHappened = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        firstFrameHappened = false;

        if (tmp == null)
            return;

        tmp.HideCaption();
        tmp = null;
    }
}