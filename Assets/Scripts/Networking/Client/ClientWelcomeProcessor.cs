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
    public class ClientWelcomeProcessor : BaseProcessor
    {
        public new Queue<Welcome> IncomingMessages { get; set; } = new Queue<Welcome>();
        public new Queue<Welcome> OutgoingMessages { get; set; } = new Queue<Welcome>();

        [Inject]
        private NetClient netClient;

        [Inject]
        private DataManager dataManager;


        public override bool AddInMessage(byte[] message, PlayerData player)
        {
            ByteBuffer bb = new ByteBuffer(message);
            WelcomeFB welcomeFB = WelcomeFB.GetRootAsWelcomeFB(bb);
            Welcome welcome = new Welcome();

            welcome.SenderID = welcomeFB.PlayerID;
            welcome.Name = welcomeFB.PlayerName;

            IncomingMessages.Enqueue(welcome);
            return true;
        }
        public override bool AddOutMessage(BaseMessageType objectToSend)
        {
            if (objectToSend is Welcome welcomeMsg)
            {
                OutgoingMessages.Enqueue(welcomeMsg);
                return true;
            }
            return false;
        }

        public override void ProcessIncoming()
        {
            while (IncomingMessages.Any())
            {
                var welcomeMsg = IncomingMessages.Dequeue();

                dataManager.LocalPlayer.ID = welcomeMsg.SenderID;

                OutgoingMessages.Enqueue(welcomeMsg);
            }
        }

        public override void ProcessOutgoing()
        {
            while (OutgoingMessages.Any())
            {
                var msg = netClient.CreateMessage();
                var welcomeMsg = OutgoingMessages.Dequeue();

                var body = Serializer.SerializeWelcome(welcomeMsg);

                msg.Write((byte)MessageTypes.Welcome);
                msg.Write(body.Length);
                msg.Write(body);

                netClient.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
            }
        }

    }
}
