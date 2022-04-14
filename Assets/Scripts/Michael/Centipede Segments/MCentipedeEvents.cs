using System;
using UnityEngine;

[RequireComponent(typeof(MCentipedeBody))]
public class MCentipedeEvents : MonoBehaviour
{
	/// <summary>The delegate to call when a Segment of the Centipede collides with something.</summary>
	public Action<Collider> OnSegmentTriggerEnter;

	MCentipedeBody Body;

	void Awake()
	{
		Body = GetComponent<MCentipedeBody>();
		OnSegmentTriggerEnter += OnTriggerEnter;
	}

	void OnTriggerEnter(Collider other)
	{
		// Handle Centipede Trigger Entries here...

		if (other.CompareTag("Weapon Pickup"))
		{
			MSegment Added = Body.AddSegment();
			WeaponPickup PickedUp = other.GetComponentInParent<WeaponPickup>();

			if (PickedUp != null)
			{
				Added.SetWeapon(PickedUp.Pickup);
			}
			else
			{
				Debug.LogError("Weapon Pickup has no WeaponPickup Component: " + other.name);
			}

			Destroy(other.gameObject);
		}
	}

	void OnDestroy()
	{
		// Garbage collection.

		OnSegmentTriggerEnter = null;
	}
}
