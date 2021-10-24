using Assets.Scripts.SERVER.Processors;
using hololensMultiplayer.Models;
using hololensMultiplayer.Networking;
using hololensMultiplayer.Packets;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace hololensMultiplayer
{
    public class Client : NetManager
    {
        public readonly NetPacketProcessor netPacketProcessor = new NetPacketProcessor();
        public readonly EventBasedNetListener listener;

        public bool IsConnected { get; private set; }

        public Client(EventBasedNetListener listener) : base(listener)
        {
            this.listener = listener;
        }

        public new void Stop()
        {
            listener.PeerConnectedEvent -= PeerConnected;
            listener.PeerDisconnectedEvent -= PeerDisconnected;
            listener.NetworkReceiveEvent -= NetworkDataReceived;

            base.Stop();
        }

        public new void Start()
        {
            listener.PeerConnectedEvent += PeerConnected;
            listener.PeerDisconnectedEvent += PeerDisconnected;
            listener.NetworkReceiveEvent += NetworkDataReceived;

            base.Start();
        }

        private void NetworkDataReceived(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            netPacketProcessor.ReadAllPackets(reader, peer);
        }

        private void PeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("[CLIENT] We disconnected because " + disconnectInfo.Reason);
            IsConnected = false;
        }

        private void PeerConnected(NetPeer peer)
        {
            Debug.Log("[CLIENT] We connected to " + peer.EndPoint);
            IsConnected = true;
        }

        public void Send(WrapperPacket wrapperPacket)
        {
           FirstPeer.Send(netPacketProcessor.Write(wrapperPacket), (byte)wrapperPacket.UdpChannel, wrapperPacket.deliveryMethod);
        }
    }
}