using UnityEngine;

/// <summary>The base class for a launched projectile from a <see cref="Weapon"/>.</summary>
/// <remarks>Default behaviour is a generic straight-line bullet.</remarks>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
	protected Rigidbody rb;
	public bool isEnemyProjectile = false; //is the projectile shot by an enemy or not

	/// <remarks>Use as Awake/Start method.</remarks>
	public virtual void Initialise()
	{
		rb = GetComponent<Rigidbody>();
		Destroy(gameObject, 10f);
	}

	/// <summary>Launches this projectile at LaunchVelocity.</summary>
	/// <param name="LaunchVelocity">The magnitude and direction to launch this projectile.</param>
	public virtual void Launch(Vector3 LaunchVelocity)
	{
		rb.AddForce(LaunchVelocity);
	}

    private void OnCollisionEnter(Collision collision)
    {
		if(!isEnemyProjectile && collision.gameObject.CompareTag("Enemy"))
        {
			collision.transform.parent.gameObject.GetComponent<GenericAnt>().ReduceHealth(30);
        }
		else if(isEnemyProjectile && collision.gameObject.CompareTag("PlayerSegment"))
        {
			GameManager1.mCentipedeBody.RemoveSegment(30);
        }
    }
}