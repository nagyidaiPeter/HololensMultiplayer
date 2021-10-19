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
    public class ServerObjectProcessor : BaseProcessor
    {
        public new Queue<ObjectTransformMsg> IncomingMessages { get; set; } = new Queue<ObjectTransformMsg>();

        public new Queue<ObjectTransformMsg> OutgoingMessages { get; set; } = new Queue<ObjectTransformMsg>();

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

            ObjectFB transformFB = ObjectFB.GetRootAsObjectFB(bb);
            ObjectTransformMsg objectTransform = new ObjectTransformMsg();

            objectTransform.SenderID = player.ID;
            objectTransform.OwnerID = transformFB.OwnerID;

            if (transformFB.Pos.HasValue)
                objectTransform.Pos = new Vector3(transformFB.Pos.Value.X, transformFB.Pos.Value.Y, transformFB.Pos.Value.Z);

            if (transformFB.Rot.HasValue)
                objectTransform.Rot = new Quaternion(transformFB.Rot.Value.X, transformFB.Rot.Value.Y, transformFB.Rot.Value.Z, transformFB.Rot.Value.W);

            if (transformFB.Scale.HasValue)
                objectTransform.Scale = new Vector3(transformFB.Scale.Value.X, transformFB.Scale.Value.Y, transformFB.Scale.Value.Z);

            IncomingMessages.Enqueue(objectTransform);
            return true;
        }

        public override void ProcessIncoming()
        {
            while (IncomingMessages.Any())
            {
                var transformMsg = IncomingMessages.Dequeue();
                var objectTransform = dataManager.Objects[transformMsg.ObjectID];
                objectTransform.Position = transformMsg.Pos;
                objectTransform.Rotation = transformMsg.Rot;
                objectTransform.Scale = transformMsg.Scale;
                objectTransform.OwnerID = transformMsg.OwnerID;

                OutgoingMessages.Enqueue(transformMsg);
            }
        }

        public override void ProcessOutgoing()
        {
            while (OutgoingMessages.Any())
            {
                var postMsg = OutgoingMessages.Dequeue();
            }

            for (int j = 0; j < dataManager.Objects.Count; j++)
            {
                var objectToSync = dataManager.Objects.ElementAt(j).Value;

                var msg = netServer.CreateMessage();
                ObjectTransformMsg ObjectTransform = new ObjectTransformMsg();
                ObjectTransform.Pos = objectToSync.Position;
                ObjectTransform.Rot = objectToSync.Rotation;
                ObjectTransform.Scale = objectToSync.Scale;
                ObjectTransform.OwnerID = objectToSync.OwnerID;
                ObjectTransform.ObjectID = objectToSync.ID;
                ObjectTransform.ObjectType = objectToSync.ObjectType;   


                var bytes = Serializer.SerializeObjectTransform(ObjectTransform);

                msg.Write((byte)MessageTypes.ObjectTransform);
                msg.Write(bytes.Length);
                msg.Write(bytes);

                netServer.SendToAll(msg, NetDeliveryMethod.UnreliableSequenced, 0);
            }
        }

    }
}
