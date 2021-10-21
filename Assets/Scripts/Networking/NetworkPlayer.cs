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
            transform.localEulerAngles -= playerData.QrOffset;

            if (RH != null)
            {
                RH.localPosition = playerData.RHPosition;
                RH.localRotation = playerData.RHRotation;
                RH.gameObject.SetActive(playerData.RHActive);
            }

            if (LH != null)
            {
                LH.localPosition = playerData.LHPosition;
                LH.localRotation = playerData.LHRotation;
                LH.gameObject.SetActive(playerData.LHActive);
            }
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, playerData.Position, InterVel * Time.deltaTime);
            transform.localRotation = playerData.Rotation;
            transform.localEulerAngles -= playerData.QrOffset;

            if (RH != null)
            {
                RH.localPosition = Vector3.Lerp(RH.localPosition, playerData.RHPosition, HandInterVel * Time.deltaTime);
                RH.localRotation = Quaternion.Lerp(RH.localRotation, playerData.RHRotation, HandInterVel * Time.deltaTime);
                RH.gameObject.SetActive(playerData.RHActive);
            }

            if (LH != null)
            {
                LH.localPosition = Vector3.Lerp(LH.localPosition, playerData.LHPosition, HandInterVel * Time.deltaTime);
                LH.localRotation = Quaternion.Lerp(LH.localRotation, playerData.LHRotation, HandInterVel * Time.deltaTime);
                LH.gameObject.SetActive(playerData.LHActive);
            }
        }
    }



    public class Factory : PlaceholderFactory<string, NetworkPlayer>
    {
    }
}
