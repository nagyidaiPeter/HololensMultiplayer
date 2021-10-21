using hololensMultiplayer;
using hololensMultiplayer.Models;
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
        public Dictionary<int, ObjectData> Objects = new Dictionary<int, ObjectData>();
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

        public DataManager()
        {

        }
    }
}
