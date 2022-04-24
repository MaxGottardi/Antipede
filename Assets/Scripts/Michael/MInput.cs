using UnityEngine;

[RequireComponent(typeof(CentipedeMovement))]
public class MInput : MonoBehaviour
{

	MCentipedeBody body;
	CentipedeMovement movement;
	public LayerMask EnemyLayer;

	bool doneAttack = false;

	void Start()
	{
		body = GetComponent<MCentipedeBody>();
		movement = GetComponent<CentipedeMovement>();
	}

	void Update()
	{
		Vector3 rayPos = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
		Debug.DrawRay(rayPos, transform.forward * 2, Color.green);
		if (Input.GetKeyDown(KeyCode.Space) && !doneAttack)
		{
			DoAttack();
			doneAttack = true;
			Invoke(nameof(wait), 0.5f);
		}

		if (Input.GetKeyDown(KeyCode.J))
		{
			body.AddSegment();
		}
		else if (Input.GetKeyDown(KeyCode.H))
		{
			body.RemoveSegment();
		}

		if (Input.GetKeyDown(KeyCode.K))
		{
			body.IncreaseSpeed(100.0f);
		}
		else if (Input.GetKeyDown(KeyCode.L))
		{
			body.DecreaseSpeed(100.0f);
		}

		float Horizontal = Input.GetAxisRaw("Horizontal");
		float Vertical = Input.GetAxisRaw("Vertical");

		movement.Set(ref Horizontal, ref Vertical);
	}

	void FixedUpdate()
	{
		movement.HandleMovement(ref body);
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
	void wait()
	{
		doneAttack = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("XDFFDSAD");
	}
}
