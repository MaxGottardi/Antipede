using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MCentipedeSegmentEvents))]
public class MSegment : MonoBehaviour
{
	public Transform ForwardNeighbour;
	Rigidbody rb;
	float FollowSpeed, MaxTurnDegreesPerFrame;
	float Tolerance;

	public void Initialise(Transform forwardNeighbour, float FollowSpeed, float MaxTurnDegreesPerFrame, float Tolerance)
	{
		ForwardNeighbour = forwardNeighbour;
		rb = GetComponent<Rigidbody>();
		this.FollowSpeed = FollowSpeed;
		this.MaxTurnDegreesPerFrame = MaxTurnDegreesPerFrame;
		this.Tolerance = Tolerance;
	}

	void FixedUpdate()
	{
		if (!MMathStatics.HasReached(transform.position, ForwardNeighbour.position, Tolerance))
		{
			MMathStatics.HomeTowards(rb, ForwardNeighbour, FollowSpeed, MaxTurnDegreesPerFrame);
		}
		else
		{
			rb.velocity = Vector3.zero;
		}
	}

	public static implicit operator Transform(MSegment s) => s.transform;
}
