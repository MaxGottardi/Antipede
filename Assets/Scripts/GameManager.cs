using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private GameObject tailEnd;
    int[] numSegments;
    private int newSegNum = 0;

    public GameObject cloneSeg;
    

    // Start is called before the first frame update
    void Start()
    {
        numSegments = new int[100];
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (int i in numSegments)
            {
                tailEnd = GameObject.Find(newSegNum.ToString());              
                if (tailEnd == null)
                {
                    GameObject newSeg;
                    int tailSeg = newSegNum - 1;
                    newSeg = Instantiate(cloneSeg, GameObject.Find(tailSeg.ToString()).transform.position, GameObject.Find(tailSeg.ToString()).transform.rotation);
                    newSeg.name = newSegNum.ToString();
                    Debug.Log(tailSeg);
                    GameObject.Find(tailSeg.ToString()).GetComponent<SegmentMovement>().CheckForNextSeg();
                    break;
                }
                else if (tailEnd != null) 
                {
                    tailEnd = null;
                    newSegNum++;
                }
                
            }
        }
    }
}
