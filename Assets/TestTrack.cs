using QRTracking;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TestTrack : MonoBehaviour
{
    public SpatialGraphNodeTracker spatialGraphNode;
    public DateTimeOffset resetTime;
    void Start()
    {
        spatialGraphNode = GetComponent<SpatialGraphNodeTracker>();
        ResetTracking();
    }

    private void Instance_QRCodeUpdated(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
    {
            spatialGraphNode.Id = e.Data.SpatialGraphNodeId;         
    }

    public void ResetTracking()
    {
        resetTime = DateTimeOffset.Now;
        QRCodesManager.Instance.QRCodeAdded += Instance_QRCodeUpdated;
        QRCodesManager.Instance.QRCodeUpdated += Instance_QRCodeUpdated;
    }

    void Update()
    {

    }
}
