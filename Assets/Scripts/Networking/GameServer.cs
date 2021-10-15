using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using Hrm;
using Google.Protobuf;
using System.Linq;

public class GameServer : MonoBehaviour, INetEventListener, INetLogger
{
    private NetManager _netServer;
    private NetDataWriter _dataWriter;

    void Start()
    {
        NetDebug.Logger = this;
    }

    public void StartServer()
    {
        _dataWriter = new NetDataWriter();
        _netServer = new NetManager(this);
        _netServer.Start(NetworkHandler.Instance._port);
        _netServer.BroadcastReceiveEnabled = true;
        _netServer.UpdateTime = 15;

        NetworkHandler.Instance.IsServer = true;

        FindObjectOfType<LocalPlayer>().id = 0;
    }

    void Update()
    {
        if (_netServer is NetManager)
        {
            _netServer.PollEvents();
        }
    }

    void FixedUpdate()
    {
        if (NetworkHandler.Instance.IsServer)
        {
            foreach (var proc in NetworkHandler.Processors.Values)
            {
                foreach (var msg in proc.ProcessOutgoing())
                {
                    _dataWriter.Reset();
                    _dataWriter.Put(msg.ToByteArray());
                    foreach (var peer in _netServer.ConnectedPeerList)
                    {
                        peer.Send(_dataWriter, DeliveryMethod.Unreliable);
                    }
                }
            }
        }
    }

    void OnDestroy()
    {
        try
        {
            NetDebug.Logger = null;
            if (_netServer != null)
                _netServer.Stop(true);
        }
        catch (Exception)
        {

        }
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[SERVER] We have new peer " + peer.EndPoint);               
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.Log("[SERVER] error " + socketErrorCode);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
        UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.Broadcast)
        {
            Debug.Log("[SERVER] Received discovery request. Send discovery response");

            int newPlayerId = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            Welcome welcome = new Welcome();
            welcome.Id = newPlayerId;

            Message msg = new Message();
            msg.Identifier = (int)MessageTypes.Welcome;
            msg.Data = welcome.ToByteString();

            NetDataWriter writer = new NetDataWriter();
            writer.Put(msg.ToByteArray());            

            _netServer.SendUnconnectedMessage(writer, remoteEndPoint);
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        request.AcceptIfKey(NetworkHandler.Instance.AppKey);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[SERVER] peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
        var disconnectedPlayer = NetworkHandler.Instance.Players.FirstOrDefault(x => x.Key == peer.Id);
        NetworkHandler.Instance.Players.Remove(disconnectedPlayer.Key);
        Destroy(disconnectedPlayer.Value.gameObject);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        byte[] message = null;

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
                //todo: error handling
            }
        }
    }

    public void WriteNet(NetLogLevel level, string str, params object[] args)
    {
        Debug.LogFormat(str, args);
    }
}
