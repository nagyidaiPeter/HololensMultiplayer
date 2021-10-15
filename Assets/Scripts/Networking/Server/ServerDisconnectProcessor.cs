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

namespace Assets.Scripts.SERVER.Processors
{
    public class ServerDisconnectProcessor : BaseProcessor
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
                var player = dataManager.GetPlayerById(dcMsg.SenderID);

                dataManager.Players.Remove(player.ID);
                Transform.Destroy(player.playerObject);
                OutgoingMessages.Enqueue(dcMsg);
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
            throw new NotImplementedException();
        }

        public override bool AddOutMessage(BaseMessageType objectToSend)
        {
            throw new NotImplementedException();
        }
    }
}
