using Lidgren.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace hololensMultiplayer
{
    public class ObjectData
    {
        public int ID;
        public int OwnerID = -1;
        public string Name = "SyncedObject";

        public string ObjectType = "PrefabName";

        public Vector3 Position = Vector3.zero;
        public Quaternion Rotation = Quaternion.identity;
        public Vector3 Scale = Vector3.one;

        public GameObject gameObject;

        public ObjectData()
        {

        }
    }
}