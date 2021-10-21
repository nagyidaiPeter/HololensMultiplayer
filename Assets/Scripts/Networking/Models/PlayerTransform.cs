using Assets.Scripts.SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace hololensMultiplayer.Models
{
    public class PlayerTransform : BaseMessageType
    {
        public Vector3 Pos { get; set; }
        public Quaternion Rot { get; set; }
        public Vector3 QrOffset { get; set; }

        public bool RHActive { get; set; }
        public Vector3 RHPos { get; set; }
        public Quaternion RHRot { get; set; }

        public bool LHActive { get; set; }
        public Vector3 LHPos { get; set; }
        public Quaternion LHRot { get; set; }

        public new MessageTypes MsgType = MessageTypes.PlayerTransform;
    }

}
