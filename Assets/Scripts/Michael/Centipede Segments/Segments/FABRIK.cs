//#define WITH_FRAME_LIMITS

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class FABRIK : MonoBehaviour
{
#if UNITY_EDITOR
	[Header("Debug Settings. [EDITOR ONLY]")]
	[SerializeField] bool bShowJoints = true;
	[SerializeField] float JointGizmosRadius = .1f;

	[SerializeField] bool bShowBiasPoint = true;
	[SerializeField] float BiasGizmosRadius = .1f;

	[SerializeField] bool bShowRays = true;

	[SerializeField] bool bAlwaysRunRegardless;
	bool bHasBeenShownWarning;
#endif

	// Optimisations
	static SpringArm MainSpringArmComponent;
	const float kSpringArmThreshold = 15;
	const float kCameraDistanceThreshold = 17f;

	[Header("FABRIK Settings.")]
	[SerializeField] int MaximumPasses = 100;
	[SerializeField] float Tolerance = .01f;
	[SerializeField] float MinLegUpHeight;
	[SerializeField] float MaxLegUpHeight;
	[SerializeField] float LegUpSpeed;
	float RandomHeightAlpha;

	[Header("Targets.")]
	[SerializeField] List<FABRIKLeg> Legs;

	Vector3 LastFramePosition, ThisFramePosition;
	Vector3 LastFrameEulers, ThisFrameEulers;
	/// <summary>True if this Leg is still.</summary>
	bool bHasStopped;

#if WITH_FRAME_LIMITS
	bool bFrameLimiterActive;
	bool bUpdateOneLastTime;
#endif

	void Start()
	{
		if (!MainSpringArmComponent)
			MainSpringArmComponent = Camera.main.GetComponent<SpringArm>();

#if UNITY_EDITOR
		if (!MainSpringArmComponent)
			Debug.LogWarning("The Camera has no Spring Arm Component. Optimisations made to FABRIK will be switched off.");
#endif

		InvokeRepeating(nameof(AssignLegHeights), 0, Mathf.PI / LegUpSpeed);
	}

	void Update()
	{
#if UNITY_EDITOR
		if (bAlwaysRunRegardless)
		{
			ExecuteFABRIKLogic(true);

			if (!bHasBeenShownWarning)
			{
				Debug.LogWarning(nameof(bAlwaysRunRegardless) + " is switched on. Remember to turn it off if you're not testing <color=#FEFE00>:)</color>");
				bHasBeenShownWarning = true;
			}

			return;
		}
#endif

#if WITH_FRAME_LIMITS
		bFrameLimiterActive = MMathStatics.FPS() < 30;

		if (bFrameLimiterActive)
		{
			if (!bUpdateOneLastTime)
			{
				ExecuteFABRIKLogic(true);
				bUpdateOneLastTime = true;
			}

			return;
		}
#endif

		// FABRIK will not execute if this Leg is too far from the Camera.
		if (IsTooFarFromCamera())
			return;

		// FABRIK will not execute on *any* Leg if the Spring Arm Component is too far.
		if (MainSpringArmComponent)
			if (MainSpringArmComponent.Distance >= kSpringArmThreshold)
				return;

		ThisFramePosition = transform.position;
		ThisFrameEulers = transform.eulerAngles;

#if WITH_FRAME_LIMITS
		bUpdateOneLastTime = false;
#endif

		if (HasMovedSinceLastFrame())
		{
			ExecuteFABRIKLogic(false);
			bHasStopped = false;
		}
		else
		{
			// Only run FABRIK once more to ensure Legs are on the ground.
			// But do not continue FABRIK when not needed - when this transform is not moving.
			if (!bHasStopped || HasRotatedSinceLastFrame())
				ExecuteFABRIKLogic(true);

			bHasStopped = true;
		}

		LastFramePosition = ThisFramePosition;
		LastFrameEulers = ThisFrameEulers;
	}

	bool HasRotatedSinceLastFrame()
	{
		Vector3 DeltaRot = ThisFrameEulers - LastFrameEulers;
		MMathStatics.Abs(ref DeltaRot);

		const float kThreshold = .1f;
		bool bHasRotatedSinceLastFrame = DeltaRot.x >= kThreshold || DeltaRot.y >= kThreshold || DeltaRot.z >= kThreshold;
		return bHasRotatedSinceLastFrame;
	}

	/// <returns>True if this transform has moved kEpsilon since the last FixedUpdate call.</returns>
	bool HasMovedSinceLastFrame()
	{
		Vector3 DeltaPosition = ThisFramePosition - LastFramePosition;
		MMathStatics.Abs(ref DeltaPosition);

		bool bHasMovedSinceLastFrame = DeltaPosition.x >= Vector3.kEpsilon || DeltaPosition.y >= Vector3.kEpsilon || DeltaPosition.z >= Vector3.kEpsilon;
		return bHasMovedSinceLastFrame;
	}

	void SineCosOfHeight(ref float Height, out float ClampedSine, out float ClampedCosine)
	{
		float Degrees = Time.time * LegUpSpeed;

		float UnclampedSine = Mathf.Sin(Degrees);
		ClampedSine = (UnclampedSine + 1) * .5f * Height;

		float UnclampedCosine = Mathf.Cos(Degrees);
		ClampedCosine = (UnclampedCosine + 1) * .5f * Height;

		if (UnclampedSine < -.9f || UnclampedCosine < -.9f)
			RandomHeightAlpha = UnityEngine.Random.Range(.2f, 1f);
	}

	void AssignLegHeights()
	{
		if (Legs.Count == 2) // For anything with only two legs.
		{
			float Random = UnityEngine.Random.Range(MinLegUpHeight, MaxLegUpHeight);

			// Leg at index 0.
			FABRIKLeg L0 = Legs[0];
			L0.Height = Random;
			Legs[0] = L0;

			// Leg at index 1.
			FABRIKLeg L1 = Legs[1];
			// Mirror Random with Min and Max.
			L1.Height = MinLegUpHeight + MaxLegUpHeight - Random;
			Legs[1] = L1;
		}
		else // Everything else.
		{
			for (int i = 0; i < Legs.Count; ++i)
			{
				FABRIKLeg L = Legs[i];
				L.Height = UnityEngine.Random.Range(MinLegUpHeight, MaxLegUpHeight);
				Legs[i] = L;
			}
		}
	}

	void ExecuteFABRIKLogic(bool bSnapToTarget = false)
	{
		for (int L = 0; L < Legs.Count; ++L)
		{
			FABRIKLeg Leg = Legs[L];

			RaySettings(Leg, out Vector3 Origin, out Vector3 Direction);

			if (Physics.Raycast(Origin, Direction, out RaycastHit Hit, 50, 256))
			{
				Vector3[] Joints = Leg.GetJointPositions();

				RunFABRIK(Joints, Leg.KneeOrToe[0].position, GetTarget(Hit, Leg, L), GetRelativeXYZ(Leg.Bias));

				for (int i = 0; i < Joints.Length; ++i)
				{
					Leg.KneeOrToe[i].position = Joints[i];

					if (i > 0)
					{
						Transform Limb = Leg.Limbs[i];

						Limb.position = Joints[i - 1];

						if (i < Joints.Length - 1)
						{
							Limb.LookAt(Leg.KneeOrToe[i]);
							Limb.RotateAround(Limb.position, Limb.up, 90);
						}
						else
						{
							Limb.LookAt(Leg.KneeOrToe[Joints.Length - 1]);
							Limb.RotateAround(Limb.position, Limb.up, 90);
						}
					}
				}
			}
		}

		Vector3 GetTarget(RaycastHit Hit, FABRIKLeg Leg, int L)
		{
			SineCosOfHeight(ref Leg.Height, out float Sine, out float Cos);

			float Alpha = L % 2 == 0 ? Sine : Cos;
			float HeightAlternator = Leg.Height * Alpha;

			return bSnapToTarget ? Hit.point : Hit.point + transform.up * HeightAlternator;
		}

#if UNITY_EDITOR
		if (bShowJoints)
		{
			foreach (FABRIKLeg Leg in Legs)
			{
				for (int i = 0; i < Leg.KneeOrToe.Count - 1; ++i)
				{
					Debug.DrawLine(Leg.KneeOrToe[i].position, Leg.KneeOrToe[i + 1].position, Color.cyan);
				}
			}
		}
#endif
	}

#region FABRIK Utils

	void RunFABRIK(Vector3[] Joints, Vector3 Root, Vector3 Target, Vector3 RelativeBias)
	{
		float[] Lengths = new float[Joints.Length - 1];
		for (int i = 0; i < Joints.Length - 1; i++)
		{
			Lengths[i] = (Joints[i + 1] - Joints[i]).magnitude;
			Joints[i] += RelativeBias;
		}

		float ToleranceSquared = Tolerance * Tolerance;

		for (int Pass = 0; Pass < MaximumPasses; ++Pass)
		{
			bool bGoingBackward = Pass % 2 == 0;

			// Reverse arrays to alternate between forward and backward passes
			Array.Reverse(Joints);
			Array.Reverse(Lengths);

			Joints[0] = (bGoingBackward) ? Target : Root;

			// Constrain lengths
			for (int i = 1; i < Joints.Length; i++)
			{
				Vector3 Direction = (Joints[i] - Joints[i - 1]).normalized;
				Joints[i] = Joints[i - 1] + Direction * Lengths[i - 1];
			}

			// Terminate if close enough to target
			float Distance = (Joints[Joints.Length - 1] - Target).sqrMagnitude;
			if (!bGoingBackward && Distance <= ToleranceSquared)
				return;
		}
	}

	void RaySettings(FABRIKLeg Leg, out Vector3 Origin, out Vector3 Direction)
	{
		Origin = transform.position + GetRelativeXYZ(Leg.TargetRayOrigin.x, Leg.TargetRayOrigin.y, Leg.TargetRayOrigin.z);
		Direction = -transform.up;
	}

	Vector3 GetRelativeXYZ(Vector3 V) => GetRelativeXYZ(V.x, V.y, V.z);

	Vector3 GetRelativeXYZ(float X, float Y, float Z) => transform.right * X + transform.up * Y + transform.forward * Z;

#endregion

	/// <returns>True if the distance between this and the Camera exceeds <see cref="kCameraDistanceThreshold"/>.</returns>
	bool IsTooFarFromCamera()
	{
		if (MainSpringArmComponent)
		{
			Vector3 FABRIK = transform.position;
			Vector3 Camera = MainSpringArmComponent.transform.position;

			float A = FABRIK.x - Camera.x;
			float B = FABRIK.y - Camera.y;
			float C = FABRIK.z - Camera.z;

			return (A * A + B * B + C * C) > (kCameraDistanceThreshold * kCameraDistanceThreshold);
		}

		return false;
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (Legs != null && (bShowRays || bShowJoints || bShowBiasPoint))
		{
			foreach (FABRIKLeg Leg in Legs)
			{
				Gizmos.color = Color.cyan;

				if (bShowRays)
				{
					RaySettings(Leg, out Vector3 Origin, out Vector3 Direction);

					Gizmos.DrawRay(Origin, Direction);
				}

				if (bShowJoints)
				{
					foreach (Transform T in Leg.KneeOrToe)
						Gizmos.DrawSphere(T.position, JointGizmosRadius);
				}

				if (bShowBiasPoint)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawSphere(transform.position + GetRelativeXYZ(Leg.Bias.x, Leg.Bias.y, Leg.Bias.z), BiasGizmosRadius);
				}
			}
		}
	}
#endif
}


[Serializable]
struct FABRIKLeg
{
	[Header("Origin and Targets.")]
	public Vector3 TargetRayOrigin;
	public List<Transform> Limbs;
	public List<Transform> KneeOrToe;
	[HideInInspector] public float Height;
	public Vector3 Bias;

	public Vector3[] GetJointPositions() => KneeOrToe.Select(KT => KT.position).ToArray();
}