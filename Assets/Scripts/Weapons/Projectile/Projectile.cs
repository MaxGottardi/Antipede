using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>The base class for a launched projectile from a <see cref="Weapon"/>.</summary>
/// <remarks>Default behaviour is a generic straight-line bullet.</remarks>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
	protected Rigidbody rb;
	public bool isEnemyProjectile = false; //is this projectile shot by an enemy or not
	public GameObject hitParticles;

	/// <remarks>Use as Awake/Start method.</remarks>
	public virtual void Initialise(bool isEnemyProjectile)
	{
		this.isEnemyProjectile = isEnemyProjectile;
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
		//Destroy(this);
		if (!isEnemyProjectile && collision.gameObject.CompareTag("Enemy"))
        {
			collision.transform.parent.gameObject.GetComponent<GenericAnt>().ReduceHealth(30);
			Instantiate(hitParticles, transform.position + Vector3.up * 0.5f, Quaternion.identity);
			Destroy(gameObject);
		}
		if (!isEnemyProjectile && collision.gameObject.CompareTag("Tarantula") 
			&& collision.gameObject.GetComponent<Tarantula>().healthSlider.value >= 0.50)
        {
			collision.gameObject.GetComponent<Tarantula>().DecreaseHealth();
			Instantiate(hitParticles, transform.position + Vector3.up * 0.5f, Quaternion.identity);
		}
		else if (collision.gameObject.CompareTag("Play"))
		{
			SceneManager.LoadScene("Environment Test");
		}
		else if (collision.gameObject.CompareTag("Settings"))
		{
			SceneManager.LoadScene("SettingsScene");
		}
		else if(collision.gameObject.layer != LayerMask.NameToLayer("Projectile"))
			Destroy(this);
		if(!isEnemyProjectile && collision.gameObject.CompareTag("Tarantula"))
        {
			Destroy(gameObject);
        }


	}

    private void OnTriggerEnter(Collider other)
    {
        if(isEnemyProjectile && other.gameObject.CompareTag("PlayerSegment"))
        {
			GameManager1.mCentipedeBody.RemoveSegment(30);
			Instantiate(hitParticles, transform.position + Vector3.up * 0.5f, Quaternion.identity);
			Destroy(gameObject);
        }
    }
}