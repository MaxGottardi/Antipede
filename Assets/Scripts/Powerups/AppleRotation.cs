using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleRotation : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 0.5f, 0 * Time.deltaTime);
    }
}
