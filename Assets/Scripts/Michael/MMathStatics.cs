using UnityEngine;

/**
 * Custom Math Library, feel free to add.
 */
public static class MMathStatics
{
	/// <summary>Converts a normalised vector direction to a Quaternion rotation</summary>
	/// <remarks>Roll (Z) cannot be calculated from a direction vector.</remarks>
	/// <param name="From">Origin</param>
	/// <param name="To">Target</param>
	/// <returns>Quaternion rotation from to.</returns>
	public static Quaternion DirectionToQuat(Vector3 From, Vector3 To)
	{
		Vector3 Direction = (To - From).normalized;

		return V2Q(Direction);
	}

	/// <summary>Converts a normalised vector to a Quaternion rotation.</summary>
	/// <remarks>Roll (Z) cannot be calculated from a direction vector.</remarks>
	/// <param name="V">Normalised direction vector.</param>
	/// <returns>Quaternion rotation without roll.</returns>
	public static Quaternion V2Q(Vector3 V)
	{
		Vector3 EulerRadians = new Vector3
		{
			x = Mathf.Asin(V.y),
			y = Mathf.Atan2(V.x, V.z),
			z = 0
		};

		return Quaternion.Euler(EulerRadians * Mathf.Rad2Deg);
	}

	/// <param name="Location">The location of the GameObject.</param>
	/// <param name="Target">The location to test whether the GameObject has reached.</param>
	/// <param name="ToleranceInUnits">The distance to consider 'reached'.</param>
	/// <returns>True if the square distance between Location and Target &lt;= ToleranceInSquareUnits.</returns>
	public static bool HasReached(Vector3 Location, Vector3 Target, float ToleranceInUnits)
	{
		float SqrDistance = Mathf.Abs((Target - Location).sqrMagnitude);
		return SqrDistance <= ((ToleranceInUnits * ToleranceInUnits) + Vector3.kEpsilon);
	}

	/// <summary>Moves a Rigidbody towards Target moving at Velocity and turning at MaxDegreesDeltaPerFrame.</summary>
	/// <param name="Rigidbody">The body to move.</param>
	/// <param name="Target">The position to home towards.</param>
	/// <param name="Velocity">The speed to move towards Target.</param>
	/// <param name="MaxDegreesDeltaPerFrame">The degrees this body can turn per frame.</param>
	public static void HomeTowards(Rigidbody Rigidbody, Transform Target, float Velocity, float MaxDegreesDeltaPerFrame)
	{
		HomeTowards(Rigidbody, Target.position, Velocity, MaxDegreesDeltaPerFrame);
	}

	/// <inheritdoc cref="HomeTowards(Rigidbody, Transform, float, float)"/>
	public static void HomeTowards(Rigidbody Rigidbody, Vector3 Target, float Velocity, float MaxDegreesDeltaPerFrame)
	{
		if (Target == Vector3.zero)
			return;

		Transform _self = Rigidbody.transform;
		Rigidbody.velocity = _self.forward * Velocity * Time.deltaTime;

		Vector3 LookForwardTarget = Target - _self.position;

		if (CheckZeroVector(LookForwardTarget))
			return;

		Rigidbody.MoveRotation(Quaternion.RotateTowards(_self.rotation, Quaternion.LookRotation(LookForwardTarget, _self.up), MaxDegreesDeltaPerFrame));
	}

	public static void Abs(ref Vector3 In)
	{
		In.x = Mathf.Abs(In.x);
		In.y = Mathf.Abs(In.y);
		In.z = Mathf.Abs(In.z);
	}

	/// <summary>Checks if a vector is close enough to zero.</summary>
	/// <param name="In"></param>
	/// <returns></returns>
	public static bool CheckZeroVector(Vector3 In)
	{
		bool bIsZero = In == Vector3.zero;

		Abs(ref In);

		return bIsZero || In.x < Vector3.kEpsilon && In.y < Vector3.kEpsilon && In.z < Vector3.kEpsilon;
	}
}
