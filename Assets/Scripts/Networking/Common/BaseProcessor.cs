using hololensMultiplayer.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hololensMultiplayer.Networking
{
    public abstract class BaseProcessor : IProcessor
    {
        public Queue<BaseMessageType> IncomingMessages { get; set; } = new Queue<BaseMessageType>();
        public Queue<BaseMessageType> OutgoingMessages { get; set; } = new Queue<BaseMessageType>();

        public abstract bool AddInMessage(byte[] message, PlayerData player);

        public abstract bool AddOutMessage(BaseMessageType objectToSend);

        public abstract void ProcessIncoming();

        public abstract void ProcessOutgoing();
    }
}
