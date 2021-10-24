
using Assets.Scripts.SERVER.Processors;
using hololensMultiplayer;
using hololensMultiplayer.Models;
using hololensMultiplayer.Networking;
using hololensMultiplayer.Packets;
using LiteNetLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class ServerHandler : MonoBehaviour
{
    private Server server;

    public bool RunningServer = false;

    public string Address = "127.0.0.1:12345";

    //In SEC
    public float UpdateTime = 0.016666f;

    private DataManager dataManager;

    private Dictionary<MessageTypes, IProcessor> MessageProcessors = new Dictionary<MessageTypes, IProcessor>();


    [Inject]
    public void Init(Server server, DataManager dataManager,
            ServerPlayTransProcessor playerTransformProc, ServerWelcomeProcessor serverWelcomeProcessor,
            ServerDisconnectProcessor disconnectProcessor, ServerObjectProcessor objectProcessor)
    {
        this.server = server;
        this.dataManager = dataManager;

        MessageProcessors.Add(MessageTypes.PlayerTransform, playerTransformProc);
        MessageProcessors.Add(MessageTypes.Welcome, serverWelcomeProcessor);
        MessageProcessors.Add(MessageTypes.Disconnect, disconnectProcessor);
        MessageProcessors.Add(MessageTypes.ObjectTransform, objectProcessor);

        this.server.netPacketProcessor.SubscribeReusable<WrapperPacket, NetPeer>(OnPacketReceived);
    }

    private void OnPacketReceived(WrapperPacket wrapperPacket, NetPeer peer)
    {
        MessageProcessors[wrapperPacket.messageType].AddInMessage(wrapperPacket.packetData, peer);
    }

    public void StartServer()
    {
        Debug.Log("Starting server..");
        server.listener.PeerDisconnectedEvent += PeerDisconnected;
        server.listener.ConnectionRequestEvent += OnConnectionRequest;

        var split = Address.Split(':');
        server.Start(int.Parse(split[1]));        
        RunningServer = true;
        dataManager.IsServer = true;

        foreach (var obj in Resources.LoadAll("Objects"))
        {
            GetComponent<ObjectSpawner>().SpawnObject(obj.name);
        }

        //GetComponent<ObjectSpawner>().SpawnObject("HumanHeart");

        StartCoroutine(ServerUpdate());       
    }

    private void OnDestroy()
    {
        StopServer();
    }
    public void StopServer()
    {
        Debug.Log("Stopping server..");
        RunningServer = false;
        dataManager.IsServer = false;
        StopAllCoroutines();
        server.Stop();

        server.listener.PeerDisconnectedEvent -= PeerDisconnected;
        server.listener.ConnectionRequestEvent -= OnConnectionRequest;
    }

    private void OnConnectionRequest(ConnectionRequest request)
    {
        if (server.ConnectedPeersCount < server.maxConnections)
        {
            Debug.Log("New peer wants to connect!");
            request.AcceptIfKey("hololensMultiplayer");
        }
        else
        {
            request.Reject();
        }
    }

    private void PeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        DisconnectMessage disconnectMessage = new DisconnectMessage();
        disconnectMessage.DisconnectedUserID = peer.Id;
        MessageProcessors[MessageTypes.Disconnect].AddOutMessage(disconnectMessage);
    }

    private IEnumerator ServerUpdate()
    {
        while (RunningServer)
        {
            server.PollEvents();
            foreach (var processor in MessageProcessors)
            {
                try
                {
                    processor.Value.ProcessIncoming();
                    processor.Value.ProcessOutgoing();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex);
                }
            }


            yield return new WaitForSeconds(UpdateTime);
        }
    }

}