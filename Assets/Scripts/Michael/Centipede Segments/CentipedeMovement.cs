using System.Collections;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CentipedeMovement : MonoBehaviour
{
#if UNITY_EDITOR
	[Header("Show Gizmos. [EDITOR ONLY]")]
	[SerializeField] bool bShowGizmos;
	[SerializeField] bool bPrintDebugLogs;
#endif

	[Header("Player Movement Preference.")]
	[SerializeField] bool bGlobalMovement = true;


	[Header("Height From Ground Setting. (Can't change during Play)")]
	[SerializeField, Tooltip("How far should the Centipede walk above the terrain?")]
	float HeightOffGround;
	Vector3 HeightAsVector;

	[Header("Terrain Checker Settings.")]
	[SerializeField, Tooltip("How far ahead of the Centipede should Terrain Ground be checked?"), Min(0f)]
	float Lead;
	[SerializeField, Tooltip("How high up should the Centipede begin to check for Ground?"), Min(0f)]
	float GroundHeightDistance = 2f;
	[SerializeField, Tooltip("How far downwards should the Centipede check for Ground?"), Min(.1f)]
	float GroundDistanceCheck = 5f;
	[SerializeField, Tooltip("How far ahead should the Centipede check for Boundaries?")]
	float BoundaryCheckDistance = 3f;
	bool bIsCourseCorrecting;

	[Header("Interpolation Settings.")]
	[SerializeField] bool bInterpolateHillClimb;
	[SerializeField, Min(Vector3.kEpsilon)] float InterpTime = .2f;
	float YMatchSpeed;
	float FromY;
	float TargetY;


	Rigidbody rb;

	// Movement.
	float Horizontal;
	float Vertical;
	Vector3 InDirection;

	// Surface Normals.
	Vector3 PreviousNormal;
	Vector3 SurfaceNormal;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		HeightAsVector = new Vector3(0, HeightOffGround);
		YMatchSpeed = 1 / InterpTime;
	}

	float t = 0;

	void Update()
	{
		if (!bInterpolateHillClimb || bGlobalMovement)
			return;

		if (t <= 1f)
		{
			t += Time.deltaTime * YMatchSpeed;

			float Interp = Mathf.Lerp(FromY, TargetY, t);
			InDirection.y = Interp + transform.position.y;
#if UNITY_EDITOR
			if (bPrintDebugLogs)
				Debug.Log("From: " + FromY.ToString("F2") + " To: " + TargetY.ToString("F2") + "\t\tInterp: " + Interp.ToString("F2") + " Time:" + t.ToString("F2"));
			if (bShowGizmos)
				Debug.DrawLine(transform.position, InDirection, Color.white);
#endif
		}
	}

	/// <summary>Send instructions for Horizontal (+X) and Vertical (+Z) Movement.</summary>
	/// <param name="H">Horizontal. -X &lt; 0 &gt; +X.</param>
	/// <param name="V">Vertical. -Z &lt; 0 &gt; +Z.</param>
	public void Set(ref float H, ref float V)
	{
		Horizontal = H;
		Vertical = V;

		if (Horizontal == 0 && Vertical == 0)
			return;

		if (BoundariesCheckCollisions())
			return;

		PreviousNormal = SurfaceNormal;
		SurfaceNormal = GetSurfaceNormal(out bool bGroundWasHit, out RaycastHit Surface);

		if (bGroundWasHit)
		{
			// Update interpolation targets when the Centipede goes on another face (a triangle in the terrain).
			if (PreviousNormal != SurfaceNormal)
			{
				FromY = transform.forward.y;
			}

			Vector3 NormalForward;
			Vector3 NormalRight;

			if (bGlobalMovement)
			{
				NormalForward = Vector3.Cross(Vector3.right, SurfaceNormal);
				NormalRight = Vector3.Cross(SurfaceNormal, Vector3.forward);
			}
			else
			{
				NormalForward = Vector3.Cross(transform.right, SurfaceNormal);
				NormalRight = Vector3.Cross(SurfaceNormal, transform.forward);
			}

			InDirection = NormalForward * Vertical + NormalRight * Horizontal;
			InDirection += transform.position;

			transform.position = Surface.point - (transform.forward * Lead) + HeightOffGround * -Evaluate(SurfaceNormal);

			if (PreviousNormal != SurfaceNormal)
			{
				TargetY = NormalForward.y;
				t = 0;
			}
		}
		else
		{
			// If nothing was hit, shoot a ray from above back down to the terrain and teleport to that position.
			// This usually happens when the Centipede falls out of the world when going up steep terrain.
			if (Physics.Raycast(transform.position + Vector3.up * GroundDistanceCheck, Vector3.down, out RaycastHit SkyRay, GroundDistanceCheck, 256))
			{
				transform.position = SkyRay.point + HeightAsVector;
				transform.LookAt(transform.position + Vector3.Cross(transform.right, SkyRay.normal));

#if UNITY_EDITOR
				if (bPrintDebugLogs)
					Debug.LogWarning("Centipede fell out of the world, but it fixed itself.");
#endif
			}
			else
			{
				bool bUnderneathIsGround = Physics.Raycast(transform.position, Vector3.down, out RaycastHit Anything, 50000);
				if (!bUnderneathIsGround)
				{
					// The Centipede has fallen out of the world.

					// ...

#if UNITY_EDITOR
					if (bPrintDebugLogs)
						Debug.LogError("Centipede has nothing underneath!");
#endif
					return;
				}

				// TODO: Interpolate the downward fall to Anything.point so that it doesn't look as bad.

				transform.position = Anything.point + HeightAsVector;
				transform.LookAt(transform.position + Vector3.Cross(transform.right, Anything.normal));
			}
		}
	}

	public void HandleMovement(ref MCentipedeBody Body)
	{
		bool bHasInput = Horizontal != 0 || Vertical != 0;
		bool bInputIsRelative = !bGlobalMovement && (Horizontal != 0 || Vertical > /* != */ 0);

		if (bHasInput && (bGlobalMovement && bHasInput || bInputIsRelative))
		{
			MMathStatics.HomeTowards(rb, InDirection, Body.MovementSpeed, Body.TurnDegrees);
		}
		else
		{
			rb.velocity = Vector3.zero;

			transform.rotation = Quaternion.Slerp(transform.rotation,
				Quaternion.FromToRotation(transform.up, SurfaceNormal) * transform.rotation, .3f);
		}
	}

	/// <summary>Grabs the Normal of the terrain the Centipede is on.</summary>
	/// <param name="bHasHitSomething">True if terrain was hit downwards from GroundHeightDistance to GroundDistanceCheck.</param>
	/// <param name="Hit"><see cref="RaycastHit"/>.</param>
	/// <returns>The Surface Normal, out bool true if something was hit, out RaycastHit information.</returns>
	Vector3 GetSurfaceNormal(out bool bHasHitSomething, out RaycastHit Hit)
	{
		Vector3 Direction = Evaluate(SurfaceNormal);

		bHasHitSomething = Physics.Raycast(transform.position + (transform.forward * Lead) + (-Direction * GroundHeightDistance),
			Direction, out Hit, GroundDistanceCheck + GroundHeightDistance, 256);

#if UNITY_EDITOR
		if (bShowGizmos)
		{
			// The Surface Normal of the terrain.
			Debug.DrawLine(Hit.point, Hit.point + Hit.normal * 3, Color.red);

			// The Centipede's up.
			Debug.DrawLine(transform.position, transform.position + transform.up * 3, Color.blue);

			// Draw the line towards the ground.
			Debug.DrawRay(transform.position, Vector3.down, Color.white);
		}
#endif

		// For some reason, Physics registers a hit, but sometimes there's no collider.
		// Check for a collider.
		bHasHitSomething &= Hit.collider;

		return bHasHitSomething ? Hit.normal : Vector3.zero;
	}

	/// <summary>Based on the Centipede's Up, get a direction that goes 'Down', relative to the Centipede.</summary>
	/// <param name="U">Normalised Up or Normal.</param>
	/// <returns>E.g. When 'Up' faces -Z, Vector3.forward.</returns>
	Vector3 Evaluate(Vector3 U)
	{
		if (U == Vector3.zero)
			return Vector3.down;

		// NOTE - THESE ARE NOT ANGLES.
		const float kConsiderForward = .9f;
		const float kConsiderRight = .9f;

		if (U.x > kConsiderRight)
			return Vector3.left;
		if (U.x < -kConsiderRight)
			return Vector3.right;
		if (U.z > kConsiderForward)
			return Vector3.back;
		if (U.z < -kConsiderForward)
			return Vector3.forward;
		if (U.y > 0f)
			return Vector3.down;
		if (U.y < 0f)
			return Vector3.up;

		Debug.LogError("Failed: " + U.ToString("F2"));
		return Vector3.down;
	}

	Vector3 Evaluate(Transform T)
	{
		return Evaluate(T.up);
	}

	bool BoundariesCheckCollisions()
	{
		if (bIsCourseCorrecting)
			return true;

		Vector3 NormalisedInDirection = transform.forward;
		Ray R = new Ray(transform.position, NormalisedInDirection);
		bool bWillCollideWithABoundary = Physics.Raycast(R, out RaycastHit BoundariesCheck, BoundaryCheckDistance, 2048);

		Vector3 BoundaryNormal = BoundariesCheck.normal;
		Vector3 AutoCorrect = Vector3.Reflect(NormalisedInDirection, BoundaryNormal);

		InDirection = transform.position + AutoCorrect;

#if UNITY_EDITOR
		if (bShowGizmos && bWillCollideWithABoundary)
		{
			Vector3 BoundaryPoint = BoundariesCheck.point;

			Debug.DrawLine(transform.position, BoundaryPoint, Color.white, 2);
			Debug.DrawLine(BoundaryPoint, BoundaryPoint + AutoCorrect, Color.red, 2);
			Debug.DrawLine(transform.position, InDirection, Color.magenta, 2);
		}
#endif

		StartCoroutine(CorrectCourse(AutoCorrect));

		return bWillCollideWithABoundary;
	}

	IEnumerator CorrectCourse(Vector3 Reflected)
	{
		bIsCourseCorrecting = true;

		// The Centipede will continue correcting its course until it faces the corrected direction.
		while (Vector3.Dot(transform.forward, Reflected) < .95f)
			yield return null;

		bIsCourseCorrecting = false;
	}

#if UNITY_EDITOR

	void OnDrawGizmos()
	{
		if (bShowGizmos)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(transform.position + (transform.forward * Lead) + (-Evaluate(transform) * GroundHeightDistance), .05f);
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(transform.position + Vector3.down * HeightOffGround, transform.position);

			Gizmos.DrawSphere(InDirection, .1f);

			const string kFloatAccuracy = "F2";

			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(transform.position, transform.up);

			Vector3 tu = Evaluate(transform);
			Gizmos.color = Color.green;
			Debug.DrawRay(transform.position, tu);

			Handles.Label(transform.position + transform.up, transform.up.ToString(kFloatAccuracy));
			Handles.Label(transform.position + tu, tu.ToString(kFloatAccuracy));
		}
	}

	float ft = 0, FPS_NOW;

	void OnGUI()
	{
		ft += Time.deltaTime;

		if (ft > .2f)
		{
			FPS_NOW = MMathStatics.FPS();
			ft = 0;
		}

		GUI.Label(new Rect(10, 10, 150, 150), "FPS: " + FPS_NOW);
	}

	void OnValidate()
	{
		if (bInterpolateHillClimb && bGlobalMovement)
			Debug.LogWarning("Interpolation and Global Movement are incompatible with each other.");
	}
#endif
}
