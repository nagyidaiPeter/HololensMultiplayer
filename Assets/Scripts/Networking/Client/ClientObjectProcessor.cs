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
    public class ClientObjectProcessor : BaseProcessor
    {
        public new Queue<ObjectTransformMsg> IncomingMessages { get; set; } = new Queue<ObjectTransformMsg>();

        public new Queue<ObjectTransformMsg> OutgoingMessages { get; set; } = new Queue<ObjectTransformMsg>();

        [Inject]
        private NetClient netClient;

        [Inject]
        private DataManager dataManager;

        [Inject]
        private NetworkObject.ObjectFactory objectFactory;

        public override bool AddInMessage(byte[] message, PlayerData player)
        {
            ByteBuffer bb = new ByteBuffer(message);
            ObjectFB objectFB = ObjectFB.GetRootAsObjectFB(bb);

            ObjectTransformMsg objectTransform = new ObjectTransformMsg();

            objectTransform.ObjectID = objectFB.ObjectID;
            objectTransform.OwnerID = objectFB.OwnerID;

            if (objectFB.Pos.HasValue)
                objectTransform.Pos = new Vector3(objectFB.Pos.Value.X, objectFB.Pos.Value.Y, objectFB.Pos.Value.Z);

            if (objectFB.Rot.HasValue)
                objectTransform.Rot = new Quaternion(objectFB.Rot.Value.X, objectFB.Rot.Value.Y, objectFB.Rot.Value.Z, objectFB.Rot.Value.W);

            if (objectFB.Scale.HasValue)
                objectTransform.Scale = new Vector3(objectFB.Scale.Value.X, objectFB.Scale.Value.Y, objectFB.Scale.Value.Z);


            IncomingMessages.Enqueue(objectTransform);
            return true;
        }

        public override bool AddOutMessage(BaseMessageType objectToSend)
        {
            if (objectToSend is ObjectTransformMsg ObjectTransform)
            {
                OutgoingMessages.Enqueue(ObjectTransform);
                return true;
            }

            return false;
        }

        public override void ProcessIncoming()
        {
            while (IncomingMessages.Any())
            {
                var objectTransformMsg = IncomingMessages.Dequeue();
                if (dataManager.Objects.ContainsKey(objectTransformMsg.ObjectID))
                {
                    var objectTransform = dataManager.Objects[objectTransformMsg.ObjectID];
                    objectTransform.OwnerID = objectTransformMsg.OwnerID;
                    objectTransform.Position = objectTransformMsg.Pos;
                    objectTransform.Rotation = objectTransformMsg.Rot;
                    objectTransform.Scale = objectTransformMsg.Scale;
                }
                else
                {
                    ObjectData newObject = new ObjectData();
                    
                    var networkPlayer = objectFactory.Create(Resources.Load($"Objects/{objectTransformMsg.ObjectType}"));
                    var newPlayerGO = networkPlayer.gameObject;
                    newPlayerGO.SetActive(true);
                    newPlayerGO.transform.parent = GameObject.Find("NetworkSpace").transform;
                    newObject.gameObject = newPlayerGO;
                    networkPlayer.objectData = newObject;
                    dataManager.Objects.Add(objectTransformMsg.ObjectID, newObject);
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
                posMsg.OwnerID = dataManager.LocalPlayer.ID;

                var bytes = Serializer.SerializeObjectTransform(posMsg);

                msg.Write((byte)MessageTypes.ObjectTransform);
                msg.Write(bytes.Length);
                msg.Write(bytes);

                netClient.SendMessage(msg, NetDeliveryMethod.UnreliableSequenced, 0);
            }
        }

    }
}
