using UnityEngine;

/// <summary>
/// The central class that handles Segment logic.
/// <br></br>
/// <br>Anything related to a Centipede's Segment can be accessed from here.</br>
/// <br><br>Includes:</br></br>
/// <br>The <see cref="MCentipedeWeapons"/> component this Segment belongs to. This is the Centipede.</br>
/// <br>The attached <see cref="global::Weapon"/>.</br>
/// <br>Handles Segment Following and Movement.</br>
/// </summary>
/// <remarks>
/// Not mentioned is the <see cref="FABRIK"/> component, which is unrelated to MSegment.
/// </remarks>
public class MSegment : MonoBehaviour
{
	static AnimationCurve AccelerationCurve;

	[Header("Follow Settings.")]
	[ReadOnly] public Transform ForwardNeighbour;
	[ReadOnly] public float FollowSpeed, MaxTurnDegreesPerFrame;
	Rigidbody rb;
	float Distance;
	CentipedeMovement Reference;
	float AccelerationTime = 0f;

	[Header("Weapons Settings.")]
	public bool bIgnoreFromWeapons;
	public Weapon Weapon;
	Transform WeaponSocket;

	public float health = 100;
	public int numAttacking = 0; //does this segment has a player locked on for attacking

	MCentipedeWeapons Owner;
	bool bDetached = false;
	float TimeDetached = 0f;

	/// <summary>Initialises this Segment to follow ForwardNeighbour at FollowSpeed and turning at MaxTurnDegreesPerFrame.</summary>
	/// <remarks>MaxTurnDegreesPerFrame will be multiplied to try and prevent disconnection. Increase as needed.</remarks>
	/// <param name="Owner">The Weapons Component this Segment belongs to. The Centipede.</param>
	/// <param name="Reference">The Centipede's Movement Component to judge the rate of acceleration and deceleration.</param>
	/// <param name="ForwardNeighbour">The Transform to follow when the body moves.</param>
	/// <param name="FollowSpeed">The speed to follow ForwardNeighbour.</param>
	/// <param name="MaxTurnDegreesPerFrame">How many DEGREES can this Segment turn towards ForwardNeighbour?</param>
	/// <param name="Distance">How close should this Segment go until it stops following ForwardNeighbour?</param>
	/// <param name="AccelerationCurve">The curve that defines the rate of acceleration towards <see cref="FollowSpeed"/>.</param>
	public void Initialise(MCentipedeWeapons Owner, CentipedeMovement Reference, Transform ForwardNeighbour, float FollowSpeed, float MaxTurnDegreesPerFrame, float Distance, AnimationCurve AccelerationCurve)
	{
		this.Owner = Owner;
		SetForwardNeighbour(ForwardNeighbour);
		rb = GetComponent<Rigidbody>();
		this.FollowSpeed = FollowSpeed;
		this.MaxTurnDegreesPerFrame = MaxTurnDegreesPerFrame;
		this.Distance = Distance;

		this.Reference = Reference;

		if (global::MSegment.AccelerationCurve == null)
			global::MSegment.AccelerationCurve = AccelerationCurve;

		WeaponSocket = transform.Find("Weapon Attachment Socket");
	}

	bool bHasRayAligned = false;
	Vector3 SurfaceNormal;

	/// <summary>Segment Movement.</summary>
	void FixedUpdate()
	{
		
		if (bDetached)
		{
			rb.AddTorque(transform.up * 50f);
			DetachGameFunctions();

			return;
		}

		if (!MMathStatics.HasReached(transform.position, ForwardNeighbour.position, Distance * 1.25f, out float SquareDistance))
		{
			if (ForwardNeighbour)
			{
				AccelerationTime = Reference.AccelerationTime;

				MMathStatics.HomeTowards(rb, ForwardNeighbour, EvaluateAcceleration(FollowSpeed), MaxTurnDegreesPerFrame);
				bHasRayAligned = false;

				AccelerationTime = Mathf.Clamp(AccelerationTime, Distance, 1f);
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
						transform.position = Hit.point + transform.up * Distance;
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

			AccelerationTime = 0f;
		}
	}

	float EvaluateAcceleration(float Scalar)
	{
		if (Reference.AccelerationTime < Vector3.kEpsilon)
			return 250; // Known safe speed for Segments and Centipede.

		float AccelRate = AccelerationCurve.Evaluate(AccelerationTime);

		return AccelRate * Scalar;
	}

	/// <summary>Override the <see cref="AccelerationTime"/>.</summary>
	/// <param name="Time">New Acceleration Time.</param>
	public void InjectAccelerationTime(float Time)
	{
		AccelerationTime = Time;
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

		Weapon AttachedWeapon = Instantiate(Weapon, WeaponSocket.position, WeaponSocket.rotation);
		this.Weapon = AttachedWeapon;
		this.Weapon.Owner = GetOwner();

		AttachedWeapon.transform.SetParent(WeaponSocket);
		AttachedWeapon.OnAttatch();

		Owner.SegmentsWithWeapons.Add(this);

		Owner.StopAllCoroutines();
	}

	public void ReplaceWeapon(Weapon NewWeapon)
	{
		Weapon WeaponNow = Weapon;

		// Deregister this Segment from Weapons.
		Deregister();

		Debug.Assert(WeaponNow != null, "Trying to replace a Segment's Weapon, but no Weapon is attached!");

		WeaponNow.transform.parent = null;

		Rigidbody WNRB = WeaponNow.gameObject.AddComponent<Rigidbody>();

		// Random force parameters.
		float RandomUAxis = Random.Range(0f, 1f);
		float RandomRAxis = Random.Range(-1f, 1f);

		// Calculate a random upwards force.
		Vector3 Force = transform.up * RandomUAxis + transform.right * RandomRAxis;
		Force.Normalize();
		Force *= 5f;

		// Ignore inertial forces from pre-detachment.
		WNRB.velocity = Vector3.zero;
		WNRB.useGravity = true;

		// Apply upwards force and random rotation.
		WNRB.AddForce(Force, ForceMode.VelocityChange);
		WNRB.transform.rotation = MMathStatics.V2Q(Random.onUnitSphere);
		WNRB.AddTorque(Force * 5000);

		// Enable physics collisions.
		WNRB.gameObject.AddComponent<BoxCollider>();

		WeaponNow.Deregister();
		Destroy(Weapon.gameObject, 5f);

		// Reregister this Segment into Weapons.
		SetWeapon(NewWeapon);
	}

	/// <param name="Socket">Outs the Weapon Socket.</param>
	/// <returns>True if there is a Weapon Socket.</returns>
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

	/// <summary>Destroy anything that makes this Segment an <see cref="MSegment"/>.</summary>
	void DetachGameFunctions()
	{
		// If this Rigidbody is no longer required to simulate physics,
		// destroy the Rigidbody and destroy the MSegment component.
		if (rb.IsSleeping() || Time.time - TimeDetached > 10f)
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
			Destroy(Replacement.GetComponent<Collider>());
		}
	}

	/// <summary>Visually fling this Segment off the Centipede line.</summary>
	public void Detach()
	{
		Deregister();
		bDetached = true;
		TimeDetached = Time.time;

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

		gameObject.tag = "Untagged";

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

	// Shorthand conversions.
	public static implicit operator Transform(MSegment s) => s.transform;
	public static implicit operator Weapon(MSegment s) => s.Weapon;
}
