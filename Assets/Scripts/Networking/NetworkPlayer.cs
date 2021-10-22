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

    [Header("Right fingers")]
    public Transform RPinky, RRing, RMiddle, RIndex, RThumb;

    [Header("Left fingers")]
    public Transform LPinky, LRing, LMiddle, LIndex, LThumb;

    [Inject]
    DataManager dataManager;

    void Start()
    {
        if (dataManager.LocalPlayer.ID == playerData.ID)
        {
            dataManager.Players[playerData.ID].playerObject.GetComponent<MeshRenderer>().enabled = false;
            foreach (var renderer in dataManager.Players[playerData.ID].playerObject.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
        }
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

                RPinky.localEulerAngles = new Vector3(playerData.RHFingers.Pinky, 0);
                RRing.localEulerAngles =  new Vector3(playerData.RHFingers.Ring, 0);
                RMiddle.localEulerAngles =new Vector3(playerData.RHFingers.Middle, 0);
                RIndex.localEulerAngles = new Vector3(playerData.RHFingers.Index, 0);
                RThumb.localEulerAngles = new Vector3(playerData.RHFingers.Thumb, 0);
            }

            if (LH != null)
            {
                LH.localPosition = playerData.LHPosition;
                LH.localRotation = playerData.LHRotation;
                LH.gameObject.SetActive(playerData.LHActive);

                LPinky.localEulerAngles = new Vector3(playerData.LHFingers.Pinky, 0);
                LRing.localEulerAngles =  new Vector3(playerData.LHFingers.Ring, 0);
                LMiddle.localEulerAngles =new Vector3(playerData.LHFingers.Middle, 0);
                LIndex.localEulerAngles = new Vector3(playerData.LHFingers.Index, 0);
                LThumb.localEulerAngles = new Vector3(playerData.LHFingers.Thumb, 0);
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

                RPinky.localEulerAngles = new Vector3(playerData.RHFingers.Pinky, 0);
                RRing.localEulerAngles =  new Vector3(playerData.RHFingers.Ring, 0);
                RMiddle.localEulerAngles =new Vector3(playerData.RHFingers.Middle, 0);
                RIndex.localEulerAngles = new Vector3(playerData.RHFingers.Index, 0);
                RThumb.localEulerAngles = new Vector3(playerData.RHFingers.Thumb, 0);
            }

            if (LH != null)
            {
                LH.localPosition = Vector3.Lerp(LH.localPosition, playerData.LHPosition, HandInterVel * Time.deltaTime);
                LH.localRotation = Quaternion.Lerp(LH.localRotation, playerData.LHRotation, HandInterVel * Time.deltaTime);
                LH.gameObject.SetActive(playerData.LHActive);

                LPinky.localEulerAngles = new Vector3(playerData.LHFingers.Pinky, 0);
                LRing.localEulerAngles =  new Vector3(playerData.LHFingers.Ring, 0);
                LMiddle.localEulerAngles =new Vector3(playerData.LHFingers.Middle, 0);
                LIndex.localEulerAngles = new Vector3(playerData.LHFingers.Index, 0);
                LThumb.localEulerAngles = new Vector3(playerData.LHFingers.Thumb, 0);
            }
        }
    }



    public class Factory : PlaceholderFactory<string, NetworkPlayer>
    {
    }
}
