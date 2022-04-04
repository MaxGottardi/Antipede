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
    public float moveSpeed = 5f;

    private float timer = 0;
    private int fpsCount;

    private bool wHeld = false;
    private bool aHeld = false;
    private bool sHeld = false;
    private bool dHeld = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        nextSeg = GameObject.Find("Seg 1");
        fps = GameObject.FindGameObjectWithTag("FPS");
        fpsText = fps.GetComponent<TextMesh>();
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
        AreButtonsHeld();
        float dist = Vector3.Distance(transform.position, nextSeg.transform.position);

        if (wHeld && dist >= 1.05f)
        {
            //Debug.Log(dist);
            nextSeg.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1);
        }
        if (sHeld && dist >= 1.05f)
        {
            //Debug.Log(dist);
            nextSeg.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 1);
        }

        if (aHeld && dist >= 1.05f)
        {
            //Debug.Log(dist);
            nextSeg.transform.position = new Vector2(gameObject.transform.position.x + 1, gameObject.transform.position.y);
        }
        
        if (dHeld && dist >= 1.05f)
        {
            //Debug.Log(dist);
            nextSeg.transform.position = new Vector2(gameObject.transform.position.x - 1, gameObject.transform.position.y);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(dist);
        }
        dirY = Input.GetAxis("Vertical");
        dirX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, dirY * moveSpeed);
    }

    private void AreButtonsHeld()
    {
        //Checks if W is being Held - Going Up
        if (dirY > 0)
        {
            wHeld = true;
        }

        if (dirY == 0)
        {
            wHeld = false;
        }
        //Checks if A is being Held - Going Left
        if (dirX < 0)
        {
            aHeld = true;
        }

        if (dirX == 0)
        {
            aHeld = false;
        }
        //Checks if S is being Held - Going Down
        if (dirY < 0)
        {
            sHeld = true;
        }

        if (dirY == 0)
        {
            sHeld = false;
        }
        //Checks if D is being Held - Going Right
        if (dirX > 0)
        {
            dHeld = true;

        }

        if (dirX == 0)
        {
            dHeld = false;
        }
    }
}
