using UnityEngine;

[RequireComponent(typeof(MCentipedeSegmentEvents))]
public class MSegment : MonoBehaviour
{
	public Transform ForwardNeighbour;
	Rigidbody rb;
	public float FollowSpeed, MaxTurnDegreesPerFrame;
	float Distance;

	Weapon Weapon;

	/// <summary>Initialises this Segment to follow ForwardNeighbour at FollowSpeed and turning at MaxTurnDegreesPerFrame.</summary>
	/// <remarks>MaxTurnDegreesPerFrame will be multiplied to try and prevent disconnection. Increase as needed.</remarks>
	/// <param name="ForwardNeighbour">The Transform to follow when the body moves.</param>
	/// <param name="FollowSpeed">The speed to follow ForwardNeighbour.</param>
	/// <param name="MaxTurnDegreesPerFrame">How many DEGREES can this Segment turn towards ForwardNeighbour?</param>
	/// <param name="Distance">How close should this Segment go until it stops following ForwardNeighbour?</param>
	public void Initialise(Transform ForwardNeighbour, float FollowSpeed, float MaxTurnDegreesPerFrame, float Distance)
	{
		SetForwardNeighbour(ForwardNeighbour);
		rb = GetComponent<Rigidbody>();
		this.FollowSpeed = FollowSpeed;
		this.MaxTurnDegreesPerFrame = MaxTurnDegreesPerFrame * 20;
		this.Distance = Distance;
	}

	void FixedUpdate()
	{
		if (!MMathStatics.HasReached(transform.position, ForwardNeighbour.position, Distance))
		{
			MMathStatics.HomeTowards(rb, ForwardNeighbour, FollowSpeed, MaxTurnDegreesPerFrame);
		}
		else
		{
			rb.velocity = Vector3.zero;
		}
	}

	public void SetForwardNeighbour(Transform NewForwardNeighbour)
	{
		ForwardNeighbour = NewForwardNeighbour;
	}

	public void SetWeapon(Weapon Weapon)
	{
		Transform WeaponSocket = transform.Find("Weapon Attachment Socket");

		if (!WeaponSocket)
		{
			Debug.Log("No Weapon Socket attached onto this Segment: " + name);
			return;
		}

		Weapon AttachedWeapon = Instantiate(Weapon, WeaponSocket.position, Quaternion.identity);
		this.Weapon = AttachedWeapon;
		AttachedWeapon.transform.SetParent(WeaponSocket);
	}

	public static implicit operator Transform(MSegment s) => s.transform;
	public static implicit operator Weapon(MSegment s) => s.Weapon;
}
