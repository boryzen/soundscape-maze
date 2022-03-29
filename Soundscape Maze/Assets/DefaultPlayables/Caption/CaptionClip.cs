using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CaptionClip : PlayableAsset, ITimelineClipAsset
{
    [SerializeField]
    private CaptionBehaviour template = new CaptionBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<CaptionBehaviour>.Create(graph, template);
    }
}