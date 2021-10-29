using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

using UnityEngine;

public struct FingersState
{
    public byte Pinky;
    public byte Ring;
    public byte Middle;
    public byte Index;
    public byte Thumb;

    public FingersState(byte pinky, byte ring, byte middle, byte index, byte thumb)
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

            handState.Pinky = (byte)(byte.MaxValue / HandPoseUtils.PinkyFingerCurl(handedness) * 90);
            handState.Ring = (byte)(byte.MaxValue / HandPoseUtils.RingFingerCurl(handedness) * 90);
            handState.Middle = (byte)(byte.MaxValue / HandPoseUtils.MiddleFingerCurl(handedness) * 90);
            handState.Index = (byte)(byte.MaxValue / HandPoseUtils.IndexFingerCurl(handedness) * 90);
            handState.Thumb = (byte)(byte.MaxValue / HandPoseUtils.ThumbFingerCurl(handedness) * 90);
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(IsTracked);
        }
    }
}
