using Microsoft.MixedReality.OpenXR.Remoting;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotingHandler : MonoBehaviour
{
    public string Address { get; set; }

    public void Connect()
    {
        var splitted = Address.Split(':');

        if (splitted.Length != 2)
            return;

        string ip = splitted[0];
        ushort port;
        ushort.TryParse(splitted[1], out port);

        RemotingConfiguration configuration = new RemotingConfiguration()
        {
            RemoteHostName = ip,
            RemotePort = port,
            MaxBitrateKbps = 20000,
            EnableAudio = false,
            VideoCodec = RemotingVideoCodec.Auto
        };

        StartCoroutine(Microsoft.MixedReality.OpenXR.Remoting.AppRemoting.Connect(configuration));
    }
}
