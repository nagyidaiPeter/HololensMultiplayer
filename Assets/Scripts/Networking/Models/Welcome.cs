using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace hololensMultiplayer.Models
{
    public class Welcome : BaseMessageType
    {
        public string Name { get; set; } = "Player";

        public new MessageTypes MsgType = MessageTypes.Welcome;
    }
}
