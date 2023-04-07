using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCamera : MonoBehaviour
{
    void LateUpdate()
    {
        this.transform.rotation = Quaternion.FromToRotation(Vector3.back, Camera.main.transform.position - this.transform.position);
    }
}
