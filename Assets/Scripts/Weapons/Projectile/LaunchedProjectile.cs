using UnityEngine;
using UnityEngine.SceneManagement;

public class LaunchedProjectile : Projectile
{
	/// Weapon is <see cref="Launcher"/>

	[Header("Launcher Settings.")]
	[SerializeField] ParticleSystem Shockwave;
	[SerializeField] float ExplosionRadius;

	public override void Launch(Vector3 LaunchVelocity)
	{
		rb.velocity = LaunchVelocity;
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);

		// Do not show Shockwave when hitting another Projectile || The scene is not the actual Game.
		if (!collision.gameObject.CompareTag("Projectile") && SceneManager.GetActiveScene().name == "Environment Test")
		{
			ParticleSystem ShockwaveParticles = Instantiate(Shockwave, transform.position, Quaternion.identity);

			float Duration = ShockwaveParticles.main.duration;
			Duration += Duration * .5f; // Let it play for a little longer.
			Destroy(ShockwaveParticles.gameObject, Duration);

			Destroy(gameObject);

			if (!isEnemyProjectile)
				Detonate();
		}
	}

	void Detonate()
	{
		Collider[] EnemiesCaughtInBlast = Physics.OverlapSphere(transform.position, ExplosionRadius, 128);

		foreach (Collider Enemy in EnemiesCaughtInBlast)
		{
			// Check to see if Enemy is a Web.
			if (Enemy.transform.parent && Enemy.transform.parent.gameObject.TryGetComponent(out GenericAnt GA))
			{
				// Deal a tenth of the usual damage to any Ant caught in ExplosionRadius.
				GA.ReduceHealth(DamageAmount / 10);
				Instantiate(bloodParticles, Enemy.transform.position + Vector3.up * 0.5f, Quaternion.identity);
			}
			else if (Enemy.gameObject.TryGetComponent(out Tarantula T))
			{
				if (T.healthSlider.value >= .5f)
				{
					// Deal half of the usual damage to any Tarantula caught in ExplosionRadius.
					T.DecreaseHealth(tarantDamage / 2);
					Instantiate(bloodParticles, Enemy.transform.position + Vector3.up * 0.5f, Quaternion.identity);
				}
			}
		}
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
	}
#endif
}
