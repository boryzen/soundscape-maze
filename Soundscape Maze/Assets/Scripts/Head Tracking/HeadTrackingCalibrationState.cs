using UnityEngine;
using System.Collections;

public class HeadTrackingCalibrationState
{
    public bool needsCalibration = true;

    /// <summary>
    /// Represents the Euler angle rotation of the headset when it first connects.
    /// This is used for aligning the user's perspective to the game world's
    /// initial perspective
    /// </summary>
    public Vector3 initialEulerOffset;
}
