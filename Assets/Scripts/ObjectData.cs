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
        public int OwnerID;
        public string Name = "SyncedObject";

        public string ObjectType = "PrefabName";

        public Vector3 Position = new Vector3();
        public Quaternion Rotation = new Quaternion();
        public Vector3 Scale = new Vector3();

        public GameObject gameObject;

        public ObjectData()
        {

        }
    }
}