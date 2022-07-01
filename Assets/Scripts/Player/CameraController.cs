#if false

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float xOffset, zOffset;

    public GameObject PlayerHead;

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        xOffset = transform.position.x - PlayerHead.transform.position.x;
        zOffset = transform.position.z - PlayerHead.transform.position.z;

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(PlayerHead.transform.position.x + xOffset, transform.position.y, PlayerHead.transform.position.z + zOffset);
    }

    public void camShake()
    {
        transform.position += transform.forward * 2;
    }
}
#endif