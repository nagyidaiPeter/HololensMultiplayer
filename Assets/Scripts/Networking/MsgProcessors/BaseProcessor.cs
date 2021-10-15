using Hrm;

using LiteNetLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.MsgProcessors
{
    public abstract class BaseProcessor<T> : IMsgProcessor
    {
        public Queue<Message> IncomingMessageQueue = new Queue<Message>();

        public Queue<Message> OutgoingMessageQueue = new Queue<Message>();

        public virtual event ReceivedMessage ReceivedMessageEvent;

        public virtual void AddInMessage(Message newMsg, NetPeer sender)
        {
            IncomingMessageQueue.Enqueue(newMsg);
        }

        public virtual void AddOutMessage(Message newMsg)
        {
            OutgoingMessageQueue.Enqueue(newMsg);
        }

        public abstract void ProcessIncoming();

        public virtual IEnumerable<Message> ProcessOutgoing()
        {
            while (OutgoingMessageQueue.Any())
            {
                yield return OutgoingMessageQueue.Dequeue();
            }
        }
    }
}
