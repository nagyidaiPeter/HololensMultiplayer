using Assets.Scripts.SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace hololensMultiplayer.Models
{
    public class PlayerInfo : BaseMessageType
    {
        public string Name { get; set; }

        public int PlayerID { get; set; }

        public int WeaponID { get; set; }

        public string ConnectionGUID { get; set; }

        public new MessageTypes MsgType = MessageTypes.PlayerInfo;

        public PlayerInfo()
        {

        }

        public PlayerInfo(PlayerData playerData)
        {
            this.Name = playerData.Name;
            this.SenderID = playerData.ID;
            this.ConnectionGUID = playerData.ConnectionGUID;
            this.PlayerID = playerData.ID;
        }

    }
}
