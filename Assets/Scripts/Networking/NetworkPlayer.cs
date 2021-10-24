
using hololensMultiplayer;

using UnityEngine;

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
        transform.localPosition = Vector3.Lerp(transform.localPosition, playerData.playerTransform.Pos, InterVel * Time.deltaTime);
        transform.localRotation = playerData.playerTransform.Rot;
        transform.localEulerAngles -= playerData.playerTransform.QrRotationOffset;

        if (RH != null)
        {
            RH.localPosition = Vector3.Lerp(RH.localPosition, playerData.playerTransform.RHPos, HandInterVel * Time.deltaTime);
            RH.localRotation = Quaternion.Lerp(RH.localRotation, playerData.playerTransform.RHRot, HandInterVel * Time.deltaTime);
            RH.gameObject.SetActive(playerData.playerTransform.RHActive);

            RPinky.localEulerAngles = new Vector3(playerData.playerTransform.RHFingers.Pinky, 0);
            RRing.localEulerAngles = new Vector3(playerData.playerTransform.RHFingers.Ring, 0);
            RMiddle.localEulerAngles = new Vector3(playerData.playerTransform.RHFingers.Middle, 0);
            RIndex.localEulerAngles = new Vector3(playerData.playerTransform.RHFingers.Index, 0);
            RThumb.localEulerAngles = new Vector3(playerData.playerTransform.RHFingers.Thumb, 0);
        }

        if (LH != null)
        {
            LH.localPosition = Vector3.Lerp(LH.localPosition, playerData.playerTransform.LHPos, HandInterVel * Time.deltaTime);
            LH.localRotation = Quaternion.Lerp(LH.localRotation, playerData.playerTransform.LHRot, HandInterVel * Time.deltaTime);
            LH.gameObject.SetActive(playerData.playerTransform.LHActive);

            LPinky.localEulerAngles = new Vector3(playerData.playerTransform.LHFingers.Pinky, 0);
            LRing.localEulerAngles = new Vector3(playerData.playerTransform.LHFingers.Ring, 0);
            LMiddle.localEulerAngles = new Vector3(playerData.playerTransform.LHFingers.Middle, 0);
            LIndex.localEulerAngles = new Vector3(playerData.playerTransform.LHFingers.Index, 0);
            LThumb.localEulerAngles = new Vector3(playerData.playerTransform.LHFingers.Thumb, 0);
        }
    }



    public class Factory : PlaceholderFactory<string, NetworkPlayer>
    {
    }
}
