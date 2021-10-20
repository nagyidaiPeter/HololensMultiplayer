using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offset : MonoBehaviour
{

    public Transform qr;

    void Update()
    {
        transform.position = -qr.position;
    }
}
