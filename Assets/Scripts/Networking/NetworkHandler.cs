using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Assets.MsgProcessors;

using Hrm;

using HRM.Utility;

using LiteNetLib;
using LiteNetLib.Utils;

using UnityEngine;

public enum MessageTypes
{
    PlayerTransform,
    ObjectTransform,
    Welcome
}

public class NetworkHandler : Singleton<NetworkHandler>
{
    public static Dictionary<MessageTypes, IMsgProcessor> Processors = new Dictionary<MessageTypes, IMsgProcessor>()
    {
        { MessageTypes.PlayerTransform, new PlayerTransformProcessor() },
        { MessageTypes.ObjectTransform, new ObjectTransformProcessor() },
        { MessageTypes.Welcome, new WelcomeProcessor() }
    };

    public Dictionary<int, NetworkPlayer> Players = new Dictionary<int, NetworkPlayer>();


    public string AppKey = "HRM";
    public string IP = "192.168.11.124";
    public int _port = 5556;
    public bool IsServer = false;

    public void Start()
    {

    }


    void OnDisable()
    {

    }


    public void LateUpdate()
    {     
        foreach (IMsgProcessor proc in Processors.Values)
        {
            proc.ProcessIncoming();
        }
    }

}