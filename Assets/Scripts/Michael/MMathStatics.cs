using UnityEngine;

public static class MMathStatics
{
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

	public static bool HasReached(Vector3 Location, Vector3 Target, float ToleranceInUnits)
	{
		float SqrDistance = Mathf.Abs((Target - Location).sqrMagnitude);
		return SqrDistance <= ToleranceInUnits + Vector3.kEpsilon;
	}

	public static void HomeTowards(Rigidbody Rigidbody, Transform Target, float Velocity, float MaxDegreesDeltaPerFrame)
	{
		HomeTowards(Rigidbody, Target.position, Velocity, MaxDegreesDeltaPerFrame);
	}

	public static void HomeTowards(Rigidbody Rigidbody, Vector3 Target, float Velocity, float MaxDegreesDeltaPerFrame)
	{
		Transform _self = Rigidbody.transform;
		Rigidbody.velocity = _self.forward * Velocity * Time.deltaTime;
		Rigidbody.MoveRotation(Quaternion.RotateTowards(_self.rotation, Quaternion.LookRotation(Target - _self.position, _self.up), MaxDegreesDeltaPerFrame));
	}
}
