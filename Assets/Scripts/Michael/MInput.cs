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
		Debug.DrawRay(rayPos, transform.forward * 2, Color.red);
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
			body.RemoveSegment(100);
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

	/// <summary>
	/// for all enemies within a radius of the pincers, they get damaged
	/// </summary>
	void DoAttack()
	{
		////////GetComponent<Animator>().SetTrigger("Pincer");
		float dist = 0.45f;
		Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward * 1.1f, dist, EnemyLayer);
        foreach (Collider antCollider in colliders)
        {
			if(antCollider.gameObject.transform.parent.GetComponent<GenericAnt>())
				antCollider.gameObject.transform.parent.GetComponent<GenericAnt>().ReduceHealth(100);
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
