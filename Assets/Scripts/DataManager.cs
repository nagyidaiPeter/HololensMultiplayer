using hololensMultiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace hololensMultiplayer
{
    public class DataManager
    {
        public Dictionary<int, PlayerData> Players = new Dictionary<int, PlayerData>();

        public PlayerData LocalPlayer;
      
        public bool Welcomed = false;


        public delegate void StartingServer();
        public event StartingServer StartingServerEvent;
        private bool _isServer = false;
        public bool IsServer
        {
            get
            {
                return _isServer;
            }

            set
            {
                _isServer = value;
                if (_isServer)
                    StartingServerEvent?.Invoke();
            }
        }


        public PlayerData GetPlayerById(int ID)
        {
            return Players[ID];
        }

        public PlayerData GetPlayerByGuid(string guid)
        {
            return Players.FirstOrDefault(x => x.Value.ConnectionGUID == guid).Value;
        }

        public DataManager()
        {

        }
    }
}
