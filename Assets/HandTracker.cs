using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public struct FingersState
{
    public float Pinky;
    public float Ring;
    public float Middle;
    public float Index;
    public float Thumb;

    public FingersState(float pinky, float ring, float middle, float index, float thumb)
    {
        Pinky = pinky;
        Ring = ring;
        Middle = middle;
        Index = index;
        Thumb = thumb;
    }
}

public class HandTracker : MonoBehaviour
{
    public Handedness handedness;
    public TrackedHandJoint trackedJoint;
    public bool IsTracked = false;

    public FingersState handState;

    void Update()
    {
        MixedRealityPose pose;
        IsTracked = HandJointUtils.TryGetJointPose(trackedJoint, handedness, out pose);
        if (IsTracked)
        {
            transform.position = pose.Position;
            transform.rotation = pose.Rotation;

            handState.Pinky = HandPoseUtils.PinkyFingerCurl(handedness) * 100;
            handState.Ring = HandPoseUtils.RingFingerCurl(handedness) * 100;
            handState.Middle = HandPoseUtils.MiddleFingerCurl(handedness) * 100;
            handState.Index = HandPoseUtils.IndexFingerCurl(handedness) * 100;
            handState.Thumb = HandPoseUtils.ThumbFingerCurl(handedness) * 100;
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(IsTracked);
        }
    }
}
