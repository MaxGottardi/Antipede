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

	[Header("Movement Settings.")]

	[SerializeField, Min(Vector3.kEpsilon)] public float FollowSpeed = 150f;
	[SerializeField, Min(Vector3.kEpsilon)] public float MaxTurnDegreesPerFrame = 7f;

	MCentipedeEvents Listener;

	List<MSegment> Segments;
	SegmentsInformation SegmentsInfo;

	public float maxSpeed;
	public float defaultSpeed;

	[Space(10)]

	public GameObject DeathScreen;

	void Start()
	{
		TailSegment = Tail.GetComponent<MSegment>();
		TailSegment.Initialise(Head, FollowSpeed, MaxTurnDegreesPerFrame, SegmentsInfo.TailScale.z);
		TailSegment.transform.parent = null;
		Construct();
		maxSpeed = 750;
		defaultSpeed = 150;

		Weapons = GetComponent<MCentipedeWeapons>();
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

			Tail.position += SegmentsInfo.SegmentScale.z * -End.forward;

			Quaternion Rot = MMathStatics.DirectionToQuat(((Transform)GetLast(1)).position, End.position);

			AddedSegment = AddSegment(Z, Rot);
		}
		else
		{
			Tail.position += SegmentsInfo.SegmentScale.z * -Head.forward;
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
		Seg.Initialise(Segments.Count == 0 ? Head : GetLast(), FollowSpeed, MaxTurnDegreesPerFrame, SegmentsInfo.SegmentScale.z);

		Seg.name = "Segment: " + SegmentsInfo.End;

		// Implicit conversion to Transform.
		Transform T = Seg;

		if (Segments.Count == 0)
		{
			// Set parent to the Centipede's object and inherit local position.
			T.SetParent(transform);
			T.localPosition = Head.localPosition - new Vector3(0, 0, Z);

			T.parent = null;
		}
		else
		{
			Transform End = GetLast();
			T.position = End.position - End.forward * SegmentsInfo.SegmentScale.z;
			T.LookAt(End);

			TailSegment.Initialise(End, FollowSpeed, MaxTurnDegreesPerFrame, SegmentsInfo.TailScale.z);
		}

		// Subscribe listening events.
		T.transform.GetComponent<MCentipedeSegmentEvents>().Initialise(Listener);

		Segments.Add(Seg);
		SegmentsInfo.AddSegment();

		return Seg;
	}

	public void RemoveSegment()
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

		Destroy(lastSegment.gameObject);
		//Segments.Remove(Segments[Segments.Count - 1]);
		Segments.RemoveAt(Segments.Count - 1);
		--NumberOfSegments;

		int lastSegIndex = Segments.Count - 1;
		TailSegment.SetForwardNeighbour(Segments[lastSegIndex]);

		SegmentsInfo.RemoveSegment();

		// Ensure the Tail is properly 'attached' to the end Segment.
		Transform newLast = GetLast();
		Tail.position = newLast.position - newLast.forward * SegmentsInfo.SegmentScale.z;
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
			foreach (MSegment segment in Segments)
			{
				segment.FollowSpeed += value;
			}
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
			foreach (MSegment segment in Segments)
			{
				segment.FollowSpeed -= value;
			}
			TailSegment.FollowSpeed -= value;
		}
	}
}