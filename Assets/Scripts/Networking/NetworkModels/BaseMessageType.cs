using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hololensMultiplayer.Models
{

    public enum MessageTypes : byte
    {
        PlayerTransform,
        Welcome,
        PlayerInfo,
        Disconnect
    }

    public class BaseMessageType
    {
        public MessageTypes MsgType { get; set; }
        public int SenderID { get; set; }
    }
}
