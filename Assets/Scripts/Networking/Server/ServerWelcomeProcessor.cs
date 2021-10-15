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
using FlatBuffers;

namespace Assets.Scripts.SERVER.Processors
{
    public class ServerWelcomeProcessor : BaseProcessor
    {
        public new Queue<Welcome> IncomingMessages { get; set; } = new Queue<Welcome>();
        public new Queue<Welcome> OutgoingMessages { get; set; } = new Queue<Welcome>();

        [Inject]
        private NetServer netServer;

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
                var player = dataManager.GetPlayerById(welcomeMsg.SenderID);
                player.Name = welcomeMsg.Name;
                Debug.Log($"Client answered: {player.Name}");
            }
        }

        public override void ProcessOutgoing()
        {
            while (OutgoingMessages.Any())
            {
                //var msg = netServer.CreateMessage();
                var welcomeMsg = OutgoingMessages.Dequeue();

                //Inform others about new player
                //PlayerInfo playerInfo = new PlayerInfo();
                //playerInfo.SenderID = welcomeMsg.SenderID;
                //playerInfo.Name = welcomeMsg.Name;

                //var body = JsonConvert.SerializeObject(playerInfo);

                //msg.Write((byte)MessageTypes.PlayerInfo);
                //msg.Write(body.Length);
                //msg.Write(body);

                //netServer.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
            }
        }
    }
}
