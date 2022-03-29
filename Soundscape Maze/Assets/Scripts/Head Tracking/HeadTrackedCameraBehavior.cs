using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SurfaceHeadphonesBridge;

public class HeadTrackedCameraBehavior : MonoBehaviour
{
    public static OrientationListener Listener = new OrientationListener(28022);
    public Quaternion ListenerOrientation { get; private set; }

    private readonly Vector3 _ignoreRoll = new Vector3(1, 1, 0);
    private readonly int layerMask = 1 << 6;

    private static HeadTrackingCalibrationState calibration = new HeadTrackingCalibrationState();

    //private Vector3 offset;
    //private bool isInitialized = false;
    private List<HeadTrackingInteractable> focussed = new List<HeadTrackingInteractable>();
    private Camera headTrackedCamera;

    private bool shouldResetCalibration => (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.T);

    void Start()
    {
        headTrackedCamera = GetComponent<Camera>();

        Listener.OrientationUpdated += Listener_OrientationUpdated;

        if (!Listener.isStarted)
        {
            Listener.Start();
        }
    }

    private void Listener_OrientationUpdated(object sender, OrientationUpdatedEventArgs e)
    {
        // Get the Euler angles for the rotation by removing the roll component
        var rotation = e.EulerAngles;
        rotation.Scale(_ignoreRoll);

        if (calibration.needsCalibration)
        {
            calibration.initialEulerOffset = rotation;
            calibration.needsCalibration = false;
        }

        ListenerOrientation = Quaternion.Euler(rotation - calibration.initialEulerOffset);
    }

    void Update()
    {
        if (shouldResetCalibration)
        {
            calibration.needsCalibration = true;
        }

        Quaternion orientation = ListenerOrientation;
        if (orientation != null)
        { 
            transform.rotation = orientation;
        }

        // Create a vector at the center of our camera's viewport
        Vector3 rayOrigin = headTrackedCamera.ViewportToWorldPoint(new Vector3());

        // Declare a raycast hit to store information about what our raycast has hit
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, headTrackedCamera.transform.forward, out hit, 20f, layerMask))
        {
            HeadTrackingInteractable tracked = hit.collider.GetComponent<HeadTrackingInteractable>();

            if (tracked != null)
            {
                if (!focussed.Contains(tracked))
                {
                    focussed.Add(tracked);
                    tracked.gainFocus();
                }
            }
        }
        else if (focussed.Count > 0)
        {
            foreach(var item in focussed)
            {
                item.loseFocus();
            }

            focussed.Clear();
        }

    }
}
