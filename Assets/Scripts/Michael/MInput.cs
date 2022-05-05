using UnityEngine;

[RequireComponent(typeof(CentipedeMovement))]
public class MInput : MonoBehaviour
{
	MCentipedeBody body;
	CentipedeMovement movement;
	public LayerMask EnemyLayer;
	public GameObject hitParticles;

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
	}

	void LateUpdate()
	{
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
		transform.GetChild(0).GetComponent<Animator>().SetTrigger("Pincers");
		float dist = 0.45f;
		Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward * 1.1f, dist, EnemyLayer);
		GenericAnt closestAnt = null;
		float currDist = -1;
		
        foreach (Collider antCollider in colliders)
        {
			if (antCollider.gameObject.CompareTag("TarantulaTail"))
            {
				antCollider.gameObject.transform.parent.GetComponent<Tarantula>().DecreaseHealth();
				return;
			}
			
			if(antCollider.gameObject.CompareTag("Tarantula"))
            {
				antCollider.gameObject.GetComponent<Tarantula>().DecreaseHealth();
            }

			float newDist = Vector3.Distance(transform.position, antCollider.gameObject.transform.position);
			if (currDist < 0 || newDist < currDist)
				closestAnt = antCollider.gameObject.transform.parent.GetComponent<GenericAnt>();
		}


		if (closestAnt != null) //only reduce health on the closest ant hit
        {
			closestAnt.ReduceHealth(100);
			RaycastHit hit;
			if (Physics.Raycast(transform.position, closestAnt.gameObject.transform.position - transform.position, out hit, EnemyLayer))
				Instantiate(hitParticles, hit.point + transform.up * 0.5f, Quaternion.identity);
			else
				Instantiate(hitParticles, transform.position, Quaternion.identity);
		}

	}
    void wait()
	{
		doneAttack = false;
	}

	Vector3 MouseToWorldCoords()
	{
		Ray Ray = MainCamera.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(Ray, out RaycastHit Hit, 5000, 384); // Enemy and Ground Layers. (1 << 7 | 1 << 8)

		return Hit.point;
	}

}
