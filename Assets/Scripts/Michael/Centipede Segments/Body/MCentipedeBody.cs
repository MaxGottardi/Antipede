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

	void Start()
	{
		TailSegment = Tail.GetComponent<MSegment>();
		TailSegment.Initialise(Head, FollowSpeed, MaxTurnDegreesPerFrame, SegmentsInfo.TailScale.z * SegmentsInfo.TailScale.z);
		TailSegment.transform.parent = null;
		Construct();
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
		MSegment lastSegment = Segments[0];
		foreach (MSegment segment in Segments)
        {
			lastSegment = segment;
        }
		Destroy(lastSegment.gameObject);
		Segments.Remove(lastSegment);
		--NumberOfSegments;
	}

	int U2I(uint U)
	{
		return Convert.ToInt32(U);
	}

	public MSegment this[int i] => Segments[i];
	public MSegment this[uint i] => Segments[U2I(i)];
}