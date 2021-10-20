using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hololensMultiplayer.Models
{
    public class DisconnectMessage : BaseMessageType
    {
        public int DisconnectedUserID { get; set; }

        public new MessageTypes MsgType = MessageTypes.Disconnect;

    }

}
