using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MCentipedeEvents))]
public partial class MCentipedeBody : MonoBehaviour
{

	[SerializeField] Transform Head, Tail;
	MSegment TailSegment;

	[SerializeField] MSegment Segment;
	[SerializeField, Tooltip("The number of Segments now (during runtime), and to begin with.")] uint NumberOfSegments;
	float DeltaZ;

	[SerializeField, Min(Vector3.kEpsilon)] public float FollowSpeed = 150f;
	[SerializeField, Min(Vector3.kEpsilon)] public float MaxTurnDegreesPerFrame = 7f;

	MCentipedeEvents Listener;

	List<MSegment> Segments;
	SegmentsInformation SegmentsInfo;

	public float maxSpeed;
	public float defaultSpeed;

	void Start()
	{
		TailSegment = Tail.GetComponent<MSegment>();
		TailSegment.Initialise(Head, FollowSpeed, MaxTurnDegreesPerFrame, SegmentsInfo.TailScale.z * SegmentsInfo.TailScale.z);
		TailSegment.transform.parent = null;
		Construct();
		maxSpeed = 750;
		defaultSpeed = 150;
	}

	public void AddSegment()
	{
		float Z = NumberOfSegments * SegmentsInfo.SegmentScale.z + DeltaZ;
		if (SegmentsInfo.End > 0)
		{
			Transform End = this[U2I(SegmentsInfo.End)];

			Tail.position += SegmentsInfo.SegmentScale.z * -Head.forward;

			Quaternion Rot = MMathStatics.DirectionToQuat(((Transform)this[U2I(SegmentsInfo.End - 1)]).position, End.position);

			AddSegment(Z, Rot);
		}
		else
		{
			Tail.position += SegmentsInfo.SegmentScale.z * -Head.forward;
			AddSegment(Z, Head.rotation);
		}

		++NumberOfSegments;
	}

	void AddSegment(float Z, Quaternion Rot)
	{
		// Inherit Centipede's rotation.
		MSegment Seg = Instantiate(Segment, Vector3.zero, Rot);
		Seg.Initialise(Segments.Count == 0 ? Head : this[SegmentsInfo.End], FollowSpeed, MaxTurnDegreesPerFrame, SegmentsInfo.SegmentScale.z * SegmentsInfo.SegmentScale.z);

		Seg.name = "Segment: " + SegmentsInfo.End;

		// Implicit conversion to Transform.
		Transform T = Seg;

		// Set parent to the Centipede's object and inherit local position.
		if (Segments.Count == 0)
		{
			T.SetParent(transform);
			T.localPosition = Head.localPosition - new Vector3(0, 0, Z);
			T.parent = null;
		}
		else
		{
			Transform End = ((Transform)this[SegmentsInfo.End]);
			T.position = End.position - End.forward * SegmentsInfo.SegmentScale.z;
			T.LookAt(End);

			TailSegment.Initialise(Segments.Count == 0 ? Head : this[SegmentsInfo.End], FollowSpeed, MaxTurnDegreesPerFrame, SegmentsInfo.TailScale.z * SegmentsInfo.TailScale.z);
		}

		// Subscribe listening events.
		T.transform.GetComponent<MCentipedeSegmentEvents>().Initialise(Listener);

		Segments.Add(Seg);
		SegmentsInfo.AddSegment();
	}

	public void RemoveSegment()
    {
		if (NumberOfSegments <= 1)
        {
			return;
        }

		MSegment lastSegment = Segments[0];
		foreach (MSegment segment in Segments)
        {
			lastSegment = segment;
        }

		Destroy(lastSegment.gameObject);
		Segments.Remove(lastSegment);
		--NumberOfSegments;

		int lastSegIndex = Segments.Count - 1;
		TailSegment.Initialise(Segments[lastSegIndex], FollowSpeed, MaxTurnDegreesPerFrame, 
		SegmentsInfo.TailScale.z * SegmentsInfo.TailScale.z);
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

	int U2I(uint U)
	{
		return Convert.ToInt32(U);
	}

	public MSegment this[int i] => Segments[i];
	public MSegment this[uint i] => Segments[U2I(i)];
}