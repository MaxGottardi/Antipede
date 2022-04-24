using UnityEngine;

public class SpringArm : MonoBehaviour
{
	[Header("Target Settings.")]
	[SerializeField] Transform Camera;
	[SerializeField] Transform Target;
	[SerializeField] Vector3 TargetOffset;

	[Header("Spring Arm Settings.")]
	[SerializeField] float Distance;
	[SerializeField] Vector3 GimbalRotation;
	[SerializeField] Vector3 CameraRotation;
	[SerializeField] bool bInheritRotation;

	[Header("Collisions")]
	[SerializeField] LayerMask IgnoreFromCollisions;

	[Header("Lag Settings")]
	[SerializeField] bool bUseLag;
	[SerializeField] float PositionalLagStrength = .2f;
	[SerializeField] float RotationalLagStrength = .2f;
	Vector3 TargetPosition;
	Quaternion TargetRotation;
	float t = 0;

	void Update()
	{
		PlaceCamera();
	}

	void FixedUpdate()
	{
		if (bUseLag)
		{
			if (t <= 1f)
			{
				Camera.position = Vector3.Lerp(Camera.position, TargetPosition, PositionalLagStrength);

				if (bInheritRotation)
					Camera.rotation = Quaternion.Slerp(Camera.rotation, TargetRotation, RotationalLagStrength);
			}
		}
	}

	void PlaceCamera()
	{

		float VerticalOrbit = GimbalRotation.x;
		float HorizontalOrbit = -GimbalRotation.y * Mathf.Deg2Rad;

		if (bInheritRotation)
		{
			// Inherit the Target's Yaw.
			VerticalOrbit += Target.localEulerAngles.y;
			CameraRotation.y = Target.localEulerAngles.y;
		}
		else
		{
			CameraRotation.y = 0;
		}

		VerticalOrbit *= Mathf.Deg2Rad;

		// Convert Angles to Vectors.
		Vector3 Ground = new Vector3(Mathf.Sin(VerticalOrbit), 0, Mathf.Cos(VerticalOrbit));
		Vector3 Up = new Vector3(0, Mathf.Sin(HorizontalOrbit), Mathf.Cos(HorizontalOrbit));
		Vector3 XY = new Vector3(Ground.x, Up.y, Ground.z).normalized;

		// If the Spring Arm will collider with something:
		if (RunCollisionsCheck(ref XY))
			return;

		Vector3 FinalPosition = TargetPos() - (Distance * XY);
		Quaternion FinalRotation = Quaternion.Euler(CameraRotation);

		SetPositionAndRotation(FinalPosition, FinalRotation);

		// Debug.DrawLine(Target.position, Target.position + -Ground * Distance, Color.red);
		// Debug.DrawLine(Target.position, Target.position + -Up * Distance, Color.green);
		// Debug.DrawLine(Target.position, Target.position + -XY, Color.yellow);
	}

	bool RunCollisionsCheck(ref Vector3 Direction)
	{
		Vector3 TP = TargetPos();
		Ray FOV = new Ray(TP, -Direction);
		bool bViewToTargetBlocked = Physics.Raycast(FOV, out RaycastHit Hit, Distance, ~IgnoreFromCollisions);

		if (bViewToTargetBlocked)
		{
			Vector3 Point = Hit.point - FOV.direction;
			SetPositionAndRotation(Point, Quaternion.Euler(CameraRotation));
		}

		return bViewToTargetBlocked;
	}

	void SetPositionAndRotation(Vector3 FinalPosition, Quaternion FinalRotation)
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			Camera.position = FinalPosition;
			Camera.rotation = FinalRotation;
			return;
		}
#endif

		if (!bUseLag)
		{
			Camera.position = FinalPosition;
			Camera.rotation = FinalRotation;
		}
		else
		{
			if (TargetPosition != FinalPosition || TargetRotation != FinalRotation)
			{
				TargetPosition = FinalPosition;
				TargetRotation = FinalRotation;
				t = 0;
			}
		}
	}

	Vector3 TargetPos() => Target.position + TargetOffset;

#if UNITY_EDITOR
	void OnValidate()
	{
		if (Camera && Target)
			PlaceCamera();

		GimbalRotation.y = Mathf.Clamp(GimbalRotation.y, -90f, 90f);
	}

	void OnDrawGizmosSelected()
	{
		if (Camera && Target)
			Debug.DrawLine(TargetPos(), Camera.position, Color.red);
	}
#endif
}
