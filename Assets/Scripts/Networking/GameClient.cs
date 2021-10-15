using System.Net;
using System.Net.Sockets;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Text;
using Hrm;
using System;
using Google.Protobuf;

public class GameClient : MonoBehaviour, INetEventListener
{
    public NetManager netClient;
    private NetPeer serverPeer;

    private NetDataWriter dataWriter = new NetDataWriter();
    void Start()
    {
        netClient = new NetManager(this);
        netClient.UnconnectedMessagesEnabled = true;
        netClient.UpdateTime = 15;
        netClient.Start();
    }

    void Update()
    {
        netClient.PollEvents();

        if (serverPeer != null && serverPeer.ConnectionState == ConnectionState.Connected)
        {
            if (!NetworkHandler.Instance.IsServer)
            {
                foreach (var proc in NetworkHandler.Processors.Values)
                {
                    foreach (var msg in proc.ProcessOutgoing())
                    {
                        dataWriter.Reset();
                        dataWriter.Put(msg.ToByteArray());
                        serverPeer.Send(dataWriter, DeliveryMethod.Unreliable);
                    }
                }
            }
        }
        else
        {
            netClient.SendBroadcast(new byte[] { 1 }, NetworkHandler.Instance._port);
        }
    }

    public void SetIp(string ip)
    {
        NetworkHandler.Instance.IP = ip;
    }

    public void ConnectToServer()
    {
        if (netClient.ConnectedPeersCount == 0)
        {
            netClient.Connect(NetworkHandler.Instance.IP, NetworkHandler.Instance._port, NetworkHandler.Instance.AppKey);
        }
    }

    void OnDestroy()
    {
        if (netClient != null)
            netClient.Stop();
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[CLIENT] We connected to " + peer.EndPoint);
        serverPeer = peer;
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.Log("[CLIENT] We received error " + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        byte[] message = new byte[0];

        if (reader != null)
        {
            try
            {
                message = new byte[reader.UserDataSize];
                reader.GetBytes(message, reader.UserDataSize);

                var msg = Message.Parser.ParseFrom(message);
                MessageTypes msgType = (MessageTypes)msg.Identifier;

                if (NetworkHandler.Processors.ContainsKey(msgType))
                {
                    NetworkHandler.Processors[msgType].AddInMessage(msg, peer);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Client ex:{ex}");
            }
        }
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.BasicMessage && netClient.ConnectedPeersCount == 0)
        {
            Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);

            byte[] message = new byte[0];
            message = new byte[reader.UserDataSize];
            reader.GetBytes(message, reader.UserDataSize);
            var msg = Message.Parser.ParseFrom(message);
            MessageTypes msgType = (MessageTypes)msg.Identifier;

            if (msgType == MessageTypes.Welcome)
            {
                NetworkHandler.Processors[MessageTypes.Welcome].AddInMessage(msg, null);
                netClient.Connect(remoteEndPoint, NetworkHandler.Instance.AppKey);
            }
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {

    }

    public void OnConnectionRequest(ConnectionRequest request)
    {

    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[CLIENT] We disconnected because " + disconnectInfo.Reason);
    }
}
