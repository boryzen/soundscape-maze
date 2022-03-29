using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AutomaticLayoutSettings
{
    private int RowCount = 1;
    public float AngularWindow;
    public float ColliderPadding;
    public float Radius;
}

/// <summary>
/// Attach this behavior to an empty game object and then add participant audio sources
/// as children of the game object. This behavior will layout the audio sources automatically
/// (or use the manually created layout of the audio sources) and then instantiate prefab
/// colliders for the audio sources. The automatic layout settings are ignored when
/// the UseAutomaticLayout property is set to false (i.e. when the container is in
/// manual mode). When the container is in manual mode, the ManualColliderLayoutSettings
/// behavior should be attached to all audio sources so that the container knows how
/// to position the new colliders it will instantiate for the audio sources.
/// </summary>
public class ParticipantLayoutContainer : MonoBehaviour
{
    #region Game Object References
    public GameObject ParticipantPrefab;
    public List<AudioSource> ParticipantSources;
    public bool shouldInitializeNametags = false;
    #endregion

    [Range(0.0f, 1.0f)]
    public float ParticipantVolume = 1.0f;
    private float lastParticipantVolume = 1.0f;

    #region Layout Parameters
    public bool UseAutomaticLayout;
    public AutomaticLayoutSettings AutomaticLayout;
    #endregion

    #region Helpers
    private float participantWindow => AutomaticLayout.AngularWindow / ParticipantSources.Count;
    private float participantCollisionWindow => participantWindow - AutomaticLayout.ColliderPadding;
    #endregion

    #region Game Object Name Helpers
    public readonly static string ColliderSolidName = "Collider Solid";
    public readonly static string ColliderTextName = "Participant Name";
    public readonly static string NameTagAudioSourceName = "Name Tag";

    public static string ParticipantColliderName(string sourceName)
    {
        return $"{sourceName} (Collider)";
    }
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        var centeringOffset = 90.0f - AutomaticLayout.AngularWindow / 2.0f;
        var initialCenterAngle = centeringOffset + (participantWindow / 2.0f);

        Debug.Log($"Participant Window: {participantWindow}");
        Debug.Log($"Initial Angle: {initialCenterAngle}");

        for (int i = 0; i < ParticipantSources.Count; i++)
        {
            var source = ParticipantSources[i];

            // Create the collider and get references to the solid and the name label
            Vector3 position, scale;
            Quaternion rotation;

            if (UseAutomaticLayout)
            {
                // Calculate the position and orientation of the collider and audio source for this participant
                var degrees = (i * participantWindow + initialCenterAngle);
                var radians = degrees * Mathf.Deg2Rad;
                var centerNormVec = new Vector3(Mathf.Cos(radians), 0.0f, Mathf.Sin(radians));
                var outsideRadians = (degrees + participantCollisionWindow / 2.0f) * Mathf.Deg2Rad;
                var outsideNormVec = new Vector3(Mathf.Cos(outsideRadians), 0.0f, Mathf.Sin(outsideRadians));
                var outsideLocation = transform.position + outsideNormVec * AutomaticLayout.Radius;

                var sourcePosition= transform.position + centerNormVec * AutomaticLayout.Radius;
                var sourceRotation = Quaternion.Euler(0.0f, 90.0f - degrees, 0.0f);
                scale = new Vector3(2.0f * Vector3.Distance(sourcePosition, outsideLocation), 1.0f, 0.01f);

                // Automatically position the source
                source.transform.localPosition = sourcePosition;
                source.transform.localRotation = sourceRotation;

                // Use the source position and rotation for the prefab
                position = source.transform.position;
                rotation = source.transform.rotation;
            }
            else
            {
                var settings = source.GetComponent<ManualColliderLayoutSettings>();
                if (settings != null)
                {
                    position = settings.matchSourcePosition ? source.transform.position : settings.position;
                    rotation = settings.matchSourceRotation ? source.transform.rotation : settings.rotation;
                    scale = settings.matchSourceScale ? source.transform.localScale : settings.scale;
                }
                else
                {
                    position = source.transform.position;
                    rotation = source.transform.rotation;
                    scale = new Vector3(1.0f, 1.0f, 1.0f);
                }
            }

            // Create the collider, set the participant name and label text for visual debugging, configure the size
            // and appearance of the collider
            var participant = Instantiate(ParticipantPrefab, position, rotation, source.transform);
            participant.name = ParticipantColliderName(source.name);
            participant.transform.Find(ColliderSolidName).localScale = scale;
            participant.transform.Find(ColliderTextName).GetComponent<TextMesh>().text = source.name;

            // Initialize the source's volume
            source.volume = ParticipantVolume;

            //Debug.Log($"{source.name}: {degrees}°, x: {location.x}, z: {location.z}, yaw: {rotation.eulerAngles.z}");
        }

        lastParticipantVolume = ParticipantVolume;

        if (shouldInitializeNametags)
        {
            var nt = GetComponent<NametagsInteraction>();

            if (nt != null)
            {
                nt.enabled = true;
            }
        }
    }

    private void Update()
    {
        if (lastParticipantVolume != ParticipantVolume)
        {
            foreach (var source in ParticipantSources)
            {
                source.volume = ParticipantVolume;
            }

            lastParticipantVolume = ParticipantVolume;
        }
    }

    /// <summary>
    /// Adds a collider for the audio source object at the provided index in the
    /// ParticipantSources array
    /// </summary>
    /// <param name="index">Index of the audio source in the ParticipantSources array</param>
    public void AddColliderForSource(int index)
    {
        if (UseAutomaticLayout)
        {
            Debug.Log("AddColliderForSource should only be called when using manual layout mode");
            return;
        }

        if (index > ParticipantSources.Count - 1 || index < 0)
        {
            throw new Exception("Index is out of range!");
        }

        var source = ParticipantSources[index];

        var settings = source.GetComponent<ManualColliderLayoutSettings>();

        Vector3 location, scale;
        Quaternion rotation;

        if (settings != null)
        {
            location = settings.matchSourcePosition ? source.transform.localPosition : settings.position;
            rotation = settings.matchSourceRotation ? source.transform.localRotation : settings.rotation;
            scale = settings.matchSourceScale ? source.transform.localScale : settings.scale;
        }
        else
        {
            location = source.transform.localPosition;
            rotation = source.transform.rotation;
            scale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        // Create the collider and get references to the solid and the name label
        var participant = Instantiate(ParticipantPrefab, location, rotation, transform);
        var collider = participant.transform.Find(ColliderSolidName);
        var label = participant.transform.Find(ColliderTextName);

        // Set the participant name and label text for visual debugging, configure the size
        // and appearance of the collider
        participant.name = ParticipantColliderName(source.name);
        label.GetComponent<TextMesh>().text = source.name;
        collider.localScale = scale;
    }

    /// <summary>
    /// Removes the collider for the audio source object at the provided index in the
    /// ParticipantSources array
    /// </summary>
    /// <param name="index">Index of the audio source in the ParticipantSources array</param>
    public void RemoveColliderForSource(int index)
    {
        if (UseAutomaticLayout)
        {
            Debug.Log("RemoveColliderForSource should only be called when using manual layout mode");
            return;
        }

        if (index > ParticipantSources.Count - 1 || index < 0)
        {
            throw new Exception("Index is out of range!");
        }

        var collider = transform.Find(ParticipantColliderName(ParticipantSources[index].name));

        if (collider != null)
        {
            Destroy(collider.gameObject);
        }
    }

    public GameObject GetColliderForSource(int index)
    {
        if (index > ParticipantSources.Count - 1 || index < 0)
        {
            throw new Exception("Index is out of range!");
        }

        return ParticipantSources[index].transform.Find(ParticipantColliderName(ParticipantSources[index].name)).gameObject;
    }

    public HeadTrackingInteractable GetColliderSolidForSource(int index)
    {
        if (index > ParticipantSources.Count - 1 || index < 0)
        {
            throw new Exception("Index is out of range!");
        }

        return GetColliderForSource(index).transform.Find(ColliderSolidName).gameObject.GetComponent<HeadTrackingInteractable>();
    }

    public AudioSource GetNameTagForSource(int index)
    {
        if (index > ParticipantSources.Count - 1 || index < 0)
        {
            throw new Exception("Index is out of range!");
        }

        return ParticipantSources[index].transform.Find(NameTagAudioSourceName).gameObject.GetComponent<AudioSource>();
    }

    public void DisableCollisionDetection()
    {
        for(int i= 0; i < ParticipantSources.Count; i++)
        {
            GetColliderSolidForSource(i)?.Deactivate();
        }
    }

    public void EnableCollisionDetection()
    {
        for (int i = 0; i < ParticipantSources.Count; i++)
        {
            GetColliderSolidForSource(i)?.Reactivate();
        }
    }
}
