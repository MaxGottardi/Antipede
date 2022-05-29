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

	[Header("Acceleration")]
	public AnimationCurve AccelerationCurve;
	[HideInInspector] public float AccelerationTime = 0f;

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
	[SerializeField] bool bEnable90DegreeInclines;

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
	/// <summary>The world coordinates of where the Head of the Centipede is going towards.</summary>
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

		// Do NOT interpolate when course correcting.
		if (t <= 1f && !bIsCourseCorrecting)
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
	public void Set(ref float H, ref float V, ref MCentipedeBody Body)
	{
		// Player Inputs.
		Horizontal = H;
		Vertical = V;

		if (Horizontal == 0 && Vertical == 0)
			return;

		BoundariesCheckCollisions(ref Body);

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

			// Get Normal vectors to get where the Centipede should face, and define which directions
			// are Forward and Right for Movement Input.
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

			// If we ARE course correcting, we don't want to get any movement input from the
			// player.
			if (!bIsCourseCorrecting)
			{
				// Player Input for Movement instructions are set here:
				InDirection = NormalForward * Vertical + NormalRight * Horizontal;
				InDirection += transform.position;
			}

			// Align the Centipede to correctly place itself above the Ground.
			transform.position = Surface.point - (transform.forward * Lead) + HeightOffGround * -Evaluate(SurfaceNormal);

			// Update interpolation.
			if (PreviousNormal != SurfaceNormal)
			{
				TargetY = NormalForward.y;
				t = 0;
			}
		}
		else
		{
			// -- Called when the Centipede ray found nothing underneath. -- \\

			// Fire a ray to check if the Centipede is on an edge.
			if (bEnable90DegreeInclines)
			{
				Ray Ray = new Ray(transform.position + (transform.forward * Lead) + (-transform.up * Lead), -transform.forward);
				if (Physics.Raycast(Ray, out RaycastHit EdgeDetection, GroundDistanceCheck, 256))
				{
					// Wrap the Centipede around the edge and keep going.

					// Update new position on the new edge.
					transform.position = EdgeDetection.point + HeightAsVector;

					// Update rotations:
					Vector3 NewHeadingDirection = transform.position + Vector3.Cross(transform.right, EdgeDetection.normal);
					transform.LookAt(NewHeadingDirection); // Look at the new edge's 'forward'.
									       // This is the 'normal' of that edge.

					// This updates the Roll (Z) of the Centipede. Keep these in this order.
					transform.rotation = Quaternion.FromToRotation(transform.up, EdgeDetection.normal) * transform.rotation;

					// Update the new moving direction.
					InDirection = NewHeadingDirection;

					// Update the normal for continuous checks.
					// This makes this edge as the new Ground the Centipede is on
					// and any further Ground checks will be relative to this new Edge.
					SurfaceNormal = EdgeDetection.normal;

					// Slow the Centipede.
					AccelerationTime = 0f;

#if UNITY_EDITOR
					if (bShowGizmos)
					{
						// Where the Ray is going.
						Debug.DrawRay(Ray.origin, Ray.direction, Color.magenta, 5f);

						// Where the Centipede should face.
						Debug.DrawLine(EdgeDetection.point, NewHeadingDirection, Color.yellow, 5f);
					}
#endif
				}
			}
			// If nothing was hit, shoot a ray from above back down to the terrain and teleport to that position.
			// This usually happens when the Centipede falls out of the world when going up steep terrain.
			else if (Physics.Raycast(transform.position + Vector3.up * GroundDistanceCheck, Vector3.down, out RaycastHit SkyRay, GroundDistanceCheck, 256))
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

		if (bHasInput && (bGlobalMovement || bInputIsRelative))
		{
			AccelerationTime += Time.deltaTime;

			MMathStatics.HomeTowards(rb, InDirection, EvaluateAcceleration(Body.MovementSpeed), Body.TurnDegrees);

			AccelerationTime = Mathf.Min(AccelerationTime, 1f);
		}
		else
		{
			rb.velocity = Vector3.zero;

			transform.rotation = Quaternion.Slerp(transform.rotation,
				Quaternion.FromToRotation(transform.up, SurfaceNormal) * transform.rotation, .3f);

			AccelerationTime = 0f;
		}
	}

	float EvaluateAcceleration(float Scalar)
	{
		float AccelRate = AccelerationCurve.Evaluate(AccelerationTime);

		return AccelRate * Scalar;
	}

	/// <summary>Grabs the Normal of the terrain the Centipede is on.</summary>
	/// <param name="bHasHitSomething">True if terrain was hit downwards from GroundHeightDistance to GroundDistanceCheck.</param>
	/// <param name="Hit"><see cref="RaycastHit"/>.</param>
	/// <returns>The Surface Normal, out bool true if something was hit, out RaycastHit information.</returns>
	Vector3 GetSurfaceNormal(out bool bHasHitSomething, out RaycastHit Hit)
	{
		if (bEnable90DegreeInclines)
		{
			if (Physics.Raycast(transform.position, transform.forward, out Hit, Lead, 256))
			{
				if (Vector3.Dot(Hit.normal, transform.forward) < 0f)
				{
					bHasHitSomething = Hit.collider;

					AccelerationTime = 0f;

					// Update new position on the new edge.
					transform.position = Hit.point + HeightAsVector;

					// Update rotations:
					Vector3 NewHeadingDirection = transform.position + Vector3.Cross(transform.right, Hit.normal);

					//transform.LookAt(NewHeadingDirection); // Look at the new edge's 'forward'.
					// This is the 'normal' of that edge.

					// This updates the Roll (Z) of the Centipede. Keep these in this order.
					transform.rotation = Quaternion.FromToRotation(transform.up, Hit.normal) * transform.rotation;

					Debug.DrawRay(Hit.point + Vector3.right * .5f, Vector3.up, Color.white, 5f);
					Debug.DrawLine(Hit.point, NewHeadingDirection, Color.cyan, 5f);

					return bHasHitSomething ? Hit.normal : Vector3.zero;
				}
			}
		}

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

	[SerializeField] ECourseCorrectionMethod CourseCorrectionMethod = ECourseCorrectionMethod.Reflect;
	IEnumerator CurrentCourseCorrection = null;

	bool BoundariesCheckCollisions(ref MCentipedeBody Body)
	{
		if (bIsCourseCorrecting)
			return true;

		Ray R = GetRay(out Vector3 NormalisedInDirection);

		FireBoundariesCheckRay(R, out bool bWillCollideWithABoundary, out RaycastHit BoundariesCheck);

		if (bWillCollideWithABoundary)
		{
			Vector3 AutoCorrect = CalculateCourseCorrection(Body, ref NormalisedInDirection, BoundariesCheck);

			// Tell this Movement Component that we are correcting our course.
			CurrentCourseCorrection = CorrectCourse(Body, AutoCorrect);
			StartCoroutine(CurrentCourseCorrection);
		}

		return bWillCollideWithABoundary;
	}

	IEnumerator CorrectCourse(MCentipedeBody Body, Vector3 Auto)
	{
		// Tell everything else that we are correcting our course.
		bIsCourseCorrecting = true;

		AccelerationTime = 0f;

		if (CourseCorrectionMethod == ECourseCorrectionMethod.ChaseTail)
		{
			// The Centipede will continue correcting its course until it faces the corrected direction.
			while (Vector3.Dot(transform.forward, (InDirection - transform.position).normalized) < .95f)
				yield return null;
		}
		else if (CourseCorrectionMethod == ECourseCorrectionMethod.Reflect)
		{
			Vector3 NewAuto = Auto;
			NewAuto.y = 0;
			NewAuto.Normalize();

			while (true)
			{
				Vector3 NewForward = transform.forward;
				NewForward.y = 0;
				NewForward.Normalize();

				if (!(Vector3.Dot(NewForward, NewAuto) < .95f))
					break;

				yield return null;
			}
		}

		// Once we are facing our corrected direction, we are no longer correcting our course.
		bIsCourseCorrecting = false;
	}

	/// <summary>The Ray that will be shot for checking Boundary collisions.</summary>
	/// <param name="NormalisedInDirection">Outs <see cref="Transform.forward"/>.</param>
	/// <returns>The Origin and Direction of the Ray for checking collisions.</returns>
	Ray GetRay(out Vector3 NormalisedInDirection)
	{
		// Fire a Ray forwards to check for Boundary collisions.
		NormalisedInDirection = transform.forward;
		return new Ray(transform.position, NormalisedInDirection);
	}

	/// <summary>Fires a Raycast used for checking Boundary collisions.</summary>
	/// <param name="R">Origin and Direction of this Ray.</param>
	/// <param name="bWillCollideWithABoundary">Outs true if the Centipede will collide with a Boundary.</param>
	/// <param name="BoundariesCheck">Raycast hit information.</param>
	void FireBoundariesCheckRay(Ray R, out bool bWillCollideWithABoundary, out RaycastHit BoundariesCheck)
	{
		// Boundary Layer (1 << 11) (2 ^ 11).
		bWillCollideWithABoundary = Physics.Raycast(R, out BoundariesCheck, BoundaryCheckDistance, 2048);
	}

	/// <summary>Calculates how the Centipede should correct its course.</summary>
	/// <param name="Body">Only used when <see cref="ECourseCorrectionMethod.ChaseTail"/> for getting <see cref="MCentipedeBody.GetAbsoluteLast"/>.</param>
	/// <param name="NormalisedInDirection">Only used in <see cref="ECourseCorrectionMethod.Reflect"/> and will be modified to represent the Upper Angle.</param>
	/// <param name="BoundariesCheck">The Raycast hit information from <see cref="FireBoundariesCheckRay(Ray, out bool, out RaycastHit)"/>.</param>
	/// <returns>
	/// If <see cref="ECourseCorrectionMethod.ChaseTail"/>, the Position of <see cref="MCentipedeBody.GetAbsoluteLast"/>.
	/// <br>Else <see cref="ECourseCorrectionMethod.Reflect"/>, the Direction of the reflected position.</br>
	/// </returns>
	Vector3 CalculateCourseCorrection(MCentipedeBody Body, ref Vector3 NormalisedInDirection, RaycastHit BoundariesCheck)
	{
		Vector3 AutoCorrect = Vector3.zero;
		if (CourseCorrectionMethod == ECourseCorrectionMethod.ChaseTail)
		{
			// If a Boundary was hit, set go back to either the Tail, Absolute Tail (the very-very last Segment)
			// or the Last Segment.
			AutoCorrect = Body.GetAbsoluteLast().transform.position;

			InDirection = AutoCorrect;

#if UNITY_EDITOR
			Debug.Log("Centipede collided with Boundary: " + BoundariesCheck.collider.name);

			if (bShowGizmos)
			{
				Vector3 BoundaryPoint = BoundariesCheck.point;

				// Boundary Collision point.
				Debug.DrawLine(transform.position, BoundaryPoint, Color.white, 2);

				// Boundary to Auto Correct point.
				Debug.DrawLine(BoundaryPoint, AutoCorrect, Color.red, 2);

				// New InDirection; where the Centipede is auto correcting towards.
				Debug.DrawLine(transform.position, AutoCorrect, Color.magenta, 2);
			}
#endif

		}
		else if (CourseCorrectionMethod == ECourseCorrectionMethod.Reflect)
		{
			#region Confusing 'Graphic'

			/*

			-- In case I forget --

			| = Boundary.
			* = Marker
			< or v = Point
			H, S, T = Head, Segment, Tail
			H', S', T' = Pretend HST.
			/ or \ = Direction or Banked Terrain.
			_ = Flat Terrain.
			[] = GraphicsSplit
			
			|                                 []       * PretendPosition
			|   Side View                     []        \        Front View  
			|   (Background is Landscape)     []         \       (Background is Boundary)  
			|                                 []          \
			|                                 []           \
			|     /T'                         []            \
			|    /S'                          []             \
			|   /H' * PretendPosition         []              \
			|  /                              []               \* Point
			| /                               []                v
			|/                                []               / \
			|< * Point                        []              /   \
			|\                                []             /     \
			| \                               []            /       \
			|  \                              []           /         \
			|   \H                            []          /           \
			|    \S                           []         /             \
			|     \T                          []        /               \
			|      \                          []       * Actual Pos.     * AutoCorrect
			|       ____________              []      _____________________

			*/

			#endregion

			Vector3 Point = BoundariesCheck.point;

			// Pretend the Centipede is above the Point of the Boundary.
			Vector3 PretendPosition = transform.position;

			float DeltaY = Point.y - PretendPosition.y;
			PretendPosition.y += DeltaY * 2; // Set the height to be higher than the Boundary so that
							 // reflecting will cause it to go towards the terrain,
							 // instead of in the air.

			// Change the direction as we're pretending that the Centipede came from above.
			NormalisedInDirection = (Point - PretendPosition).normalized;

			AutoCorrect = Vector3.Reflect(NormalisedInDirection, BoundariesCheck.normal);
			InDirection = transform.position + AutoCorrect;

			// From the Boundary to the Reflected point.
			Debug.DrawRay(BoundariesCheck.point, AutoCorrect, Color.blue, 5);

			Debug.DrawLine(PretendPosition, Point, Color.red, 5);       // From the Pretended Position to the Boundary.
			Debug.DrawLine(transform.position, Point, Color.white, 5);  // From the Centipede's actual position to the point.
			Debug.DrawRay(Point, NormalisedInDirection, Color.cyan, 5); // From the Boundary to where the Centipede is going towards.

#if UNITY_EDITOR
			Debug.Log("Centipede collided with Boundary: " + BoundariesCheck.collider.name);

			if (bShowGizmos)
			{
				Vector3 BoundaryPoint = BoundariesCheck.point;

				// Boundary Collision point.
				Debug.DrawLine(transform.position, BoundaryPoint, Color.white, 2);

				// Boundary to Auto Correct point.
				Debug.DrawLine(BoundaryPoint, BoundaryPoint + AutoCorrect, Color.red, 2);

				// New InDirection; where the Centipede is auto correcting towards.
				Debug.DrawLine(transform.position, InDirection, Color.magenta, 2);
			}
#endif
		}

		return AutoCorrect;
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

		GUI.Label(new Rect(10, 40, 350, 150), "Acceleration Time: " + AccelerationTime);
	}

	void OnValidate()
	{
		if (bInterpolateHillClimb && bGlobalMovement)
			Debug.LogWarning("Interpolation and Global Movement are incompatible with each other.");
	}
#endif

	enum ECourseCorrectionMethod : int
	{
		[InspectorName("Chase Tail")]
		ChaseTail = 0,
		Reflect = 1
	}
}
