using UnityEngine;

public class MSegment : MonoBehaviour
{
	public Transform ForwardNeighbour;
	Rigidbody rb;
	public float FollowSpeed, MaxTurnDegreesPerFrame;
	float Distance;

	public bool bIgnoreFromWeapons;
	Transform WeaponSocket;
	Weapon Weapon;

	MCentipedeWeapons Owner;

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

	void FixedUpdate()
	{
		if (!MMathStatics.HasReached(transform.position, ForwardNeighbour.position, Distance, out float SquareDistance))
		{
			MMathStatics.HomeTowards(rb, ForwardNeighbour, FollowSpeed, MaxTurnDegreesPerFrame);
		}
		else
		{
			rb.velocity = Vector3.zero;

			if (SquareDistance < .5f)
			{
				Vector3 ThisRot = transform.localEulerAngles;
				Vector3 TargetRot = new Vector3(ThisRot.x, ThisRot.y, ForwardNeighbour.eulerAngles.z);

				Quaternion To = Quaternion.Euler(TargetRot);

				transform.rotation = Quaternion.Slerp(transform.rotation, To, .3f);
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
		AttachedWeapon.transform.SetParent(WeaponSocket);

		Owner.SegmentsWithWeapons.Add(this);
	}

	public bool TryGetWeaponSocket(out Transform Socket)
	{
		Socket = WeaponSocket;
		return Socket;
	}

	void OnDestroy()
	{
		Owner.SegmentsWithWeapons.Remove(this);
	}

	public static implicit operator Transform(MSegment s) => s.transform;
	public static implicit operator Weapon(MSegment s) => s.Weapon;
}
