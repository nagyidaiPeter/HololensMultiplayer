using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offset : MonoBehaviour
{

    void Update()
    {
        transform.localPosition = -transform.parent.position;
        transform.localEulerAngles = Vector3.zero;
    }
}
