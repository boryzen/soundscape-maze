using UnityEngine;
using System.Collections;

public class AudioMixerSelector : MonoBehaviour
{
    [SerializeField]
    private AudioMixerOptions mixers;
    
    // Use this for initialization
    void Start()
    {
        var player = GetComponent<AudioSource>();

#if PLATFORM_STANDALONE_OSX
        player.outputAudioMixerGroup = mixers.MacOS;
#else
        player.outputAudioMixerGroup = mixers.UWP;
#endif
    }
}
