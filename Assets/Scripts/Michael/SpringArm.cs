using System;
using UnityEngine;

public class SpringArm : MonoBehaviour
{

#if UNITY_EDITOR
	[Header("Debug Options. [EDITOR ONLY]")]
	[SerializeField] bool bDrawRotationalLines;
	[Space(10)]
#endif
	public GameSettings Settings;

	[Header("Target Settings.")]
	[SerializeField] Transform Camera;
	[SerializeField] Transform Target;
	[SerializeField] Vector3 TargetOffset;

	[Header("Spring Arm Settings.")]
	public float Distance;
	[SerializeField] Vector3 GimbalRotation;
	[SerializeField] Vector3 CameraRotation;
	[SerializeField] bool bInheritRotation;
	[Space(5)]
	[SerializeField] bool bEnableScrollToDistance;
	[SerializeField] float ScrollSensitivity;
	[HideInInspector, SerializeField] Vector3 DefaultGimbalRotation;
	[HideInInspector, SerializeField] Vector3 DefaultCameraRotation;
	Vector2 PreviousMouseDragPosition;

	[Header("Collisions")]
	[SerializeField] LayerMask OnlyCollideWith;

	[Header("Lag Settings")]
	[SerializeField] float PositionalLagStrength = .2f;
	[SerializeField] float RotationalLagStrength = .2f;
	Vector3 TargetPosition;
	Quaternion TargetRotation;

	void Start()
	{
		if (Settings)
		{
			Settings.OnSettingsChanged += ReceiveSettings;
			Settings.OnReceiveInspectorDefaults?.Invoke(new Settings(bInheritRotation));
		}
		else
		{
			Debug.LogWarning("No Settings Object. Not needed if there is no Pause Menu. ");
		}

		DefaultGimbalRotation = GimbalRotation;
		DefaultCameraRotation = CameraRotation;
	}

	void Update()
	{
		UpdateRotationOnMouse();

		if (Input.GetKeyDown(KeyCode.T))
			bInheritRotation = !bInheritRotation;

		ScrollDistance();
	}

	void FixedUpdate()
	{
		Camera.position = Vector3.Lerp(Camera.position, TargetPosition, PositionalLagStrength);
		Camera.rotation = Quaternion.Slerp(Camera.rotation, TargetRotation, RotationalLagStrength);

		PlaceCamera();
	}

	void PlaceCamera()
	{
		// Where the Spring Arm will point towards.
		Vector3 ArmDirection;
		Vector3 FinalPosition;

		if (!bInheritRotation)
		{
			float VerticalOrbit = GimbalRotation.x;
			float HorizontalOrbit = -GimbalRotation.y;

			VerticalOrbit *= Mathf.Deg2Rad;
			HorizontalOrbit *= Mathf.Deg2Rad;

			// Convert Angles to Vectors.
			Vector3 Ground = new Vector3(Mathf.Sin(VerticalOrbit), 0, Mathf.Cos(VerticalOrbit)); // XZ.
			Vector3 Up = new Vector3(0, Mathf.Sin(HorizontalOrbit), Mathf.Cos(HorizontalOrbit)); // XYZ.

			// Ground's XZ and Up's Y will be used to define the direction of the Spring Arm.
			ArmDirection = new Vector3(Ground.x, Up.y, Ground.z).normalized;

#if UNITY_EDITOR
			if (bDrawRotationalLines)
			{
				Debug.DrawLine(Target.position, Target.position + -Ground * Distance, Color.red);
				Debug.DrawLine(Target.position, Target.position + -Up * Distance, Color.green);
				Debug.DrawLine(Target.position, Target.position + -ArmDirection * Distance, Color.yellow);
			}
#endif
		}
		else
		{
			// Rotates the Camera around Target, given the Gimbal Rotation's Pitch (Y).
			// As a side-effect, this also inherits the Yaw.
			Quaternion InheritRotation = Quaternion.AngleAxis(DefaultGimbalRotation.y, Target.right);
			ArmDirection = (InheritRotation * Target.forward).normalized;

			// Look at the Target, but add the CameraRotation's custom Pitch setting from the Inspector.
			// (-) Pitches towards global up. (+) Pitches towards global down.
			Camera.LookAt(TargetPos());
			Camera.localEulerAngles -= new Vector3(DefaultCameraRotation.x, 0, 0);
		}

		// If the Spring Arm will collider with something:
		if (RunCollisionsCheck(ref ArmDirection))
			return;

		// Make the Position and Rotation for Lag.
		FinalPosition = TargetPos() - (Distance * ArmDirection);
		Quaternion FinalRotation = Quaternion.Euler(CameraRotation);

		SetPositionAndRotation(FinalPosition, FinalRotation);
	}

	bool RunCollisionsCheck(ref Vector3 Direction)
	{
		Vector3 TP = TargetPos();
		Ray FOV = new Ray(TP, -Direction);
		bool bViewToTargetBlocked = Physics.Raycast(FOV, out RaycastHit Hit, Distance, OnlyCollideWith);

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

		if (TargetPosition != FinalPosition || TargetRotation != FinalRotation)
		{
			TargetPosition = FinalPosition;
			TargetRotation = FinalRotation;
		}
	}

	Vector3 TargetPos() => Target.position + TargetOffset;

	void ScrollDistance()
	{
		if (bEnableScrollToDistance)
		{
			Distance += Input.mouseScrollDelta.y * -ScrollSensitivity;
			Distance = Mathf.Clamp(Distance, 1, 50);
		}
	}

	void ReceiveSettings(Settings InSettings)
	{
		bInheritRotation = InSettings.bInheritRotation;
	}

	void UpdateRotationOnMouse()
	{
		if (!bInheritRotation)
		{
			Vector3 MousePosition = Input.mousePosition;

			if (Input.GetMouseButton(1))
			{
				float DeltaX = MousePosition.x - PreviousMouseDragPosition.x;
				float DeltaY = MousePosition.y - PreviousMouseDragPosition.y;

				GimbalRotation.x += DeltaX;
				CameraRotation.y += DeltaX;

				if (GimbalRotation.y - DeltaY < 70 && GimbalRotation.y - DeltaY >= 0)
				{
					GimbalRotation.y -= DeltaY;
					CameraRotation.x -= DeltaY;
				}

			}

			PreviousMouseDragPosition = MousePosition;
		}
	}

#if UNITY_EDITOR
	void OnValidate()
	{
		if (Camera && Target)
			PlaceCamera();

		GimbalRotation.y = Mathf.Clamp(GimbalRotation.y, -90f, 90f);

		PositionalLagStrength = Mathf.Clamp(PositionalLagStrength, Vector3.kEpsilon, 1f);
		RotationalLagStrength = Mathf.Clamp(RotationalLagStrength, Vector3.kEpsilon, 1f);
	}

	void OnDrawGizmosSelected()
	{
		if (Camera && Target)
			Debug.DrawLine(TargetPos(), Camera.position, Color.red);
	}
#endif
}
