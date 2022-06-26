using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{

    public GameObject player;

    private void LateUpdate()
    {
        Vector3 newPostion = player.transform.position;
        newPostion.y = this.transform.position.y;
        this.transform.position = newPostion;
    }






}

