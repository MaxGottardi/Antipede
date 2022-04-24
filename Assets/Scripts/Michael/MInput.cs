using UnityEngine;

[RequireComponent(typeof(CentipedeMovement))]
public class MInput : MonoBehaviour
{

	MCentipedeBody body;
	CentipedeMovement movement;
	public LayerMask EnemyLayer;

	float Horizontal;
	float Vertical;
	Vector3 InDirection;

	bool doneAttack = false, attackRequested = false;
	Camera MainCamera;

	void Start()
	{
		body = GetComponent<MCentipedeBody>();
		movement = GetComponent<CentipedeMovement>();
		MainCamera = Camera.main;
	}

	void Update()
	{
		Vector3 rayPos = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
		Debug.DrawRay(rayPos, transform.forward * 2, Color.green);
		if (Input.GetKeyDown(KeyCode.Space) || attackRequested)
		{
			if (!doneAttack)
			{
				DoAttack();
				doneAttack = true;
				attackRequested = false;
				Invoke("wait", 0.5f);
			}
			else if(!attackRequested)
				attackRequested = true;

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

		body.Weapons.ReceiveMouseCoords(MouseToWorldCoords());
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
			if(hit.collider.gameObject.transform.parent.GetComponent<GenericAnt>())
				hit.collider.gameObject.transform.parent.GetComponent<GenericAnt>().ReduceHealth(100);
			//body.AddSegment();
        }

	}
	void wait()
	{
		doneAttack = false;
	}

	Vector3 MouseToWorldCoords()
	{
		Ray Ray = MainCamera.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(Ray, out RaycastHit Hit, 5000);

		return Hit.point;
	}
}
