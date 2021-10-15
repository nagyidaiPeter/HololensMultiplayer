using System.Collections;
using System.Collections.Generic;

using HRM;

using LiteNetLib;

using Microsoft.MixedReality.Toolkit.UI;

using UnityEngine;
using UnityEngine.UI;

using static NetworkHandler;

public class NetworkPlayer : MonoBehaviour
{
    public int ID;
    public NetPeer peer;

    public float InterVel = 5;
    public Vector3 CurrentPos = new Vector3();
    public Quaternion CurrentRotation = new Quaternion();

    void Start()
    {

    }

    void Update()
    {
        if (Vector3.Distance(new Vector3(CurrentPos.x, transform.localPosition.y, CurrentPos.z), transform.localPosition) > 1.5f)
        {
            transform.localPosition = new Vector3(CurrentPos.x, CurrentPos.y, CurrentPos.z);
            transform.localRotation = CurrentRotation;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, CurrentPos, InterVel*Time.deltaTime);
            transform.localRotation = CurrentRotation;
        }
    }
}
