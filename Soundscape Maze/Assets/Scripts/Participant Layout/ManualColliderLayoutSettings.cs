using UnityEngine;

/// <summary>
/// Attach this script to audio sources when using the manual layout mode in the
/// ParticipantLayoutContainer. These settings determine how the collider will be
/// laid out given the manual layout of the audio sources. Setting any of the "match"
/// settings to true will cause the localPosition, localRotation, or locationScale
/// of the respective audio source to be used for the collider as well.
/// </summary>
public class ManualColliderLayoutSettings : MonoBehaviour
{
    /// <summary>
    /// Indicates if a collider should automatically be added for this audio source
    /// when the ParticipantLayoutContainer's Start() function is called.
    /// </summary>
    public bool AddColliderOnStart = true;

    public bool matchSourcePosition = true;
    public Vector3 position;
    public bool matchSourceRotation = true;
    public Quaternion rotation;
    public bool matchSourceScale = false;
    public Vector3 scale;
}