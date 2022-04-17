using System.Collections.Generic;
using UnityEngine;

public partial class MCentipedeBody : MonoBehaviour
{
	/// <summary>The distances between Segments.</summary>
	float DeltaZ;

	void Awake()
	{
		Listener = GetComponent<MCentipedeEvents>();
		Head.GetComponent<MCentipedeSegmentEvents>().Initialise(Listener);
		Tail.GetComponent<MCentipedeSegmentEvents>().Initialise(Listener);
		SegmentsInfo = new SegmentsInformation(Head.localScale, Tail.localScale, ((Transform)Segment).localScale);
	}

	void Construct()
	{
		Segments = new List<MSegment>();

		// Set the positions of the segments.
		DeltaZ = (SegmentsInfo.HeadScale.z + SegmentsInfo.SegmentScale.z) * .5f;

		if (NumberOfSegments > 0)
		{
			// Set the position of the tail.
			Tail.position += ((NumberOfSegments + 1) * SegmentsInfo.SegmentScale.z - SegmentsInfo.SegmentScale.z) * -Head.forward;

			Quaternion ParentRotation = transform.rotation;

			for (int i = 0; i < NumberOfSegments; ++i)
			{
				float Z = i == 0 ? DeltaZ : i * SegmentsInfo.SegmentScale.z + DeltaZ;

				AddSegment(Z, ParentRotation);
			}
		}
		else
		{
			Tail.position = Head.position + (SegmentsInfo.TailScale.z * -Head.forward);
		}
	}
}

struct SegmentsInformation
{
	public Vector3 HeadScale;
	public Vector3 TailScale;
	public Vector3 SegmentScale;

	/// <summary>The index position of the last Segment.</summary>
	/// <remarks>Zero if there are no Segments.</remarks>
	public uint End
	{
		get
		{
			// Prevent unsigned int under/over flow.
			return Internal_End == 0 ? 0 : Internal_End - 1;
		}
		private set
		{
			Internal_End = value;
		}
	}

	internal uint Internal_End;

	public SegmentsInformation(Vector3 headScale, Vector3 tailScale, Vector3 segmentScale)
	{
		HeadScale = headScale;
		TailScale = tailScale;
		SegmentScale = segmentScale;
		Internal_End = 0;
	}

	public void AddSegment()
	{
		++Internal_End;
	}

	public void RemoveSegment()
    {
		--Internal_End;
    }
}
