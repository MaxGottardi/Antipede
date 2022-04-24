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
		return Quaternion.Euler(V2PYR(V));
	}

	public static Vector3 V2PYR(Vector3 V)
	{
		Vector3 EulerRadians = new Vector3
		{
			x = Mathf.Asin(V.y),
			y = Mathf.Atan2(V.x, V.z),
			z = 0
		};

		return EulerRadians * Mathf.Rad2Deg;
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

	/// <param name="Location">The location of the GameObject.</param>
	/// <param name="Target">The location to test whether the GameObject has reached.</param>
	/// <param name="ToleranceInUnits">The distance to consider 'reached'.</param>
	/// <param name="SquareDistance">Outs the square distance that was calculated.</param>
	/// <returns>True if the square distance between Location and Target &lt;= ToleranceInSquareUnits.</returns>
	public static bool HasReached(Vector3 Location, Vector3 Target, float ToleranceInUnits, out float SquareDistance)
	{
		float SqrDistance = Mathf.Abs((Target - Location).sqrMagnitude);
		SquareDistance = SqrDistance;
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

	public static Vector3 Abs(Vector3 In)
	{
		Abs(ref In);

		return In;
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

	/// <summary>Computes a velocity to launch a Rigidbody From To achieving a TargetHeight.</summary>
	/// <remarks>Will not compute if TargetHeight cannot reach To.y. <see cref="float.NaN"/> will be returned instead.</remarks>
	/// <param name="From">Where to launch from.</param>
	/// <param name="To">Where to launch to.</param>
	/// <param name="TargetHeight">The apex.</param>
	/// <param name="bLaunchRegardless">
	/// True to ignore the height limitation and compute a velocity anyway.
	/// May fail or be inaccurate.
	/// </param>
	/// <returns>The velocity required to launch a projectile From to To, or NaN if impossible.</returns>
	public static Vector3 ComputeLaunchVelocity(Vector3 From, Vector3 To, float TargetHeight, bool bLaunchRegardless = false)
	{
		float DeltaY = To.y - From.y;

		if (DeltaY > TargetHeight)
		{
			if (!bLaunchRegardless)
			{
				// Cannot launch a projectile at a height > target height.
				return new Vector3(float.NaN, float.NaN);
			}
			else
			{
				TargetHeight += DeltaY;
			}
		}

		// v -> { Δx / (sqrt(-2 / g) + sqrt(Δy + h / g)), sqrt(-2 * g * h) }

		Vector3 DeltaXZ = new Vector3(To.x - From.x, 0, To.z - From.z);
		float Gravity = Physics.gravity.y;
		Vector3 VY = Vector3.up * Mathf.Sqrt(-2f * Gravity * TargetHeight);
		Vector3 VXZ = DeltaXZ / (Mathf.Sqrt(-2f * TargetHeight / Gravity) + Mathf.Sqrt(2 * (DeltaY - TargetHeight) / Gravity));

		Vector3 LaunchVelocity = VXZ + VY * -Mathf.Sign(Gravity);
		return LaunchVelocity;
	}

	public static bool DiagnosticCheckNaN(Vector3 V)
	{
		return DiagnosticCheckNaN(V.x) || DiagnosticCheckNaN(V.y) || DiagnosticCheckNaN(V.z);
	}

	public static bool DiagnosticCheckNaN(float F)
	{
		return float.IsNaN(F);
	}
}
