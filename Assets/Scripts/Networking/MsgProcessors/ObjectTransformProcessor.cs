using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Hrm;

namespace Assets.MsgProcessors
{
    public class ObjectTransformProcessor : BaseProcessor<ObjectTransform>
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
                        var objectTransform = ObjectTransform.Parser.ParseFrom(message.Data);
                        ReceivedMessageEvent?.Invoke(objectTransform);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
