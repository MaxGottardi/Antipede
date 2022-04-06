using UnityEngine;

public class MInput : MonoBehaviour
{

	Rigidbody rb;

	MCentipedeBody body;
	public LayerMask EnemyLayer;

	float Horizontal;
	float Vertical;
	Vector3 InDirection;

	bool doneAttack = false;

	void Start()
	{
		body = GetComponent<MCentipedeBody>();
		rb = GetComponent<Rigidbody>();
	}
	void wait()
    {
		doneAttack = false;
    }
	void Update()
	{
		Vector3 rayPos = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
		Debug.DrawRay(rayPos, transform.forward * 2, Color.green);
		if (Input.GetKeyDown(KeyCode.Space) && !doneAttack)
		{
			DoAttack();
			doneAttack = true;
			Invoke("wait", 0.5f);
		}
		if (Input.GetKeyDown(KeyCode.J))
		{
			body.AddSegment();
		}
		if (Input.GetKeyDown(KeyCode.H))
        {
			body.RemoveSegment();
        }
		if (Input.GetKeyDown(KeyCode.K))
        {
			body.IncreaseSpeed(100.0f);
        }
		if (Input.GetKeyDown(KeyCode.L))
        {
			body.DecreaseSpeed(100.0f);
        }

		Horizontal = Input.GetAxisRaw("Horizontal");
		Vertical = Input.GetAxisRaw("Vertical");

		InDirection = new Vector3(Horizontal, 0, Vertical).normalized;

		InDirection += transform.position;

	}

	void DoAttack()
    {
		GetComponent<Animator>().SetTrigger("Pincer");
		Vector3 rayPos = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);

		RaycastHit hit;
		float dist = 4.0f;
		if (Physics.Raycast(rayPos, transform.forward, out hit, dist, EnemyLayer)
			|| Physics.Raycast(rayPos, (transform.forward + transform.right / 3), out hit, dist, EnemyLayer)
			|| Physics.Raycast(rayPos, (transform.forward - transform.right / 3), out hit, dist, EnemyLayer))
		{
			Destroy(hit.collider.gameObject);
			body.AddSegment();
        }

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
