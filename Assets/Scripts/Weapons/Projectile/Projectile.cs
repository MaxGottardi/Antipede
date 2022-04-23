using UnityEngine;

/// <summary>The base class for a launched projectile from a <see cref="Weapon"/>.</summary>
/// <remarks>Default behaviour is a generic straight-line bullet.</remarks>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
	protected Rigidbody rb;

	/// <remarks>Use as Awake/Start method.</remarks>
	public virtual void Initialise()
	{
		rb = GetComponent<Rigidbody>();
	}

	/// <summary>Launches this projectile at LaunchVelocity.</summary>
	/// <param name="LaunchVelocity">The magnitude and direction to launch this projectile.</param>
	public virtual void Launch(Vector3 LaunchVelocity)
	{
		rb.AddForce(LaunchVelocity);
	}
    public virtual void Ray()
    {
        
    }
}