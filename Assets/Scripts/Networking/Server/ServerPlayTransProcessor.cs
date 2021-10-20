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
    public class ServerPlayTransProcessor : BaseProcessor
    {
        public new Queue<PlayerTransform> IncomingMessages { get; set; } = new Queue<PlayerTransform>();

        public new Queue<PlayerTransform> OutgoingMessages { get; set; } = new Queue<PlayerTransform>();

        [Inject]
        private NetServer netServer;

        [Inject]
        private DataManager dataManager;
        public override bool AddOutMessage(BaseMessageType objectToSend)
        {
            throw new NotImplementedException();
        }

        public override bool AddInMessage(byte[] message, PlayerData player)
        {
            ByteBuffer bb = new ByteBuffer(message);

            TransformFB transformFB = TransformFB.GetRootAsTransformFB(bb);
            PlayerTransform playerTransform = new PlayerTransform();

            playerTransform.SenderID = player.ID;

            if (transformFB.Pos.HasValue)
                playerTransform.Pos = new Vector3(transformFB.Pos.Value.X, transformFB.Pos.Value.Y, transformFB.Pos.Value.Z);

            if (transformFB.Rot.HasValue)
                playerTransform.Rot = new Quaternion(transformFB.Rot.Value.X, transformFB.Rot.Value.Y, transformFB.Rot.Value.Z, transformFB.Rot.Value.W);

            if (transformFB.QrOffset.HasValue)
                playerTransform.QrOffset = new Vector3(transformFB.QrOffset.Value.X, transformFB.QrOffset.Value.Y, transformFB.QrOffset.Value.Z);

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

        public override void ProcessIncoming()
        {
            while (IncomingMessages.Any())
            {
                var transformMsg = IncomingMessages.Dequeue();
                var player = dataManager.Players[transformMsg.SenderID];
                player.Position = transformMsg.Pos;
                player.Rotation = transformMsg.Rot;
                player.QrOffset = transformMsg.QrOffset;

                player.RHPosition = transformMsg.RHPos;
                player.RHRotation = transformMsg.RHRot;

                player.LHPosition = transformMsg.LHPos;
                player.LHRotation = transformMsg.LHRot;

                OutgoingMessages.Enqueue(transformMsg);
            }
        }

        public override void ProcessOutgoing()
        {
            while (OutgoingMessages.Any())
            {
                var postMsg = OutgoingMessages.Dequeue();
            }

            for (int j = 0; j < dataManager.Players.Count; j++)
            {
                var otherPlayer = dataManager.Players.ElementAt(j).Value;

                var msg = netServer.CreateMessage();
                PlayerTransform playerTransform = new PlayerTransform();
                playerTransform.Pos = otherPlayer.Position;
                playerTransform.Rot = otherPlayer.Rotation;
                playerTransform.QrOffset = otherPlayer.QrOffset;

                playerTransform.RHPos = otherPlayer.RHPosition;
                playerTransform.RHRot = otherPlayer.RHRotation;

                playerTransform.LHPos = otherPlayer.LHPosition;
                playerTransform.LHRot = otherPlayer.LHRotation;

                playerTransform.SenderID = otherPlayer.ID;

                var bytes = Serializer.SerializePlayerTransform(playerTransform);

                msg.Write((byte)MessageTypes.PlayerTransform);
                msg.Write(bytes.Length);
                msg.Write(bytes);

                netServer.SendToAll(msg, NetDeliveryMethod.UnreliableSequenced, 0);
            }
        }

    }
}
