using System.Collections;
using System.Collections.Generic;

using hololensMultiplayer;

using HRM;

using Microsoft.MixedReality.Toolkit.UI;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class NetworkPlayer : MonoBehaviour
{
    public int ID;

    public float InterVel = 5;
    public PlayerData playerData = null;

    public Transform RH, LH;

    void Start()
    {

    }

    void Update()
    {
        if (Vector3.Distance(new Vector3(playerData.Position.x, playerData.Position.y, playerData.Position.z), transform.localPosition) > 1.5f)
        {
            transform.localPosition = playerData.Position;
            transform.localRotation = playerData.Rotation;

            RH.position = transform.position + playerData.RHPosition;
            RH.localRotation = playerData.RHRotation;

            LH.position = transform.position + playerData.LHPosition;
            LH.localRotation = playerData.LHRotation;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, playerData.Position, InterVel * Time.deltaTime);
            RH.position = transform.position + playerData.RHPosition;
            LH.position = transform.position + playerData.LHPosition;

            transform.localRotation = playerData.Rotation;
            RH.localRotation = playerData.RHRotation;
            LH.localRotation = playerData.LHRotation;
        }
    }



    public class Factory : PlaceholderFactory<string, NetworkPlayer>
    {
    }
}
