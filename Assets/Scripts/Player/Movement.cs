using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject nextSeg;
    private GameObject fps;
    private TextMesh fpsText;

    private float dirY;
    private float dirX;
    public float moveSpeed = 10f;
    private float dist;

    private float timer = 0;
    private int fpsCount;

    public bool wHeld = false;
    public bool aHeld = false;
    public bool sHeld = false;
    public bool dHeld = false;

    private int nextSegInt;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //nextSeg = GameObject.Find("1");
        fps = GameObject.FindGameObjectWithTag("FPS");
        fpsText = fps.GetComponent<TextMesh>();

        nextSegInt = int.Parse(gameObject.name) + 1;
        nextSeg = GameObject.Find(nextSegInt.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        //FPS counter, i put it here because i was checking something it should probably be moved
        timer += Time.deltaTime;
        fpsCount++;
        if (timer >= 1)
        {
            fpsText.text = "FPS: " + fpsCount;
            timer = 0;
            fpsCount = 0;
        }
        
        

        dirY = Input.GetAxis("Vertical");
        dirX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, dirY * moveSpeed);

        if (nextSeg != null)
        {
            UpdateNextSeg();
        }
    }

    private void UpdateNextSeg()
    {
        Vector3 dir = nextSeg.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * RotationSpeed);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        float dist = Vector3.Distance(nextSeg.transform.position, transform.position);

        if (dist >= 1.25)
        {
            float moveDist = dist - 0.0416f;
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * moveDist;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * moveDist;

            nextSeg.transform.position = new Vector3(x, y, 0f) + transform.position;

        }
    }
}
