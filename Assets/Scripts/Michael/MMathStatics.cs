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
		Vector3 EulerRadians = new Vector3
		{
			x = Mathf.Asin(Direction.y),
			y = Mathf.Atan2(Direction.x, Direction.z),
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
		Transform _self = Rigidbody.transform;
		Rigidbody.velocity = _self.forward * Velocity * Time.deltaTime;
		Rigidbody.MoveRotation(Quaternion.RotateTowards(_self.rotation, Quaternion.LookRotation(Target - _self.position, _self.up), MaxDegreesDeltaPerFrame));
	}
}
