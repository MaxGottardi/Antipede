using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowShrink : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] float range;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float change = Mathf.Sin(Time.time * speed) * range;
        this.transform.localScale = new Vector3(change, change, this.transform.localScale.z);
    }
}
