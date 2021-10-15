using FlatBuffers;
using Lidgren.Network;
using Newtonsoft.Json;
using hololensMultiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using hololensMultiplayer.Networking;
using hololensMultiplayer.Models;
using hololensMulti;

namespace Assets.Scripts.SERVER.Processors
{
    public class ClientPlayTransProcessor : BaseProcessor
    {
        public new Queue<PlayerTransform> IncomingMessages { get; set; } = new Queue<PlayerTransform>();

        public new Queue<PlayerTransform> OutgoingMessages { get; set; } = new Queue<PlayerTransform>();

        [Inject]
        private NetClient netClient;

        [Inject]
        private DataManager dataManager;

        [Inject]
        private NetworkPlayer.Factory playerFactory;

        public override bool AddInMessage(byte[] message, PlayerData player)
        {
            ByteBuffer bb = new ByteBuffer(message);

            TransformFB transformFB = TransformFB.GetRootAsTransformFB(bb);

            PlayerTransform playerTransform = new PlayerTransform();

            playerTransform.SenderID = transformFB.PlayerID;

            if (transformFB.Pos.HasValue)
                playerTransform.Pos = new Vector3(transformFB.Pos.Value.X, transformFB.Pos.Value.Y, transformFB.Pos.Value.Z);

            if (transformFB.Rot.HasValue)
                playerTransform.Rot = new Quaternion(transformFB.Rot.Value.X, transformFB.Rot.Value.Y, transformFB.Rot.Value.Z, transformFB.Rot.Value.W);

            if (transformFB.RHPos.HasValue)
                playerTransform.RHPos = new Vector3(transformFB.RHPos.Value.X, transformFB.RHPos.Value.Y, transformFB.RHPos.Value.Z);

            if (transformFB.RHRot.HasValue)
                playerTransform.RHRot = new Quaternion(transformFB.RHRot.Value.X, transformFB.RHRot.Value.Y, transformFB.RHRot.Value.Z, transformFB.RHRot.Value.W);

            if (transformFB.LHPos.HasValue)
                playerTransform.LHPos = new Vector3(transformFB.LHPos.Value.X, transformFB.LHPos.Value.Y, transformFB.LHPos.Value.Z);

            if (transformFB.LHRot.HasValue)
                playerTransform.LHRot = new Quaternion(transformFB.LHRot.Value.X, transformFB.LHRot.Value.Y, transformFB.LHRot.Value.Z, transformFB.LHRot.Value.W);

            IncomingMessages.Enqueue(playerTransform);
            return true;
        }

        public override bool AddOutMessage(BaseMessageType objectToSend)
        {
            if (objectToSend is PlayerTransform playerTransform)
            {
                OutgoingMessages.Enqueue(playerTransform);
                return true;
            }

            return false;
        }

        public override void ProcessIncoming()
        {
            while (IncomingMessages.Any())
            {
                var transformMsg = IncomingMessages.Dequeue();
                if (dataManager.Players.ContainsKey(transformMsg.SenderID) && dataManager.Players[transformMsg.SenderID].playerObject != null)
                {
                    var player = dataManager.Players[transformMsg.SenderID];
                    player.Position = transformMsg.Pos;
                    player.Rotation = transformMsg.Rot;

                    player.RHPosition = transformMsg.RHPos;
                    player.RHRotation = transformMsg.RHRot;

                    player.LHPosition = transformMsg.LHPos;
                    player.LHRotation = transformMsg.LHRot;
                }
                else
                {
                    if (!dataManager.Players.ContainsKey(transformMsg.SenderID))
                    {
                        PlayerData newPlayer = new PlayerData();
                        newPlayer.ID = transformMsg.SenderID;

                        var networkPlayer = playerFactory.Create("PlayerHead");
                        var newPlayerGO = networkPlayer.gameObject;
                        newPlayerGO.SetActive(true);
                        newPlayerGO.transform.parent = GameObject.Find("NetworkSpace").transform;
                        newPlayer.playerObject = newPlayerGO;
                        networkPlayer.playerData = newPlayer;
                        dataManager.Players.Add(newPlayer.ID, newPlayer);
                    }
                    else
                    {
                        PlayerData newPlayer = dataManager.Players[transformMsg.SenderID];

                        var networkPlayer = playerFactory.Create("PlayerHead");
                        var newPlayerGO = networkPlayer.gameObject;
                        newPlayerGO.SetActive(true);
                        newPlayerGO.transform.parent = GameObject.Find("NetworkSpace").transform;
                        newPlayer.playerObject = newPlayerGO;
                        networkPlayer.playerData = newPlayer;
                        dataManager.Players[newPlayer.ID] = newPlayer;
                    }
                 
                }
            }
        }

        public override void ProcessOutgoing()
        {
            while (OutgoingMessages.Any())
            {
                var posMsg = OutgoingMessages.Dequeue();

                var msg = netClient.CreateMessage();

                posMsg.SenderID = dataManager.LocalPlayer.ID;

                var bytes = Serializer.SerializePlayerTransform(posMsg);

                msg.Write((byte)MessageTypes.PlayerTransform);
                msg.Write(bytes.Length);
                msg.Write(bytes);

                netClient.SendMessage(msg, NetDeliveryMethod.UnreliableSequenced, 0);
            }
        }

    }
}
