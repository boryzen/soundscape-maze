using UnityEngine;
using UnityEngine.XR;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

internal static class MathHelper
{
    public static float LinearMap(float value, float s0, float s1, float d0, float d1)
    {
        return d0 + (value - s0) * (d1 - d0) / (s1 - s0);
    }

    public static float WrapDegree(float degree)
    {
        if (degree > 180f)
        {
            return degree - 360f;
        }
        return degree;
    }
}

internal struct PoseSample
{
    public readonly float Timestamp;
    public Quaternion Orientation;
    public Vector3 EulerAngles;

    public PoseSample(float timestamp, Quaternion orientation)
    {
        Timestamp = timestamp;
        Orientation = orientation;

        EulerAngles = orientation.eulerAngles;
        EulerAngles.x = MathHelper.WrapDegree(EulerAngles.x);
        EulerAngles.y = MathHelper.WrapDegree(EulerAngles.y);
    }
}

public static class Extensions
{
    public static double StdDev<T>(this IEnumerable<T> list, Func<T, double> values)
    {
        // ref: https://stackoverflow.com/questions/2253874/linq-equivalent-for-standard-deviation
        // ref: http://warrenseen.com/blog/2006/03/13/how-to-calculate-standard-deviation/ 
        var mean = 0.0;
        var sum = 0.0;
        var stdDev = 0.0;
        var n = 0;
        foreach (var value in list.Select(values))
        {
            n++;
            var delta = value - mean;
            mean += delta / n;
            sum += delta * (value - mean);
        }
        if (1 < n)
            stdDev = Math.Sqrt(sum / (n - 1));

        return stdDev;

    }
}

/// <summary>
/// Nod gesture recognizer based on open source project from Katsuomi Kobayashi https://github.com/korinVR/VRGestureRecognizer
/// </summary>
public class NodRecognizer : MonoBehaviour
{
    [SerializeField]
    private HeadTrackedCameraBehavior headTracker;

    [SerializeField]
    private float recognitionInterval = 1.0f;

    public UnityEvent OnNodRecognized;

    private readonly Queue<PoseSample> PoseSamples = new Queue<PoseSample>();

    private float prevGestureTime;

    void Update()
    {
        // Record orientation
        PoseSamples.Enqueue(new PoseSample(Time.time, headTracker.ListenerOrientation));

        if (PoseSamples.Count >= 120)
            PoseSamples.Dequeue();

        // Recognize gestures
        RecognizeNod();
    }

    IEnumerable<PoseSample> Range(float startTime, float endTime) =>
        PoseSamples.Where(sample =>
            sample.Timestamp < Time.time - startTime &&
            sample.Timestamp >= Time.time - endTime);

    void RecognizeNod()
    {
        try
        {
            if (prevGestureTime > Time.time - recognitionInterval)
                return;

            var averagePitch = Range(0.3f, 0.6f).Average(sample => sample.EulerAngles.x);
            var maxPitch = Range(0.01f, 0.3f).Max(sample => sample.EulerAngles.x);
            var preStdDevPitch = Range(0.01f, 0.3f).StdDev(sample => sample.EulerAngles.x);
            var postStdDevPitch = Range(0.3f, 0.6f).StdDev(sample => sample.EulerAngles.x);

            if (!(postStdDevPitch - preStdDevPitch > 0.5f) || !(maxPitch - averagePitch > 10f) || !(Mathf.Abs(PoseSamples.First().EulerAngles.x - averagePitch) < 5f))
                return;

            prevGestureTime = Time.time;
            OnNodRecognized?.Invoke();
            Debug.Log($"User nodded... (StdDev: {preStdDevPitch}, {postStdDevPitch})");
        }
        catch (InvalidOperationException)
        {
            // Range contains no entry
        }
    }
}