using Google.Protobuf;

using Hrm;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LocalPlayer : MonoBehaviour
{
    public Transform qrPos;
    public GameClient client;
    public int id = -1;
    
    void Start()
    {
        NetworkHandler.Processors[MessageTypes.Welcome].ReceivedMessageEvent += WelcomeReceived;
        client = FindObjectOfType<GameClient>();
    }

    private void WelcomeReceived(object msg)
    {
        if (msg is Welcome welcome)
        {
            id = welcome.Id;
        }
    }

    void Update()
    {
        if (id >= 0)
        {
            PlayerTransform playerTransform = new PlayerTransform();
            playerTransform.Id = id;

            Vector3 qrRelativePos = transform.localPosition - qrPos.localPosition;
            playerTransform.Position = new Point3D()
            {
                X = qrRelativePos.x,
                Y = qrRelativePos.y,
                Z = qrRelativePos.z
            };

            playerTransform.Rotation = new Point4D()
            {
                X = transform.localRotation.x,
                Y = transform.localRotation.y,
                Z = transform.localRotation.z,
                W = transform.localRotation.w
            };


            playerTransform.RightHandPosition = new Point3D()
            {
                X = qrRelativePos.x,
                Y = qrRelativePos.y,
                Z = qrRelativePos.z
            };

            Message msg = new Message();
            msg.Identifier = (int)MessageTypes.PlayerTransform;
            msg.Data = playerTransform.ToByteString();

            NetworkHandler.Processors[MessageTypes.PlayerTransform].AddOutMessage(msg); 
        }
    }
}
