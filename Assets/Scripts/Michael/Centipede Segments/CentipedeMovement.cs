using UnityEngine;

public class CentipedeMovement : MonoBehaviour
{
	[SerializeField] bool bGlobalMovement = true;

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

		if (bGlobalMovement)
		{
			InDirection = new Vector3(Horizontal, 0, Vertical).normalized;

			InDirection += transform.position;
		}
		else
		{
			// Problem when pressing 'S' or 'DOWN' (going directly backwards) when using Relative Movement (NOT Global Movement)
			// where the centipede doesn't want to rotate behind.
			// With Global Movement, this would just turn with a radius and continue.
			// With Relative Movement, this would vigorously shake the Centipede, but not turn.
			//
			// It's probably because -transform.forward is constantly being updated to be behind the centipede.

			InDirection = transform.position + (transform.right * Horizontal);
			if (Vertical > 0)
			{
				InDirection += transform.forward;
			}


			/*

			// We *could* simply turn left or right in a circle, which is essentially *trying* to go backwards.

			else
			{
				InDirection += transform.right * Vertical;
			}

			*/
		}
	}

	public void HandleMovement(ref MCentipedeBody Body)
	{
		if (bGlobalMovement && (Horizontal != 0 || Vertical != 0) || !bGlobalMovement && (Horizontal != 0 || Vertical > /* != */ 0))
		{
			MMathStatics.HomeTowards(rb, InDirection, Body.FollowSpeed, Body.MaxTurnDegreesPerFrame);
		}
		else
		{
			rb.velocity = Vector3.zero;
		}
		
		rb.AddForce(Physics.gravity);
	}
}
