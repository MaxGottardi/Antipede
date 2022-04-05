using UnityEngine;

public class MInput : MonoBehaviour
{

	Rigidbody rb;

	MCentipedeBody body;

	float Horizontal;
	float Vertical;
	Vector3 InDirection;

	void Start()
	{
		body = GetComponent<MCentipedeBody>();
		rb = GetComponent<Rigidbody>();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			body.AddSegment();
		}
		if (Input.GetKeyDown(KeyCode.H))
        {
			//body.RemoveSegment();
        }

		Horizontal = Input.GetAxisRaw("Horizontal");
		Vertical = Input.GetAxisRaw("Vertical");

		InDirection = new Vector3(Horizontal, 0, Vertical).normalized;

		InDirection += transform.position;

	}

	void FixedUpdate()
	{
		if (Horizontal != 0 || Vertical != 0)
		{
			MMathStatics.HomeTowards(rb, InDirection, body.FollowSpeed, body.MaxTurnDegreesPerFrame);
		}
		else
		{
			rb.velocity = Vector3.zero;
		}
	}
}
