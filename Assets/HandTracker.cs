using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTracker : MonoBehaviour
{
    public Handedness handedness;
    public TrackedHandJoint trackedJoint;
    public bool IsTracked = false;

    void Update()
    {
        MixedRealityPose pose;
        IsTracked = HandJointUtils.TryGetJointPose(trackedJoint, handedness, out pose);
        if (IsTracked)
        {
            transform.position = pose.Position;
            transform.rotation = pose.Rotation;            
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(IsTracked);
        }
    }
}
