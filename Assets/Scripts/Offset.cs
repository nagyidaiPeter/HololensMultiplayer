using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offset : MonoBehaviour
{
    void Update()
    {
        transform.localEulerAngles = -transform.parent.eulerAngles;
    }
}
