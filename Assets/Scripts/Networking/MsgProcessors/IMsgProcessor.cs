using Hrm;

using LiteNetLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.MsgProcessors
{
    public delegate void ReceivedMessage(Object msg);

    public interface IMsgProcessor
    {
        event ReceivedMessage ReceivedMessageEvent;

        void AddInMessage(Message newMsg, NetPeer sender);

        void AddOutMessage(Message newMsg);

        void ProcessIncoming();

        IEnumerable<Message> ProcessOutgoing();
    }
}
