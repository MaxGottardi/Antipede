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

	/// <summary>Converts V to Pitch Yaw Roll.</summary>
	/// <remarks>Roll (Z) cannot be calculated from a direction vector.</remarks>
	/// <param name="V">Normalised direction vector.</param>
	/// <returns>X = Pitch, Y = Yaw, Z = Roll = 0.</returns>
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

	/// <summary>A fast <see cref="Vector3.Distance(Vector3, Vector3)"/> check against a known value: ToleranceInUnits.</summary>
	/// <param name="Location">The location of the GameObject.</param>
	/// <param name="Target">The location to test whether the GameObject has reached.</param>
	/// <param name="ToleranceInUnits">The distance to consider 'reached'.</param>
	/// <returns>True if the square distance between Location and Target &lt;= ToleranceInSquareUnits.</returns>
	public static bool HasReached(Vector3 Location, Vector3 Target, float ToleranceInUnits)
	{
		float SqrDistance = (Target - Location).sqrMagnitude;
		return SqrDistance <= ((ToleranceInUnits * ToleranceInUnits) + Vector3.kEpsilon);
	}

	/// <summary>A fast <see cref="Vector3.Distance(Vector3, Vector3)"/> check against a known value: ToleranceInUnits.</summary>
	/// <param name="Location">The location of the GameObject.</param>
	/// <param name="Target">The location to test whether the GameObject has reached.</param>
	/// <param name="ToleranceInUnits">The distance to consider 'reached'.</param>
	/// <param name="SquareDistance">Outs the square distance that was calculated.</param>
	/// <returns>True if the square distance between Location and Target &lt;= ToleranceInUnits.</returns>
	public static bool HasReached(Vector3 Location, Vector3 Target, float ToleranceInUnits, out float SquareDistance)
	{
		float SqrDistance = (Target - Location).sqrMagnitude;
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
	/// <returns>True if all components are &lt; <see cref="Vector3.kEpsilon"/>.</returns>
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
		return ComputeLaunchVelocity(From, To, TargetHeight, out _, bLaunchRegardless);
	}

	/// <inheritdoc cref="ComputeLaunchVelocity(Vector3, Vector3, float, bool)"/>
	/// <param name="Time">The time to reach To.</param>
	public static Vector3 ComputeLaunchVelocity(Vector3 From, Vector3 To, float TargetHeight, out float Time, bool bLaunchRegardless = false)
	{
		float DeltaY = To.y - From.y;

		if (DeltaY > TargetHeight)
		{
			if (!bLaunchRegardless)
			{
				// Cannot launch a projectile at a height > target height.
				Time = float.NaN;
				return new Vector3(float.NaN, float.NaN);
			}
			else
			{
				TargetHeight = DeltaY;
			}
		}

		// v -> { Δx / (sqrt(-2 / g) + sqrt(Δy + h / g)), sqrt(-2 * g * h) }

		Vector3 DeltaXZ = new Vector3(To.x - From.x, 0, To.z - From.z);
		float Gravity = Physics.gravity.y;
		float InverseG = 1 / Gravity;
		Vector3 VY = Vector3.up * Mathf.Sqrt(-2f * Gravity * TargetHeight);
		Time = FastSqrt(-2f * TargetHeight * InverseG) + FastSqrt(2 * (DeltaY - TargetHeight) * InverseG);
		Vector3 VXZ = DeltaXZ / Time;

		Vector3 LaunchVelocity = VXZ + VY * -Mathf.Sign(Gravity);
		return LaunchVelocity;
	}

	/// <summary>1 / sqrt(N).</summary>
	/// <remarks>Slightly modified from: <see href="https://github.com/id-Software/Quake-III-Arena/blob/dbe4ddb10315479fc00086f08e25d968b4b43c49/code/game/q_math.c#L552"/></remarks>
	/// <param name="N">1 / sqrt(x) where x is N.</param>
	/// <returns>1 / sqrt(N) within 2% accuracy.</returns>
	public static unsafe float FastInverseSqrt(float N)
	{
		int F = *(int*)&N;
		F = 0x5F3759DF - (F >> 1);
		float X = *(float*)&F;

		return X * (1.5f - .5f * N * X * X);
	}

	/// <summary>Faster version of <see cref="Mathf.Sqrt(float)"/>.</summary>
	/// <param name="F"></param>
	/// <returns><see cref="FastInverseSqrt(float)"/> with other fancy tricks.</returns>
	public static float FastSqrt(float F) => FastInverseSqrt(Mathf.Max(F, Vector3.kEpsilon)) * F;

	public static bool DiagnosticCheckNaN(Vector3 V)
	{
		return DiagnosticCheckNaN(V.x) || DiagnosticCheckNaN(V.y) || DiagnosticCheckNaN(V.z);
	}

	public static bool DiagnosticCheckNaN(float F)
	{
		return float.IsNaN(F);
	}

	public static int FPS() => (int)(1f / Time.unscaledDeltaTime);
}
