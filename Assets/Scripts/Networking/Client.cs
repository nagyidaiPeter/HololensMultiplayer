using Assets.Scripts.SERVER;
using Assets.Scripts.SERVER.Processors;

using FlatBuffers;

using hololensMultiplayer;
using hololensMultiplayer.Models;
using hololensMultiplayer.Networking;

using Lidgren.Network;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class Client : MonoBehaviour
{
    public bool ClientIsServer = false;

    public float Interpol = 0.5f;
    public string Address = "127.0.0.1:12345";

    private NetClient client;
    private DataManager dataManager;

    //In MS
    public float UpdateTime = 0.01666f;

    public Dictionary<MessageTypes, IProcessor> MessageProcessors = new Dictionary<MessageTypes, IProcessor>();

    [Inject]
    public void Init(NetClient client, DataManager dataManager, ClientPlayTransProcessor clientPlayTransProcessor, ClientDisconnectProcessor clientDisconnect,
        ClientWelcomeProcessor clientWelcome)
    {
        this.client = client;
        this.dataManager = dataManager;

        MessageProcessors.Add(MessageTypes.PlayerTransform, clientPlayTransProcessor);
        MessageProcessors.Add(MessageTypes.Welcome, clientWelcome);
        MessageProcessors.Add(MessageTypes.Disconnect, clientDisconnect);
    }

    public void SetAddress(string address)
    {
        Address = address;
    }

    void Start()
    {
        StartCoroutine(ClientUpdate());
    }

    public void Connect()
    {
        var split = Address.Split(':');
        client.Connect(host: split[0], port: int.Parse(split[1]));
    }

    public void Disconnect()
    {
        client.Disconnect("exit");
        StopAllCoroutines();
    }

    private IEnumerator ClientUpdate()
    {
        while (true)
        {
            NetIncomingMessage message;
            while ((message = client.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        byte[] header = message.ReadBytes(5);
                        ByteBuffer headerBuffer = new ByteBuffer(header);
                        MessageTypes msgType = (MessageTypes)header[0];
                        int msgLenth = headerBuffer.GetInt(1);

                        byte[] content = message.ReadBytes(msgLenth);
                        MessageProcessors[msgType].AddInMessage(content, null);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        Debug.Log($"Connection status: {message.SenderConnection.Status} Data: {message.ReadString()}");
                        break;
                }
            }


            foreach (var handlerPair in MessageProcessors)
            {
                handlerPair.Value.ProcessIncoming();
                handlerPair.Value.ProcessOutgoing();
            }

            yield return new WaitForSeconds(UpdateTime);
        }

    }
    private void OnDestroy()
    {
        Disconnect();
    }
}
