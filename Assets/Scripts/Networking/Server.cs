using Assets.Scripts.SERVER;
using Assets.Scripts.SERVER.Processors;

using FlatBuffers;

using hololensMultiplayer.Models;
using hololensMultiplayer.Networking;

using Lidgren.Network;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

namespace hololensMultiplayer
{
    public class Server
    {
        private NetServer server;
        private int ConnectedPlayers = 0;
        Dictionary<MessageTypes, IProcessor> MessageProcessors = new Dictionary<MessageTypes, IProcessor>();

        private DataManager dataManager;

        public Server(NetServer serverPeer, ServerPlayTransProcessor playerTransformProc, ServerWelcomeProcessor serverWelcomeProcessor,
            ServerDisconnectProcessor disconnectProcessor, ServerObjectProcessor objectProcessor, DataManager dataMan)
        {
            server = serverPeer;
            dataManager = dataMan;

            MessageProcessors.Add(MessageTypes.PlayerTransform, playerTransformProc);
            MessageProcessors.Add(MessageTypes.Welcome, serverWelcomeProcessor);
            MessageProcessors.Add(MessageTypes.Disconnect, disconnectProcessor);
            MessageProcessors.Add(MessageTypes.ObjectTransform, objectProcessor);
        }

        internal void StoptServer()
        {
            server.Shutdown("bye");
        }

        public void StartServer()
        {
            server.Start();
        }

        public void Reactions()
        {
            foreach (var processor in MessageProcessors)
            {
                processor.Value.ProcessOutgoing();
            }
        }

        public void ReadAndProcess()
        {
            NetIncomingMessage message;
            while ((message = server.ReadMessage()) != null)
            {
                PlayerData player =
                    dataManager.Players.FirstOrDefault(x => x.Value.connection == message.SenderConnection).Value;
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        if (player != null)
                        {
                            byte[] header = message.ReadBytes(5);
                            ByteBuffer headerBuffer = new ByteBuffer(header);
                            MessageTypes msgType = (MessageTypes)header[0];
                            int msgLenth = headerBuffer.GetInt(1);

                            byte[] content = message.ReadBytes(msgLenth);

                            MessageProcessors[msgType].AddInMessage(content, player);
                        }
                        break;

                    case NetIncomingMessageType.StatusChanged:

                        switch (message.SenderConnection.Status)
                        {
                            case NetConnectionStatus.None:
                                break;
                            case NetConnectionStatus.InitiatedConnect:
                                break;
                            case NetConnectionStatus.ReceivedInitiation:
                                break;
                            case NetConnectionStatus.RespondedAwaitingApproval:
                                break;
                            case NetConnectionStatus.RespondedConnect:
                                break;
                            case NetConnectionStatus.Connected:
                                ConnectedPlayers++;
                                var newPlayer = new PlayerData();
                                newPlayer.connection = message.SenderConnection;
                                newPlayer.ID = ConnectedPlayers;

                                dataManager.Players.Add(newPlayer.ID, newPlayer);

                                //Welcome player
                                Welcome welcomeMsg = new Welcome();
                                welcomeMsg.SenderID = ConnectedPlayers;
                                welcomeMsg.Name = newPlayer.Name;

                                var msgBody = Serializer.SerializeWelcome(welcomeMsg);

                                var msg = server.CreateMessage();
                                msg.Write((sbyte)MessageTypes.Welcome);
                                msg.Write(msgBody.Length);
                                msg.Write(msgBody);
                                newPlayer.connection.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
                                break;
                            case NetConnectionStatus.Disconnecting:
                                break;
                            case NetConnectionStatus.Disconnected:
                                if (player != null)
                                {
                                    DisconnectMessage disconnectMessage = new DisconnectMessage();
                                    disconnectMessage.DisconnectedUserID = player.ID;
                                    Debug.Log($"Dc happened: {player.ID} from players: {dataManager.Players.Count()}");
                                    MessageProcessors[MessageTypes.Disconnect].AddOutMessage(disconnectMessage);
                                }
                                break;
                        }
                        break;
                }
            }

            foreach (var processor in MessageProcessors)
            {
                processor.Value.ProcessIncoming();
            }
        }
    }
}
