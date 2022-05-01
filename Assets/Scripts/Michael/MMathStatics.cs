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

	public static int FPS() => (int)(1f / Time.unscaledDeltaTime);

	/// <summary>Rotates a vector at an angle of AngleDegrees around Axis.</summary>
	/// <param name="AngleDegrees">The degrees at which to rotate this MVector.</param>
	/// <param name="Axis">The axis to rotate this vector around.</param>
	public static Vector3 RotateAngleAxis(Vector3 Point, float AngleDegrees, Vector3 Axis)
	{
		SinCos(out float S, out float C, AngleDegrees * Mathf.Deg2Rad);

		float XX = Axis.x * Axis.x;
		float YY = Axis.y * Axis.y;
		float ZZ = Axis.z * Axis.z;

		float XY = Axis.x * Axis.y;
		float YZ = Axis.y * Axis.z;
		float ZX = Axis.z * Axis.x;

		float XS = Axis.x * S;
		float YS = Axis.y * S;
		float ZS = Axis.z * S;

		float OMC = 1f - C;

		return new Vector3(
			(OMC * XX + C) * Point.x + (OMC * XY - ZS) * Point.y + (OMC * ZX + YS) * Point.z,
			(OMC * XY + ZS) * Point.x + (OMC * YY + C) * Point.y + (OMC * YZ - XS) * Point.z,
			(OMC * ZX - YS) * Point.x + (OMC * YZ + XS) * Point.y + (OMC * ZZ + C) * Point.z
		);
	}


	/// <summary>The 11-Degree Minimax Approximation Sine and 10-Degree Minimax Approximation Cosine over an angle.</summary>
	/// <param name="Sine">The Sine result over fValue.</param>
	/// <param name="Cosine">The Cosine result over fValue.</param>
	/// <param name="Value">The angle.</param>
	public static void SinCos(out float Sine, out float Cosine, float Value)
	{
		const float kHalfPI = Mathf.PI * .5f;
		const float kInversePI = 1 / Mathf.PI;

		float Quotient = (kInversePI * 0.5f) * Value;
		if (Value >= 0.0f)
		{
			Quotient = (int)(Quotient + 0.5f);
		}
		else
		{
			Quotient = (int)(Quotient - 0.5f);
		}
		float Y = Value - (2.0f * Mathf.PI) * Quotient;

		// Map Y to [-PI / 2, PI / 2] with Sin(y) = Sin(Value).
		float Sign;
		if (Y > kHalfPI)
		{
			Y = Mathf.PI - Y;
			Sign = -1.0f;
		}
		else if (Y < -kHalfPI)
		{
			Y = -Mathf.PI - Y;
			Sign = -1.0f;
		}
		else
		{
			Sign = +1.0f;
		}

		float y2 = Y * Y;

		// 11-degree minimax approximation
		Sine = (((((-2.3889859e-08f * y2 + 2.7525562e-06f) * y2 - 0.00019840874f) * y2 + 0.0083333310f) * y2 - 0.16666667f) * y2 + 1.0f) * Y;

		// 10-degree minimax approximation
		float p = ((((-2.6051615e-07f * y2 + 2.4760495e-05f) * y2 - 0.0013888378f) * y2 + 0.041666638f) * y2 - 0.5f) * y2 + 1.0f;
		Cosine = Sign * p;
	}

	public static Vector3 ComponentWiseMul(Vector3 L, Vector3 R)
	{
		return new Vector3
		{
			x = L.x * R.x,
			y = L.y * R.y,
			z = L.z * R.z,
		};
	}

}
