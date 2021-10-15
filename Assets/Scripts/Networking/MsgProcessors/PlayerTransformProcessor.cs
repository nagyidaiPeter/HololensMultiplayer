using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Hrm;

using LiteNetLib;
using LiteNetLib.Utils;

using UnityEngine;

namespace Assets.MsgProcessors
{
    public class PlayerTransformProcessor : BaseProcessor<PlayerTransform>
    {
        public override event ReceivedMessage ReceivedMessageEvent;

        public override void ProcessIncoming()
        {
            while (IncomingMessageQueue.Any())
            {
                try
                {
                    var msg = IncomingMessageQueue.Dequeue();

                    if (msg is Message message)
                    {
                        var playerTransform = PlayerTransform.Parser.ParseFrom(message.Data);                        
                        ReceivedMessageEvent?.Invoke(playerTransform);
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

    }
}
