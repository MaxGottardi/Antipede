using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMovement : MonoBehaviour
{

    public Vector3 startPosition;
    bool doMove;


    // Start is called before the first frame update
    void Start()
    {
        startPosition = this.gameObject.transform.position;
        doMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (doMove)
        {
            if (this.gameObject.transform.position.y >= -20.0f)
            {
                this.gameObject.transform.Translate(Vector3.down * Time.deltaTime * 1f);
            }
        }
    }

    public void SetMovable()
    {
        doMove = true;
    }
}
