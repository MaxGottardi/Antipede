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
/// <br>MCentipedeConstructor.cs. (Contains the <see cref="Awake"/> method and first initialises this Centipede with <see cref="NumberOfSegments"/> starting Segments.</br>
/// <br>MCentipedeUtils.cs. (Contains the utility implementation for accessing Segments easier.</br>
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

	const float kMaxSpeed = 750;
	const float kDefaultSpeed = 150;

	public float slowTimer;
	public bool slowed;

	[Space(10)]

	public GameObject DeathScreen;
	public bool shieldActive;

	private GameObject[] checkPoints;
	private bool backupPlayer = false;

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
	}

	private void Update()
	{

		if (slowed == true)
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
		//		Debug.Log(shieldActive);
		//Debug.Log(Segments.Count);
		//		Debug.Log(U2I(SegmentsInfo.End));
		/*if (Input.GetKeyDown(KeyCode.Y)) {
			sfxManager.ActivateShield();
			shieldActive = true;
        }
		if (Input.GetKeyUp(KeyCode.Y))
        {
			sfxManager.DeactivateShield();
			shieldActive = false;
        }*/

	}

	public MSegment AddSegment()
	{
		if (slowed == true)
		{
			IncreaseSpeed(5);
		}
		else if (slowed == false)
		{
			IncreaseSpeed(10);
		}
		if (GameManager1.uiButtons)
			GameManager1.uiButtons.AddSegment();
		float Z = NumberOfSegments * SegmentsInfo.SegmentScale.z + DeltaZ;

		MSegment AddedSegment;

		if (SegmentsInfo.End > 0)
		{
			Transform End = GetLast();

			Vector3 Displacement = SegmentsInfo.SegmentScale.z * -End.forward;
			Tail.position += Displacement;

			foreach (MSegment C in CustomSegments)
				C.transform.position += Displacement;

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
			T.localPosition = Head.localPosition - new Vector3(0, 0, Z);

			T.parent = null;
		}
		else
		{
			Transform End = GetLast(1);
			T.position = End.position - End.forward * FollowDistance;
			T.LookAt(End);
		}

		TailSegment.Initialise(Weapons, MovementComponent, GetLast(), FollowSpeed, MaxTurnDegreesPerFrame, FollowDistance, AccelerationCurve);

		return Seg;
	}

	public void RemoveSegment(float healthReduction)//MSegment deadSegment)
	{
		if (!shieldActive && Segments != null && Segments.Count > 0)
		{
			//		Debug.Log("Killing Player");

			//Segments.Remove(Segments[Segments.Count - 1]);

			if (Segments.Count <= 0)
				return;


			MSegment lastSegment = GetLast();

			if (!lastSegment)
				return;

			if (lastSegment.ReduceHealth(healthReduction))
			{
				if (GameManager1.uiButtons != null)
					GameManager1.uiButtons.RemoveSegment();
				if (slowed == true)
				{
					DecreaseSpeed(5);
				}
				else if (slowed == false)
				{
					DecreaseSpeed(10);
				}

				Instantiate(DamageParticles, lastSegment.transform.position, Quaternion.identity);

				//Destroy(lastSegment.gameObject);
				lastSegment.Detach();

				//int nextIndex = 1;
				//while (segmentIndex + nextIndex < Segments.Count - 1 && !Segments[segmentIndex + nextIndex]) //if multiple in a row get destroyed at same time, prevents it from bugging out
				//	nextIndex++;
				//if (segmentIndex + nextIndex < Segments.Count - 1)
				//	Segments[segmentIndex + nextIndex].ForwardNeighbour = lastSegment.ForwardNeighbour;

				Segments.RemoveAt(Segments.Count - 1);
				--NumberOfSegments;
				GameManager1.cameraController.gameObject.transform.position += GameManager1.cameraController.gameObject.transform.forward * 2;

				int lastSegIndex = Segments.Count - 1;
				TailSegment.SetForwardNeighbour(Segments[lastSegIndex]);

				SegmentsInfo.RemoveSegment();

				// Ensure the Tail is properly 'attached' to the end Segment.
				Transform newLast = GetLast();
				Vector3 NewPos = newLast.position - newLast.forward * SegmentsInfo.SegmentScale.z;
				Tail.position = NewPos;

				for (byte i = 0; i < CustomSegments.Count; ++i)
					CustomSegments[i].transform.position = NewPos - (i * FollowDistance * newLast.forward);

			}

			// Make the check after removing a Segment.
			if (NumberOfSegments <= 1)
			{
				foreach (GameObject checkpoint in checkPoints)
				{
					if (checkpoint.GetComponent<Checkpoint>().backupPlayer != null)
					{
						checkpoint.SetActive(true);
						backupPlayer = true;
					}
				}

				if (backupPlayer == false)
				{
					Debug.Log("You Died");
					if (DeathScreen != null)
						DeathScreen.SetActive(true);
					Time.timeScale = 0;
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
}

