using System;
using UnityEngine;

public class SpringArm : MonoBehaviour, IDataInterface
{

#if UNITY_EDITOR
	[Header("Debug Options. [EDITOR ONLY]")]
	[SerializeField] bool bDrawRotationalLines;
	[Space(10)]
#endif
	public static SpringArm Instance;

	public GameSettings Settings;

	[Header("Target Settings.")]
	[SerializeField] Transform Camera;
	public Transform Target;
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
	float OrbitSensitivity = 1f;
	Vector2 PreviousMouseDragPosition;
	Vector3 GimbalRotationInherited;
	Vector3 CameraRotationInherited;
	Vector2 PreviousMousePanPosition;
	Vector3 OriginalTargetOffset;

	[Header("Inverse Settings.")]
	[SerializeField] bool bInvertX; // Inverse LR dragging Orbit Controls.
	[SerializeField] bool bInvertY; // Inverse UD dragging Orbit Controls.
	[SerializeField] bool bInvertZ; // Inverse Zoom Controls.

	[Header("Collisions")]
	[SerializeField] LayerMask OnlyCollideWith;

	[Header("Lag Settings")]
	[SerializeField] float PositionalLagStrength = .2f;
	[SerializeField] float RotationalLagStrength = .2f;
	Vector3 TargetPosition;
	Quaternion TargetRotation;

	[Header("Projection Settings.")]
	[SerializeField] bool bUseCustomProjection;
	[SerializeField] Transform Plane;
	[SerializeField] float NearClipDistance;
	[SerializeField] float DistanceLimit;
	Matrix4x4 DefaultProjection;

	void Awake()
	{
		if (Instance)
		{
			Debug.LogWarning("Make sure there is only one " + nameof(SpringArm) + " in the Game!");
		}
		else
		{
			Instance = this;
		}
	}

	void Start()
	{
		if (Settings)
		{
			Settings.OnSettingsChanged += ReceiveSettings;
			Settings.OnReceiveInspectorDefaults?.Invoke(new Settings(bInheritRotation, 1f));
		}
		else
		{
			Debug.LogWarning("No Settings Object. Not needed if there is no Pause Menu. ");
		}

		DefaultGimbalRotation = GimbalRotation;
		DefaultCameraRotation = CameraRotation;

		GimbalRotationInherited = DefaultGimbalRotation;
		CameraRotationInherited = DefaultCameraRotation;

		OriginalTargetOffset = TargetOffset;

		DefaultProjection = MInput.MainCamera.projectionMatrix;
	}

	void Update()
	{
		UpdateRotationOnMouse();
		PanCameraOnMouse();

		if (Input.GetKeyDown(SettingsVariables.keyDictionary["ChangeCam"]))
			bInheritRotation = !bInheritRotation;

		ScrollDistance();

		if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.Backslash))
			bUseCustomProjection = !bUseCustomProjection;
	}

	void FixedUpdate()
	{
		if (SettingsVariables.boolDictionary["bCamFollow"])
		{
			Camera.position = Vector3.Lerp(Camera.position, TargetPosition, PositionalLagStrength);
			Camera.rotation = Quaternion.Slerp(Camera.rotation, TargetRotation, RotationalLagStrength);
		}
		else
        {
			Camera.position = TargetPosition;
			Camera.rotation = TargetRotation;
		}

		PlaceCamera();
	}

	void OnPreCull()
	{
		ComputeProjection();
	}

	void PlaceCamera()
	{
		// Where the Spring Arm will point towards.
		Vector3 ArmDirection = Vector3.one;
		Vector3 FinalPosition;
		Quaternion FinalRotation = Quaternion.Euler(CameraRotation);

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
			Quaternion InheritRotation = Quaternion.AngleAxis(GimbalRotationInherited.y, Target.right);
			ArmDirection = (InheritRotation * Target.forward).normalized;

			FinalRotation = GetInheritedRotation();
		}

		// If the Spring Arm will collider with something:
		if (RunCollisionsCheck(ref ArmDirection))
			return;

		// Make the Position and Rotation for Lag.
		FinalPosition = TargetPos() - (Distance * ArmDirection);

		SetPositionAndRotation(FinalPosition, FinalRotation);
	}

	bool RunCollisionsCheck(ref Vector3 Direction)
	{
		if (bUseCustomProjection)
			return false;

		Vector3 TP = TargetPos();
		Ray FOV = new Ray(TP, -Direction);
		bool bViewToTargetBlocked = Physics.Raycast(FOV, out RaycastHit Hit, Distance, OnlyCollideWith);

		if (bViewToTargetBlocked)
		{
			Vector3 Point = Hit.point - FOV.direction;
			SetPositionAndRotation(Point, bInheritRotation
				? GetInheritedRotation()
				: Quaternion.Euler(CameraRotation));
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

	Vector3 TargetPos() => Target.position + TargetOffset * Target.up.y;

	Quaternion GetInheritedRotation()
	{
		return Quaternion.Euler(new Vector3(GetInheritedAxis(Target.localEulerAngles.x) + GimbalRotationInherited.y - CameraRotationInherited.x, CameraRotationInherited.y + GetInheritedAxis(Target.localEulerAngles.y)));
	}

	float GetInheritedAxis(float AxisAngle)
	{
		float TargetAxis = AxisAngle;
		if (TargetAxis < 0f)
			TargetAxis = 360f - TargetAxis;
		return TargetAxis;
	}

	void ScrollDistance()
	{
		if (bEnableScrollToDistance)
		{
			Distance += Input.mouseScrollDelta.y * (bInvertZ ? -1f : 1f) * -SettingsVariables.sliderDictionary["zoomSpeed"] / 100;

			Distance = Mathf.Clamp(Distance, 1, 30);
		}
	}

	void ReceiveSettings(Settings InSettings)
	{
		bInheritRotation = InSettings.bInheritRotation;
		OrbitSensitivity = InSettings.CameraMouseSensitivity /* * 2f + Vector3.kEpsilon*/;
		SettingsVariables.sliderDictionary["camRotSpeed"] = OrbitSensitivity * 100; //convert to a range of 0 - 200
	}

	void UpdateRotationOnMouse()
	{
		Vector3 MousePosition = Input.mousePosition;

		if (Input.GetMouseButton(1))
		{
			float DeltaX = (MousePosition.x - PreviousMouseDragPosition.x) * SettingsVariables.sliderDictionary["camRotSpeed"] * .01f; //set to a range between 0 - 2
			float DeltaY = (MousePosition.y - PreviousMouseDragPosition.y) * SettingsVariables.sliderDictionary["camRotSpeed"] * .01f;

			DetermineInverse(ref DeltaX, ref DeltaY);

			if (!bInheritRotation)
			{
				GimbalRotation.x += DeltaX;
				CameraRotation.y += DeltaX;

				if (GimbalRotation.y - DeltaY < 70 && GimbalRotation.y - DeltaY >= -70)
				{
					GimbalRotation.y -= DeltaY;
					CameraRotation.x -= DeltaY;
				}
			}
			else
			{
				CameraRotationInherited.y += DeltaX;

				if (GimbalRotationInherited.y - DeltaY < 70 && GimbalRotationInherited.y - DeltaY >= -70)
				{
					GimbalRotationInherited.y -= DeltaY;
				}
			}
		}
		else
		{
			GimbalRotationInherited = DefaultGimbalRotation;
			CameraRotationInherited = DefaultCameraRotation;
		}

		PreviousMouseDragPosition = MousePosition;
	}

	void DetermineInverse(ref float DeltaX, ref float DeltaY)
	{
		if (bInvertX)
			Inverse(ref DeltaX);
		else if (bInvertY)
			Inverse(ref DeltaY);

		static void Inverse(ref float F) => F *= -1f;
	}


	void PanCameraOnMouse()
	{
		Vector3 MousePosition = Input.mousePosition;

		if (Input.GetMouseButton(2))
		{
			float DeltaX = (MousePosition.x - PreviousMousePanPosition.x) * SettingsVariables.sliderDictionary["camRotSpeed"] * .01f;
			float DeltaY = (MousePosition.y - PreviousMousePanPosition.y) * SettingsVariables.sliderDictionary["camRotSpeed"] * .01f;

			// Ensure 'Right' and 'Up' is relative to the Camera.
			TargetOffset -= DeltaX * Time.deltaTime * Camera.right + DeltaY * Time.deltaTime * Camera.up;
			TargetOffset = Vector3.ClampMagnitude(TargetOffset, 5f);
		}
		else
		{
			TargetOffset = Vector3.Lerp(TargetOffset, OriginalTargetOffset, .2f);
		}

		PreviousMousePanPosition = MousePosition;
	}

	void ComputeProjection()
	{
		if (bUseCustomProjection && Distance > 3)
		{
			if (Physics.Linecast(Target.position, Camera.position, out RaycastHit Intercept, 256))
			{
				NearClipDistance = Intercept.distance;
			}
			else
			{
				NearClipDistance = Distance * .5f;
			}

			Camera C = MInput.MainCamera;

			int Dot = Math.Sign(Vector3.Dot(Plane.forward, Target.position - Camera.position));
			Vector3 CameraWorldPosition = C.worldToCameraMatrix.MultiplyPoint(Target.position);
			Vector3 CameraNormal = C.worldToCameraMatrix.MultiplyVector(Plane.forward) * Dot;

			float CameraDistance = -Vector3.Dot(CameraWorldPosition, CameraNormal) + NearClipDistance;

			// If the Camera is too close to the Target, don't use oblique projection.
			if (Mathf.Abs(CameraDistance) > DistanceLimit)
			{
				Vector4 clipPlaneCameraSpace = new Vector4(CameraNormal.x, CameraNormal.y, CameraNormal.z, CameraDistance);

				C.projectionMatrix = C.CalculateObliqueMatrix(clipPlaneCameraSpace);
			}
			else
			{
				C.projectionMatrix = DefaultProjection;
			}
		}
		else
		{
			MInput.MainCamera.projectionMatrix = DefaultProjection;
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
	public void LoadData(SaveableData saveableData)
    {
		transform.position = saveableData.camPos;
		transform.rotation = saveableData.camRot;
		Distance = saveableData.scrollDistance;
    }

    public void SaveData(SaveableData saveableData)
    {
		saveableData.camPos = transform.position;
		saveableData.camRot = transform.rotation;
		saveableData.scrollDistance = Distance;
    }
}
