using UnityEngine;

public class MSegment : MonoBehaviour
{
	public Transform ForwardNeighbour;
	Rigidbody rb;
	public float FollowSpeed, MaxTurnDegreesPerFrame;
	float Distance;
	//public bool beingAttacked = false;

	public bool bIgnoreFromWeapons;
	Transform WeaponSocket;
	Weapon Weapon;

	MCentipedeWeapons Owner;
	public float health = 100;

	bool bDetached = false;

	/// <summary>Initialises this Segment to follow ForwardNeighbour at FollowSpeed and turning at MaxTurnDegreesPerFrame.</summary>
	/// <remarks>MaxTurnDegreesPerFrame will be multiplied to try and prevent disconnection. Increase as needed.</remarks>
	/// <param name="ForwardNeighbour">The Transform to follow when the body moves.</param>
	/// <param name="FollowSpeed">The speed to follow ForwardNeighbour.</param>
	/// <param name="MaxTurnDegreesPerFrame">How many DEGREES can this Segment turn towards ForwardNeighbour?</param>
	/// <param name="Distance">How close should this Segment go until it stops following ForwardNeighbour?</param>
	public void Initialise(MCentipedeWeapons Owner, Transform ForwardNeighbour, float FollowSpeed, float MaxTurnDegreesPerFrame, float Distance)
	{
		this.Owner = Owner;
		SetForwardNeighbour(ForwardNeighbour);
		rb = GetComponent<Rigidbody>();
		this.FollowSpeed = FollowSpeed;
		this.MaxTurnDegreesPerFrame = MaxTurnDegreesPerFrame * 20;
		this.Distance = Distance;

		WeaponSocket = transform.Find("Weapon Attachment Socket");
	}

	bool bHasRayAligned = false;
	Vector3 SurfaceNormal;

	void FixedUpdate()
	{
		if (bDetached)
		{
			DetachGameFunctions();
			return;
		}

		if (!MMathStatics.HasReached(transform.position, ForwardNeighbour.position, Distance, out float SquareDistance))
		{
			if (ForwardNeighbour)
			{
				MMathStatics.HomeTowards(rb, ForwardNeighbour, FollowSpeed, MaxTurnDegreesPerFrame);
				bHasRayAligned = false;
			}
		}
		else
		{
			rb.velocity = Vector3.zero;

			if (SquareDistance < .5f)
			{
				if (!bHasRayAligned)
				{
					if (Physics.Raycast(transform.position, -transform.up, out RaycastHit Hit, 1, 256))
					{
						SurfaceNormal = Hit.normal;
					}
					else
					{
						SurfaceNormal = Vector3.zero;
					}

					bHasRayAligned = true;
				}

				if (bHasRayAligned && SurfaceNormal != Vector3.zero)
				{
					transform.rotation = Quaternion.Slerp(transform.rotation,
						Quaternion.FromToRotation(transform.up, SurfaceNormal) * transform.rotation, .3f);
				}
				else
				{
					Vector3 ThisRot = transform.eulerAngles;
					Transform Target = ForwardNeighbour.parent ?? ForwardNeighbour;
					Vector3 TargetRot = new Vector3(ThisRot.x, ThisRot.y, Target.localEulerAngles.z);

					Quaternion To = Quaternion.Euler(TargetRot);

					transform.rotation = Quaternion.Slerp(transform.rotation, To, .3f);
				}
			}
		}
	}

	public void SetForwardNeighbour(Transform NewForwardNeighbour)
	{
		if (NewForwardNeighbour != null)
		{
			ForwardNeighbour = NewForwardNeighbour;
		}
	}

	public void SetWeapon(Weapon Weapon)
	{
		if (bIgnoreFromWeapons)
			return;

		if (!WeaponSocket)
		{
			Debug.LogError("No Weapon Socket attached onto this Segment: " + name);
			return;
		}

		Weapon AttachedWeapon = Instantiate(Weapon, WeaponSocket.position, Quaternion.identity);
		this.Weapon = AttachedWeapon;
		this.Weapon.Owner = GetOwner();

		AttachedWeapon.transform.SetParent(WeaponSocket);

		Owner.SegmentsWithWeapons.Add(this);
	}

	public bool TryGetWeaponSocket(out Transform Socket)
	{
		Socket = WeaponSocket;
		return Socket;
	}

	public MCentipedeWeapons GetOwner()
	{
		return Owner;
	}

	public bool ReduceHealth(float amount)
	{
		health -= amount;
		return health <= 0;
	}

	void DetachGameFunctions()
	{
		// If this Rigidbody is no longer required to simulate physics,
		// destroy the Rigidbody and destroy the MSegment component.
		if (rb.IsSleeping())
		{
			// Mark this (the actual Segment that *was* attached) for destruction.
			Destroy(gameObject);

			// Duplicate this Game Object.
			Transform T = transform;
			Transform Replacement = Instantiate(T, T.position, T.rotation);
			Replacement.name = name + " (Detached)";

			// Remove core components from this duplicated Segment.
			Destroy(Replacement.GetComponent<Rigidbody>());
			Destroy(Replacement.GetComponent<MSegment>());
		}
	}

	public void Detach()
	{
		Deregister();
		bDetached = true;

		// Random force parameters.
		float RandomUAxis = Random.Range(0f, 1f);
		float RandomRAxis = Random.Range(-1f, 1f);

		// Calculate a random upwards force.
		Vector3 Force = transform.up * RandomUAxis + transform.right * RandomRAxis;
		Force.Normalize();
		Force *= 5f;

		// Ignore inertial forces from pre-detachment.
		rb.velocity = Vector3.zero;
		rb.useGravity = true;

		// Apply upwards force and random rotation.
		rb.AddForce(Force, ForceMode.VelocityChange);
		transform.rotation = MMathStatics.V2Q(Random.onUnitSphere);

		// Enable physics collisions.
		GetComponent<Collider>().isTrigger = false;

		// Disable FABRIK.
		Destroy(GetComponent<FABRIK>());

		// Deregister and ignore Weapon commands (if any).
		if (Weapon)
			Weapon.Deregister();
	}

	/// <summary>Make this Segment ignore Weapon commands.</summary>
	void Deregister()
	{
		if (Owner)
			// If this doesn't fix the InvalidOperationException problem, then idk.
			Owner.StopAllCoroutines();

		Owner.SegmentsWithWeapons.Remove(this);
	}

	public static implicit operator Transform(MSegment s) => s.transform;
	public static implicit operator Weapon(MSegment s) => s.Weapon;
}
