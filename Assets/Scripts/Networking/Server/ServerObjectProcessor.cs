using FlatBuffers;

using hololensMultiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using hololensMultiplayer.Networking;
using hololensMultiplayer.Models;
using hololensMulti;
using LiteNetLib;

namespace Assets.Scripts.SERVER.Processors
{
    public class ServerObjectProcessor : BaseProcessor
    {
        public new Queue<ObjectTransform> IncomingMessages { get; set; } = new Queue<ObjectTransform>();

        public new Queue<ObjectTransform> OutgoingMessages { get; set; } = new Queue<ObjectTransform>();

        [Inject]
        private Server server;

        public override bool AddOutMessage(BaseMessageType objectToSend)
        {
            throw new NotImplementedException();
        }

        public override bool AddInMessage(byte[] message, NetPeer player)
        {
            ObjectTransform objectTransform = new ObjectTransform(message);           
            IncomingMessages.Enqueue(objectTransform);
            return true;
        }

        public override void ProcessIncoming()
        {
            while (IncomingMessages.Any())
            {
                var transformMsg = IncomingMessages.Dequeue();
                if (dataManager.Objects.ContainsKey(transformMsg.ObjectID))
                {
                    var objectTransform = dataManager.Objects[transformMsg.ObjectID];
                    objectTransform.objectTransform = transformMsg;
                }
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
                server.SendToAll(objectToSync.objectTransform.Serialize());
            }
        }

    }
}
