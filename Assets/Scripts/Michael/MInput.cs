using UnityEngine;

[RequireComponent(typeof(CentipedeMovement))]
public class MInput : MonoBehaviour
{
	MCentipedeBody body;
	CentipedeMovement movement;
	public LayerMask EnemyLayer;
	public GameObject hitParticles;

	bool doneAttack = false, attackRequested = false;
	public static Camera MainCamera;

	float PreSlowShift;
	SFXManager sfxManager;

	bool bIsPaused = false;
    private void Awake()
    {
		MainCamera = Camera.main;
	}
	void Start()
	{
		body = GetComponent<MCentipedeBody>();
		movement = GetComponent<CentipedeMovement>();

		if (GameObject.Find("SFXMAnager"))
			sfxManager = GameObject.Find("SFXMAnager").GetComponent<SFXManager>();

		GameSettings.OnPause += OnPause;
		GameSettings.OnResume += OnResume;
	}

	void Update()
	{
		if (bIsPaused)
			return;

		Vector3 rayPos = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
		Debug.DrawRay(rayPos, transform.forward * 2, Color.red);
		if (Time.timeScale > 0.1f && (Input.GetKeyDown(SettingsVariables.keyDictionary["Fire"]) || attackRequested))
		{
			if (!doneAttack)
			{
				DoAttack();
				doneAttack = true;
				attackRequested = false;
				Invoke("wait", 0.5f);
			}
			else if (!attackRequested)
				attackRequested = true;

		}

		if (Input.GetKeyDown(KeyCode.J))
		{
			//body.AddSegment();
		}
		else if (Input.GetKeyDown(KeyCode.H))
		{
			//body.RemoveSegment(100, transform.position);
		}

		if (Input.GetKeyDown(KeyCode.K))
		{
			//body.IncreaseSpeed(100.0f);
		}
		else if (Input.GetKeyDown(KeyCode.L))
		{
			//body.DecreaseSpeed(100.0f);
		}

		if (Input.GetKeyDown(SettingsVariables.keyDictionary["HalveSpeed"]))
		{
			PreSlowShift = body.MovementSpeed;
			body.ChangeSpeedDirectly(PreSlowShift * .5f);
		}
		else if (Input.GetKeyUp(SettingsVariables.keyDictionary["HalveSpeed"]))
		{
			body.ChangeSpeedDirectly(PreSlowShift);
		}

		float Horizontal = Input.GetAxisRaw("Horizontal");
		float Vertical = Input.GetAxisRaw("Vertical");

		movement.Set(ref Horizontal, ref Vertical, ref body);
		if (Horizontal != 0 || Vertical != 0)
			if (sfxManager != null && Time.timeScale > 0)
				sfxManager.Walk();
	}

	void LateUpdate()
	{
		if (bIsPaused)
			return;

		body.Weapons.ReceiveMouseCoords(MouseToWorldCoords());
	}

	void FixedUpdate()
	{
		if (bIsPaused)
			return;

		movement.HandleMovement(ref body);
	}

	void OnPause()
	{
		bIsPaused = true;
	}

	void OnResume()
	{
		bIsPaused = false;
	}

	/// <summary>
	/// for all enemies within a radius of the pincers, they get damaged
	/// </summary>
	void DoAttack()
	{
		sfxManager.CollectLarvae();
		transform.GetChild(0).GetComponent<Animator>().SetTrigger("Pincers");
		float dist = 1.25f;
		Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward * 1.1f, dist, EnemyLayer);
		GenericAnt closestAnt = null;
		float currDist = -1;

		bool seenTail = false, seenTarant = false;
		Tarantula tarant = null;
		foreach (Collider antCollider in colliders)
		{//im not really sure why this works for differentiating between the tail and the rest of the body
		 //but it does so im rolling with it (especially because it wasnt working before)
			if (antCollider.gameObject.CompareTag("TarantulaTail"))
			{
				tarant = antCollider.gameObject.transform.parent.GetComponent<Tarantula>();//.DecreaseHealth(2);
				seenTail = true;
			}

			if (antCollider.gameObject.CompareTag("Tarantula"))
			{
				tarant = antCollider.gameObject.GetComponent<Tarantula>();//.DecreaseHealth(1);
				//Instantiate(hitParticles, antCollider.gameObject.transform.position, Quaternion.identity);
				seenTarant = true;
			}

			if (antCollider.gameObject.CompareTag("Enemy"))
			{
				float newDist = Vector3.Distance(transform.position, antCollider.gameObject.transform.position);
				if (currDist < 0 || newDist < currDist)
					closestAnt = antCollider.gameObject.transform.parent.GetComponent<GenericAnt>();
			}
		}

		if(seenTail)
        {
			tarant.DecreaseHealth(2);
			Instantiate(hitParticles, tarant.gameObject.transform.position, Quaternion.identity);
		}
		else if(seenTarant)
        {
			tarant.DecreaseHealth(1);
			Instantiate(hitParticles, tarant.gameObject.transform.position, Quaternion.identity);
		}
		else if (closestAnt != null) //only reduce health on the closest ant hit
		{
			closestAnt.ReduceHealth(100);
			Instantiate(hitParticles, closestAnt.transform.position, Quaternion.identity);
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
