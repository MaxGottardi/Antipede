using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MCentipedeEvents))]
public partial class MCentipedeBody : MonoBehaviour
{
	[Header("Construction References.")]

	public Transform Head;
	public Transform Tail;
	MSegment TailSegment;
	[HideInInspector] public MCentipedeWeapons Weapons;

	[SerializeField] MSegment Segment;
	[SerializeField, Tooltip("The number of Segments now (during runtime), and to begin with.")] uint NumberOfSegments;

	[Header("Centipede Movement Settings.")]
	[Min(Vector3.kEpsilon)] public float MovementSpeed = 150f;
	[Min(Vector3.kEpsilon)] public float TurnDegrees = 7f;

	[Header("Segment Settings.")]

	[Min(Vector3.kEpsilon)] public float FollowSpeed = 150f;
	[SerializeField, Min(Vector3.kEpsilon)] float FollowDistance = .2f;
	[Min(Vector3.kEpsilon)] public float MaxTurnDegreesPerFrame = 7f;
	[SerializeField, Tooltip("Any additional Segments that are not spawned in Constuct.")] List<MSegment> CustomSegments;

	public List<MSegment> Segments;
	SegmentsInformation SegmentsInfo;

	public float maxSpeed = 750;
	public float defaultSpeed = 150;

	[Space(10)]

	public GameObject DeathScreen;

	void Start()
	{
		Weapons = GetComponent<MCentipedeWeapons>();

		TailSegment = Tail.GetComponent<MSegment>();
		TailSegment.Initialise(Weapons, Head, FollowSpeed, MaxTurnDegreesPerFrame, FollowDistance);
		TailSegment.transform.parent = null;

		foreach (MSegment MS in CustomSegments)
		{
			MS.Initialise(Weapons, null, FollowSpeed, MaxTurnDegreesPerFrame, FollowDistance);
			MS.transform.localEulerAngles = Vector3.zero;
			MS.transform.parent = null;
		}

		Construct();
	}

	private void Update()
	{
		//Debug.Log(Segments.Count);
		//		Debug.Log(U2I(SegmentsInfo.End));
	}

	public MSegment AddSegment()
	{
		IncreaseSpeed(10);
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
			return AddedSegment;

		Debug.LogError("No Segment was added!");
		return null;
	}

	MSegment AddSegment(float Z, Quaternion Rot)
	{
		// Inherit Centipede's rotation.
		MSegment Seg = Instantiate(Segment, Vector3.zero, Rot);
		Seg.Initialise(Weapons, Segments.Count == 0 ? Head : GetLast(), FollowSpeed, MaxTurnDegreesPerFrame, FollowDistance);
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

		TailSegment.Initialise(Weapons, GetLast(), FollowSpeed, MaxTurnDegreesPerFrame, FollowDistance);

		return Seg;
	}

	public void RemoveSegment()//MSegment deadSegment)
	{
		DecreaseSpeed(10);
		Debug.Log("Killing Player");
		if (NumberOfSegments <= 1)
		{
			Debug.Log("You Died");
			DeathScreen.SetActive(true);
			Time.timeScale = 0;
			return;
		}

		MSegment lastSegment = GetLast();
		/*foreach (MSegment segment in Segments)
		{
			lastSegment = segment;
		}*/

		//Segments.Remove(Segments[Segments.Count - 1]);

		Destroy(lastSegment.gameObject);

		//int nextIndex = 1;
		//while (segmentIndex + nextIndex < Segments.Count - 1 && !Segments[segmentIndex + nextIndex]) //if multiple in a row get destroyed at same time, prevents it from bugging out
		//	nextIndex++;
		//if (segmentIndex + nextIndex < Segments.Count - 1)
		//	Segments[segmentIndex + nextIndex].ForwardNeighbour = lastSegment.ForwardNeighbour;

		Segments.RemoveAt(Segments.Count - 1);
		--NumberOfSegments;
		GameManager1.cameraController.camShake();

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

	public void IncreaseSpeed(float value)
	{
		if (FollowSpeed + value > maxSpeed)
		{
			SetSpeed(maxSpeed);
		}
		else
		{
			FollowSpeed += value;
			MovementSpeed += value;

			foreach (MSegment segment in Segments)
			{
				segment.FollowSpeed += value;
			}

			foreach (MSegment S in CustomSegments)
				S.FollowSpeed += value;

			TailSegment.FollowSpeed += value;
		}
	}

	public void SetSpeed(float value)
	{
		if (value > maxSpeed)
		{
			FollowSpeed = maxSpeed;

			foreach (MSegment segment in Segments)
			{
				segment.FollowSpeed = maxSpeed;
			}

			foreach (MSegment S in CustomSegments)
				S.FollowSpeed = maxSpeed;

			TailSegment.FollowSpeed = maxSpeed;
			return;
		}
		else
		{
			FollowSpeed = value;
			foreach (MSegment segment in Segments)
			{
				segment.FollowSpeed = value;
			}

			foreach (MSegment S in CustomSegments)
				S.FollowSpeed = value;

			TailSegment.FollowSpeed = value;
		}
	}

	public void DecreaseSpeed(float value)
	{
		if (FollowSpeed - value < 0)
		{
			SetSpeed(defaultSpeed);
		}
		else
		{
			FollowSpeed -= value;
			MovementSpeed -= value;

			foreach (MSegment segment in Segments)
			{
				segment.FollowSpeed -= value;
			}

			foreach (MSegment S in CustomSegments)
				S.FollowSpeed -= value;

			TailSegment.FollowSpeed -= value;
		}
	}
}