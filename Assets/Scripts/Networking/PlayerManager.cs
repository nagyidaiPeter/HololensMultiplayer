using System;
using System.Collections;
using System.Collections.Generic;

using Hrm;

using UnityEngine;

using static NetworkHandler;

public class PlayerManager : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public Transform Map;

    void Start()
    {
        Map = Map ?? transform;

        NetworkHandler.Processors[MessageTypes.PlayerTransform].ReceivedMessageEvent += PlayerTransformReceived;
    }

    private void PlayerTransformReceived(object msg)
    {
        if (msg is PlayerTransform pos)
        {
            if (NetworkHandler.Instance.Players.ContainsKey(pos.Id))
            {
                var player = NetworkHandler.Instance.Players[pos.Id];
                player.CurrentPos = new Vector3(pos.Position.X, pos.Position.Y, pos.Position.Z);
                player.CurrentRotation = new UnityEngine.Quaternion(pos.Rotation.X, pos.Rotation.Y, pos.Rotation.Z, pos.Rotation.W);
            }
            else
            {
                var newPlayer = Instantiate(PlayerPrefab, Map);
                newPlayer.transform.localPosition = new Vector3(pos.Position.X, pos.Position.Y, pos.Position.Z);
                newPlayer.transform.localRotation = new UnityEngine.Quaternion(pos.Rotation.X, pos.Rotation.Y, pos.Rotation.Z, pos.Rotation.W);
                newPlayer.GetComponent<NetworkPlayer>().ID = pos.Id;
                NetworkHandler.Instance.Players.Add(pos.Id, newPlayer.GetComponent<NetworkPlayer>());
            }
        }
    }

    void Update()
    {

    }
}
