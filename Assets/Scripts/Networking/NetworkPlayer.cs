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
    public float InterVel = 35;
    public float HandInterVel = 35;
    public PlayerData playerData = null;

    public Transform RH, LH;

    void Start()
    {

    }

    void Update()
    {
        if (Vector3.Distance(new Vector3(playerData.Position.x, playerData.Position.y, playerData.Position.z), transform.position) > 1.5f)
        {
            transform.localPosition = playerData.Position;
            transform.localRotation = playerData.Rotation;

            if (RH != null)
            {
                RH.position = transform.position + playerData.RHPosition;
                RH.localRotation = playerData.RHRotation;
            }

            if (LH != null)
            {
                LH.position = transform.position + playerData.LHPosition;
                LH.localRotation = playerData.LHRotation;
            }
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, playerData.Position, InterVel * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, playerData.Rotation, InterVel * Time.deltaTime);

            if (RH != null)
            {
                RH.position = Vector3.Lerp(RH.position, transform.position + playerData.RHPosition, HandInterVel * Time.deltaTime);
                RH.localRotation = Quaternion.Lerp(RH.localRotation, playerData.RHRotation, HandInterVel * Time.deltaTime);
            }

            if (LH != null)
            {
                LH.position = Vector3.Lerp(LH.position, transform.position + playerData.LHPosition, HandInterVel * Time.deltaTime);
                LH.localRotation = Quaternion.Lerp(LH.localRotation, playerData.LHRotation, HandInterVel * Time.deltaTime);
            }
        }
    }



    public class Factory : PlaceholderFactory<string, NetworkPlayer>
    {
    }
}
