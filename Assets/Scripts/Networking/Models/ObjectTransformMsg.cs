using Assets.Scripts.SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace hololensMultiplayer.Models
{
    public class ObjectTransformMsg : BaseMessageType
    {
        public int ObjectID { get; set; }
        public string ObjectType { get; set; }
        public int OwnerID { get; set; }
        public Vector3 Pos { get; set; }
        public Quaternion Rot { get; set; }
        public Vector3 Scale { get; set; }

        public new MessageTypes MsgType = MessageTypes.ObjectTransform;
    }

}
