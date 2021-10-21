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
using FlatBuffers;
using hololensMulti;

namespace Assets.Scripts.SERVER.Processors
{
    public class ClientDisconnectProcessor : BaseProcessor
    {
        public new Queue<DisconnectMessage> IncomingMessages { get; set; } = new Queue<DisconnectMessage>();
        public new Queue<DisconnectMessage> OutgoingMessages { get; set; } = new Queue<DisconnectMessage>();

        [Inject]
        private NetServer netServer;

        [Inject]
        private DataManager dataManager;

        public override void ProcessIncoming()
        {
            while (IncomingMessages.Any())
            {
                var dcMsg = IncomingMessages.Dequeue();
                Debug.Log($"Player {dcMsg.DisconnectedUserID} disconnected..");
                var player = dataManager.GetPlayerById(dcMsg.DisconnectedUserID);

                dataManager.Players.Remove(player.ID);
                GameObject.Destroy(player.playerObject);
            }
        }

        public override void ProcessOutgoing()
        {
            while (OutgoingMessages.Any())
            {
                var msg = netServer.CreateMessage();
                var registerMsg = OutgoingMessages.Dequeue();

                var body = JsonConvert.SerializeObject(registerMsg);
                msg.Write((byte)MessageTypes.Disconnect);
                msg.Write(body.Length);
                msg.Write(body);

                netServer.SendToAll(msg, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        public override bool AddInMessage(byte[] message, PlayerData player)
        {
            ByteBuffer bb = new ByteBuffer(message);
            DisconnectFB disconnectFB = DisconnectFB.GetRootAsDisconnectFB(bb);
            DisconnectMessage dcMsg = new DisconnectMessage();

            dcMsg.SenderID = disconnectFB.PlayerID;
            dcMsg.DisconnectedUserID = disconnectFB.PlayerID;

            IncomingMessages.Enqueue(dcMsg);
            return true;
        }

        public override bool AddOutMessage(BaseMessageType objectToSend)
        {
            return true;
        }
    }
}
