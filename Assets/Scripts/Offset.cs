using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offset : MonoBehaviour
{
    public Transform QR;

    void Update()
    {
        transform.position = transform.parent.position;
        transform.rotation = Quaternion.identity;
    }
}
