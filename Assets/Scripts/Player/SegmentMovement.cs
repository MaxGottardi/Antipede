using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentMovement : MonoBehaviour
{
    private GameObject prevSeg;
    private int prevSegInt;
    private GameObject nextSeg;
    private int nextSegInt;

    private float dist;
    // Start is called before the first frame update
    void Start()
    {
        prevSegInt = int.Parse(gameObject.name) - 1;
        prevSeg = GameObject.Find(prevSegInt.ToString());
        nextSegInt = int.Parse(gameObject.name) + 1;
        nextSeg = GameObject.Find(nextSegInt.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(nextSegInt);
        }

        dist = Vector3.Distance(transform.position, nextSeg.transform.position);
        //Going Up
        if (prevSeg.transform.position.y == transform.position.y + 1 && dist >= 1.05f)
        {
            //Debug.Log(dist);
            nextSeg.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1);
        }
        //Going Down
        if (prevSeg.transform.position.y == transform.position.y - 1 && dist >= 1.05f)
        {
            //Debug.Log(dist);
            nextSeg.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 1);
        }
        //Going Left
        if (prevSeg.transform.position.x == transform.position.x - 1 && dist >= 1.05f)
        {
            //Debug.Log(dist);
            nextSeg.transform.position = new Vector2(gameObject.transform.position.x + 1, gameObject.transform.position.y);
        }
        //Going Right
        if (prevSeg.transform.position.x == transform.position.x + 1 && dist >= 1.05f)
        {
            //Debug.Log(dist);
            nextSeg.transform.position = new Vector2(gameObject.transform.position.x - 1, gameObject.transform.position.y);
        }
    }
}
