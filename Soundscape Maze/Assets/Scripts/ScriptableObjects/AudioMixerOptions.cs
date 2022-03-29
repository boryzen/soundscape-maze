using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

[CreateAssetMenu(fileName = "AudioMixerOptions", menuName = "ScriptableObjects/AudioMixerOptions", order = 2)]
public class AudioMixerOptions : ScriptableObject
{
    public AudioMixerGroup MacOS;
    public AudioMixerGroup UWP;
}
