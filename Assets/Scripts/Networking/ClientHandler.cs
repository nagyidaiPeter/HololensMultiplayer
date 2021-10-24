using Assets.Scripts.SERVER.Processors;

using FlatBuffers;

using hololensMultiplayer;
using hololensMultiplayer.Models;
using hololensMultiplayer.Networking;
using hololensMultiplayer.Packets;
using LiteNetLib;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Zenject;

public class ClientHandler : MonoBehaviour
{
    public bool ClientIsServer = false;

    public string Address = "127.0.0.1:12345";

    private Client client;
    private DataManager dataManager;

    //In MS
    public float UpdateTime = 0.01666f;

    public Dictionary<MessageTypes, IProcessor> MessageProcessors = new Dictionary<MessageTypes, IProcessor>();

    [Inject]
    public void Init(Client client, DataManager dataManager, ClientPlayTransProcessor clientPlayTransProcessor, ClientDisconnectProcessor clientDisconnect,
        ClientWelcomeProcessor clientWelcome, ClientObjectProcessor objectProcessor)
    {
        this.client = client;
        this.dataManager = dataManager;

        this.client.netPacketProcessor.SubscribeReusable<WrapperPacket, NetPeer>(OnPacketReceived);

        MessageProcessors.Add(MessageTypes.PlayerTransform, clientPlayTransProcessor);
        MessageProcessors.Add(MessageTypes.Welcome, clientWelcome);
        MessageProcessors.Add(MessageTypes.Disconnect, clientDisconnect);
        MessageProcessors.Add(MessageTypes.ObjectTransform, objectProcessor);
    }

    private void OnPacketReceived(WrapperPacket wrapperPacket, NetPeer peer)
    {
        MessageProcessors[wrapperPacket.messageType].AddInMessage(wrapperPacket.packetData, peer);
    }

    public bool IsConnected { get { return client.IsConnected; } }

    public void SetAddress(string address)
    {
        Address = address;
    }

    void Start()
    {

    }

    public void Connect()
    {
        var split = Address.Split(':');
        client.Start();
        client.Connect(split[0], int.Parse(split[1]), "hololensMultiplayer");        
        Debug.Log($"Connecting to {Address}");
        StartCoroutine(ClientUpdate());
    }

    public void Disconnect()
    {
        client.DisconnectAll();
        client.Stop();
        StopAllCoroutines();
    }

    private IEnumerator ClientUpdate()
    {
        while (true)
        {
            client.PollEvents();

            foreach (var handlerPair in MessageProcessors)
            {
                handlerPair.Value.ProcessIncoming();
                handlerPair.Value.ProcessOutgoing();
            }

            yield return new WaitForSeconds(UpdateTime);
        }

    }
    private void OnDestroy()
    {
        Disconnect();
    }
}
