using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The central class that handles Centipede logic.
/// <br></br>
/// <br>Any other Centipede Component can be accessed from here.</br>
/// <br><br>Includes:</br></br>
/// <br><see cref="CentipedeMovement"/></br>
/// <br><see cref="MCentipedeWeapons"/></br>
/// <br><see cref="MCentipedeEvents"/></br>
/// <br>Handles <see cref="MInput"/></br>
/// <br>Centipede <see cref="Construct"/></br>
/// <br>References to any attached <see cref="MSegment"/></br>
/// </summary>
/// <remarks>
/// <b>This class is split into 3 different files:</b>
/// <br>This MCentipedeBody.cs file. (Contains the bulk logic of the Centipede)</br>
/// <br>MCentipedeConstructor.cs. (Contains the <see cref="Awake"/> method and first initialises this Centipede with <see cref="NumberOfSegments"/> starting Segments)</br>
/// <br>MCentipedeUtils.cs. (Contains the utility implementation for accessing Segments easier)</br>
/// </remarks>
[RequireComponent(typeof(MCentipedeEvents))]
public partial class MCentipedeBody : MonoBehaviour
{
	[Header("Construction References.")]

	public Transform Head;
	public Transform Tail;
	public GameObject DamageParticles;
	MSegment TailSegment;
	[HideInInspector] public MCentipedeWeapons Weapons;

	[SerializeField] MSegment Segment;
	[SerializeField, Tooltip("The number of Segments now (during runtime), and to begin with.")] uint NumberOfSegments;

	[Header("Centipede Movement Settings.")]
	[Min(Vector3.kEpsilon), Tooltip("The speed of the Centipede's Head.")] public float MovementSpeed = 150f;
	[Min(Vector3.kEpsilon)] public float TurnDegrees = 7f;
	private float preSlowedSpeed;
	CentipedeMovement MovementComponent;
	AnimationCurve AccelerationCurve;

	[Header("Segment Settings.")]
	[Min(Vector3.kEpsilon), Tooltip("How fast should the Segments/Tails follow their ForwardNeighbour?")] public float FollowSpeed = 150f;
	[SerializeField, Min(Vector3.kEpsilon)] float FollowDistance = .2f;
	[Min(Vector3.kEpsilon)] public float MaxTurnDegreesPerFrame = 7f;
	[SerializeField, Tooltip("Any additional Segments that are not spawned in Construct.")] List<MSegment> CustomSegments;

	public List<MSegment> Segments;
	SegmentsInformation SegmentsInfo;

	public const float kBufferZone = 1.5f;

	const float kMaxSpeed = 750;
	const float kDefaultSpeed = 150;

	public float slowTimer;
	public bool slowed;

	[Space(10)]

	public GameObject DeathScreen;
	public bool shieldActive;

	private GameObject[] checkPoints;
	private bool backupPlayerExists = false;
	public static MCentipedeBody newPlayer;

	public float distanceToNest;
	public Tarantula[] nests;


	void Start()
	{
		shieldActive = false;
		//shieldDuration = 5.0f;
		Weapons = GetComponent<MCentipedeWeapons>();
		MovementComponent = GetComponent<CentipedeMovement>();

		AccelerationCurve = MovementComponent.AccelerationCurve;

		TailSegment = Tail.GetComponent<MSegment>();
		TailSegment.Initialise(Weapons, MovementComponent, Head, FollowSpeed, MaxTurnDegreesPerFrame, FollowDistance, AccelerationCurve);
		TailSegment.transform.parent = null;

		foreach (MSegment MS in CustomSegments)
		{
			MS.Initialise(Weapons, MovementComponent, null, FollowSpeed, MaxTurnDegreesPerFrame, FollowDistance, AccelerationCurve);
			MS.transform.localEulerAngles = Vector3.zero;
			MS.transform.parent = null;
		}

		checkPoints = GameObject.FindGameObjectsWithTag("CheckPoint");

		slowed = false;
		Construct();

		if (newPlayer)
		{
			// Ensure Custom Segments spawn at the correctly when restarting from a Checkpoint.
			for (byte i = 0; i < CustomSegments.Count; ++i)
			{

				MSegment ThisCustomSegment = CustomSegments[i];
				if (i == 0)
				{
					MSegment LastSegment = newPlayer.GetLast(1);
					Tail.position = LastSegment.transform.position - FollowDistance * LastSegment.transform.forward * kBufferZone;

					MSegment TailSegment = newPlayer.GetLast();
					ThisCustomSegment.transform.position = TailSegment.transform.position - FollowDistance * TailSegment.transform.forward * kBufferZone;
				}
				else
				{
					Vector3 ForwardNeighbourPosition = ThisCustomSegment.ForwardNeighbour.position;
					ThisCustomSegment.transform.position = ForwardNeighbourPosition -
						i * FollowDistance * ThisCustomSegment.ForwardNeighbour.forward
						* kBufferZone;
				}
			}
		}

		nests = Object.FindObjectsOfType<Tarantula>();

	}

	void Update()
	{
		if (slowed)
		{
			slowTimer += Time.deltaTime;
			if (slowTimer >= 5)
			{
				MovementSpeed = preSlowedSpeed;
				ChangeSpeedDirectly(MovementSpeed);
				slowTimer = 0;
				slowed = false;
			}
		}

		distanceToNest = CalculateNestDistance();
	}

	float CalculateNestDistance()
    {
		float smallestDistance = 0;

		if (nests.Length > 0)
        {
			foreach (Tarantula t in nests)
            {
				float f = Vector3.Distance(t.transform.position, Head.transform.position);
				if (f > smallestDistance)
                {
					smallestDistance = f;
                }
            }
        }

		return smallestDistance;
    }

	/// <summary>Adds a Segment to the back of the Centipede.</summary>
	/// <remarks>
	/// First-level Abstraction.
	/// <br></br>
	/// This is the preparation phase for adding a Segment.
	/// </remarks>
	/// <returns>The <see cref="MSegment"/> that was added, or null if something failed.</returns>
	public MSegment AddSegment()
	{
		IncreaseSpeed(slowed ? 5 : 10);

		if (GameManager1.uiButtons)
			GameManager1.uiButtons.AddSegment();

		float Z = NumberOfSegments * SegmentsInfo.SegmentScale.z + DeltaZ;

		MSegment AddedSegment;

		if (SegmentsInfo.End > 0)
		{
			Transform End = GetLast();

			Vector3 Displacement = kBufferZone * SegmentsInfo.SegmentScale.z * -End.forward;
			Tail.position += Displacement;

			for (byte i = 0; i < CustomSegments.Count; ++i)
				CustomSegments[i].transform.position = (i + 1) * Displacement + Tail.position;

			Quaternion Rot = MMathStatics.DirectionToQuat(((Transform)GetLast(1)).position, End.position);

			AddedSegment = AddSegment(Z, Rot);
		}
		else
		{
			Vector3 Displacement = SegmentsInfo.SegmentScale.z * -Head.forward;
			Tail.position += Displacement;

			foreach (MSegment C in CustomSegments)
				C.transform.position += Displacement;

			AddedSegment = AddSegment(Z, Head.rotation);
		}

		++NumberOfSegments;

		if (AddedSegment)
		{
			AddedSegment.InjectAccelerationTime(1f);
			return AddedSegment;
		}

		Debug.LogError("No Segment was added!");
		return null;
	}

	/// <summary>Adds a Segment to the back of the Centipede.</summary>
	/// <remarks>
	/// Second-level Abstraction.
	/// <br></br>
	/// This is where an <see cref="MSegment"/> gets instantiated, added, and fully registered to the Centipede.
	/// </remarks>
	/// <param name="Z">The Z-Axis gap between the first Segment to be added and <see cref="Head"/>.</param>
	/// <param name="Rot">The Rotation to orient the new Segment to face a previously added Segment.</param>
	/// <returns>The <see cref="MSegment"/> that was added, or null if something failed.</returns>
	MSegment AddSegment(float Z, Quaternion Rot)
	{
		// Inherit Centipede's rotation.
		MSegment Seg = Instantiate(Segment, Vector3.zero, Rot);
		Seg.Initialise(Weapons, MovementComponent, Segments.Count == 0 ? Head : GetLast(), FollowSpeed, MaxTurnDegreesPerFrame, FollowDistance, AccelerationCurve);
		Seg.name = "Segment: " + Segments.Count;

		Segments.Add(Seg);
		SegmentsInfo.AddSegment();

		// Implicit conversion to Transform.
		Transform T = Seg;

		if (SegmentsInfo.End == 0)
		{
			// Set parent to the Centipede's object and inherit local position.
			T.SetParent(transform);
			T.localPosition = Head.localPosition - new Vector3(0, 0, Z * kBufferZone);

			T.parent = null;
		}
		else
		{
			Transform End = GetLast(1);
			T.position = End.position - End.forward * FollowDistance * kBufferZone;
			T.LookAt(End);
		}

		TailSegment.Initialise(Weapons, MovementComponent, GetLast(), FollowSpeed, MaxTurnDegreesPerFrame, FollowDistance, AccelerationCurve);

		return Seg;
	}

	/// <summary>Removes or damages <see cref="GetLast"/>.</summary>
	/// <param name="healthReduction">The health to deduct from <see cref="GetLast"/>.</param>
	/// <param name="particalPos">Where should the damage particles spawn?</param>
	public void RemoveSegment(float healthReduction, Vector3 particalPos)
	{
		if (!shieldActive && Segments != null && Segments.Count > 0)
		{
			if (Segments.Count <= 0)
				return;

			MSegment lastSegment = GetLast();

			if (!lastSegment)
				return;

			if (lastSegment.ReduceHealth(healthReduction))
			{
				if (GameManager1.uiButtons != null)
					GameManager1.uiButtons.RemoveSegment();

				DecreaseSpeed(slowed ? 5 : 10);

				Instantiate(DamageParticles, particalPos, Quaternion.identity);

				// Fling!
				lastSegment.Detach();

				// Remove this Segment.
				Segments.RemoveAt(Segments.Count - 1);
				--NumberOfSegments;
				SegmentsInfo.RemoveSegment();

				// Camera feedback for losing a Segment. (Goes forward, SpringArm will return it to normal)
				GameManager1.cameraController.gameObject.transform.position += GameManager1.cameraController.gameObject.transform.forward * 2;

				// Update the Tail's new ForwardNeighbour.
				int lastSegIndex = Segments.Count - 1;
				TailSegment.SetForwardNeighbour(Segments[lastSegIndex]);

				// Ensure the Tail is properly 'attached' to the end Segment.
				Transform newLast = GetLast();
				Vector3 NewPos = newLast.position - newLast.forward * SegmentsInfo.SegmentScale.z * kBufferZone;
				Tail.position = NewPos;

				for (byte i = 0; i < CustomSegments.Count; ++i)
					CustomSegments[i].transform.position = NewPos - ((i + 1) * FollowDistance * newLast.forward) * kBufferZone;
			}

			// Make the check after removing a Segment.
			if (NumberOfSegments <= 1)
			{
				foreach (GameObject checkpoint in checkPoints)
				{
					if (checkpoint.TryGetComponent(out Checkpoint Checkpoint) && Checkpoint.backupPlayerExists)
					{
						// Call Construct on the new Centipede.
						newPlayer.gameObject.SetActive(true);

						// Reset its position from underground.
						newPlayer.transform.position = new Vector3(newPlayer.transform.position.x, newPlayer.transform.position.y + 5, newPlayer.transform.position.z);
						newPlayer.name = "Centipede";

						backupPlayerExists = true;

						// Give back the Weapons.
						Checkpoint.SpawnBackupWeapons();

						// Reset the Camera Target.
						SpringArm.Instance.Target = newPlayer.transform;

						// Because the Centipede is considered dead at 1 Segment remaining,
						// ensure the 0'th Segment is destroyed as well.
						Destroy(Segments[0].gameObject);

						// Destroy this (dead) Centipede.
						Destroy(gameObject);

						// Mark the Player as having NOT gone through a Checkpoint.
						Checkpoint.backupPlayerExists = false;
					}
				}

				if (backupPlayerExists != true)
				{
					Debug.Log("You Died");
					if (DeathScreen != null)
						DeathScreen.SetActive(true);
					Time.timeScale = 0;
				}
				else
				{
					backupPlayerExists = false;
				}
			}
		}
	}

	/// <summary>Increases the MovementSpeed of this Centipede by value, or limits to <see cref="kMaxSpeed"/>.</summary>
	/// <param name="value">The movement speed to add.</param>
	public void IncreaseSpeed(float value)
	{
		DetermineSpeed(value);
	}

#if UNITY_EDITOR
	[System.Obsolete("Do not call SetSpeed(). See ChangeSpeedDirectly(), or make a new function.")]
	public void SetSpeed(float value)
	{
		throw new System.NotImplementedException("Do not call SetSpeed(). See ChangeSpeedDirectly(), or make a new function.");
	}
#endif

	void DetermineSpeed(float Speed)
	{
		// Maximum Speed Limit.
		if (MovementSpeed + Speed >= kMaxSpeed)
		{
			ChangeSpeedDirectly(kMaxSpeed);
		}
		// Minimum Speed Limit.
		else if (MovementSpeed + Speed <= kDefaultSpeed)
		{
			ChangeSpeedDirectly(kDefaultSpeed);
		}
		// Otherwise, set NewSpeed.
		else
		{
			// Speed will either be positive or negative, depending
			// on increasing or decreasing speed.

			MovementSpeed += Speed;
			FollowSpeed += Speed;

			// Every Segment needs a speed update.
			foreach (MSegment segment in Segments)
				segment.FollowSpeed += Speed;

			// If there are any Custom Segments, update them as well.
			foreach (MSegment S in CustomSegments)
				S.FollowSpeed += Speed;

			// The Tail is a standalone Segment. Update it as well.
			TailSegment.FollowSpeed += Speed;
		}
	}

	/// <summary>Immediately change the speed of the Centipede and its Segments.</summary>
	/// <remarks>Ignores speed limit checks.</remarks>
	/// <param name="NewSpeed">The new movement speed this Centipede and its Segments will travel at.</param>
	public void ChangeSpeedDirectly(float NewSpeed)
	{
		MovementSpeed = NewSpeed;
		FollowSpeed = NewSpeed;

		foreach (MSegment segment in Segments)
			segment.FollowSpeed = NewSpeed;

		foreach (MSegment S in CustomSegments)
			S.FollowSpeed = NewSpeed;

		TailSegment.FollowSpeed = NewSpeed;
	}

	/// <summary>Decreases the MovementSpeed of this Centipede by value, or limits to <see cref="kDefaultSpeed"/>.</summary>
	/// <param name="value">The movement speed to decrease.</param>
	public void DecreaseSpeed(float value)
	{
		DetermineSpeed(-value);
	}

	public void tempSlowSpeed()
	{
		if (!slowed)
		{
			preSlowedSpeed = MovementSpeed;
			ChangeSpeedDirectly(MovementSpeed * .5f);
			slowed = true;
		}
	}

#if UNITY_EDITOR
	void OnGUI()
	{
		GUI.Label(new Rect(10, 25, 250, 150), "Movement Speed: " + MovementSpeed);
		GUI.Label(new Rect(10, 55, 250, 150), "Number of Segments: " + NumberOfSegments);
		GUI.Label(new Rect(10, 70, 250, 150), "Distance to Nest: " + Mathf.Round(distanceToNest));
	}

#endif
}

