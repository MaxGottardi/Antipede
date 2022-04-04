using System;
using UnityEngine;

[RequireComponent(typeof(MCentipedeBody))]
public class MCentipedeEvents : MonoBehaviour
{
	/// <summary>The delegate to call when a Segment of the Centipede collides with something.</summary>
	public Action<Collider> OnSegmentTriggerEnter;

	MCentipedeBody body;

	void Awake()
	{
		body = GetComponent<MCentipedeBody>();
		OnSegmentTriggerEnter += OnTriggerEnter;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Add Segment"))
		{
			body.AddSegment();
		}
	}

	void OnDestroy()
	{
		OnSegmentTriggerEnter -= null;
	}
}
