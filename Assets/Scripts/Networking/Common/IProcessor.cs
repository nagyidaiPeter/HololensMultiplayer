using Lidgren.Network;
using hololensMultiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using hololensMultiplayer.Models;

namespace hololensMultiplayer.Networking
{
    public interface IProcessor
    {
        Queue<BaseMessageType> IncomingMessages { get; set; }

        Queue<BaseMessageType> OutgoingMessages { get; set; }

        bool AddInMessage(byte[] message, PlayerData player);

        bool AddOutMessage(BaseMessageType objectToSend);

        void ProcessIncoming();

        void ProcessOutgoing();
    }
}
