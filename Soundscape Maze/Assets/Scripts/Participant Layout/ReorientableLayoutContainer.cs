using UnityEngine;
using System.Collections;

public class ReorientableLayoutContainer : MonoBehaviour
{
    public HeadTrackedCameraBehavior headTrackedCamera;

    private Vector3 resetOrientation;
    private Vector3 initialLayoutOrientation;
    private Vector3 initialCameraOrientation;

    void Start()
    {
        resetOrientation = transform.forward;
        initialLayoutOrientation = transform.forward;

        if (headTrackedCamera == null)
        {
            Debug.Log("Error: Unable to find head tracked camera...");
        }
        else
        {
            initialCameraOrientation = headTrackedCamera.transform.forward;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            transform.forward = resetOrientation;
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            initialCameraOrientation = headTrackedCamera.transform.forward;
        }

        if (Input.GetKey(KeyCode.R))
        {
            var relativeRotation = Quaternion.FromToRotation(initialCameraOrientation, headTrackedCamera.transform.forward);
            transform.forward = relativeRotation * initialLayoutOrientation;
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            initialLayoutOrientation = transform.forward;
        }
    }
}
