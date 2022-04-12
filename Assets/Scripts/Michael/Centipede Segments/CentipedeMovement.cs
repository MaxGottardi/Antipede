using UnityEngine;

public class CentipedeMovement : MonoBehaviour
{
	[SerializeField] bool bGlobalMovement = true;
	[SerializeField] float HeightOffGround;

	[SerializeField, Tooltip("How far ahead of the Centipede should Terrain Surface Normals be checked?"), Min(0f)]
	float Lead;

	[SerializeField, Tooltip("How far downwards should the Centipede check for Surface Normals?")]
	float GroundDistanceCheck = 5;

	Rigidbody rb;

	float Horizontal;
	float Vertical;
	Vector3 InDirection;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	public void Set(ref float H, ref float V)
	{
		Horizontal = H;
		Vertical = V;

		Vector3 SurfaceNormal = GetSurfaceNormal(out bool bGroundWasHit, out RaycastHit Surface);

		if (bGroundWasHit)
		{
			if (bGlobalMovement)
			{
				//Vector3 ForwardPitch = transform.forward;
				//ForwardPitch.x = 0;
				//ForwardPitch.z = 0;
				//Vector3 GlobalForwardRelativePitch = Vector3.forward + ForwardPitch;
				//InDirection = GlobalForwardRelativePitch * Vertical + Vector3.right * Horizontal;

				Vector3 NormalForward = Vector3.Cross(Vector3.right, SurfaceNormal);
				Vector3 NormalRight = Vector3.Cross(SurfaceNormal, Vector3.forward);

				InDirection = NormalForward * Vertical + NormalRight * Horizontal;
			}
			else
			{
				Vector3 NormalRelativeForward = Vector3.Cross(transform.right, SurfaceNormal);
				Vector3 NormalRelativeRight = Vector3.Cross(SurfaceNormal, transform.forward);

				InDirection = NormalRelativeForward * Vertical + NormalRelativeRight * Horizontal;

				// Problem when pressing 'S' or 'DOWN' (going directly backwards) when using Relative Movement (NOT Global Movement)
				// where the centipede doesn't want to rotate behind.
				// With Global Movement, this would just turn with a radius and continue.
				// With Relative Movement, this would vigorously shake the Centipede, but not turn.
				//
				// For now, you cannot move directly backwards when moving Relatively.
			}

			InDirection += transform.position;
			transform.position = Surface.point - (transform.forward * Lead) + new Vector3(0, HeightOffGround);
		}
		else
		{
			if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit UpHit, 5, 256))
			{
				transform.position = UpHit.point;
				Set(ref Horizontal, ref Horizontal);
			}
		}
	}

	public void HandleMovement(ref MCentipedeBody Body)
	{
		bool bHasInput = Horizontal != 0 || Vertical != 0;
		bool bInputIsRelative = !bGlobalMovement && (Horizontal != 0 || Vertical > /* != */ 0);

		if (bHasInput && (bGlobalMovement && bHasInput || bInputIsRelative))
		{
			MMathStatics.HomeTowards(rb, InDirection, Body.FollowSpeed, Body.MaxTurnDegreesPerFrame);
		}
		else
		{
			rb.velocity = Vector3.zero;
		}
	}

	Vector3 GetSurfaceNormal(out bool bDidHitSomething, out RaycastHit Hit)
	{
		bDidHitSomething = Physics.Raycast(transform.position + (transform.forward * Lead), Vector3.down, out Hit, GroundDistanceCheck, 256);

		// The Surface Normal of the terrain.
		Debug.DrawLine(Hit.point, Hit.point + Hit.normal * 3, Color.red);

		// The Centipede's up.
		Debug.DrawLine(transform.position, transform.position + transform.up * 3, Color.blue);

		return bDidHitSomething ? Hit.normal : Vector3.zero;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(transform.position + (transform.forward * Lead), .05f);
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(transform.position + Vector3.down * HeightOffGround, transform.position);
	}

	void OnValidate()
	{
		transform.position = new Vector3(transform.position.x, HeightOffGround, transform.position.z);
	}
}
