using UnityEngine;

public class Laser : Weapon
{
	/* - Block commented out by MW.
	 * - Not needed.

	[Header(nameof(Laser) + " Settings.")]
	//[SerializeField] float LaunchSpeed;

	[SerializeField] GameObject laserLine;

	*/

	public override Projectile Fire(Vector3 Position)
	{
		/* - Original code by Yingfa.

		//Projectile RayProjectile = InstantiateProjectile();
		//RayProjectile.Initialise(false);
		////RayProjectile.Launch(transform.forward * LaunchSpeed);

		//return RayProjectile;

		*/

		if (CanFire(Position))
		{
			sfxManager.ActivateLazer();
			Projectile Laser = InstantiateProjectile();
			Laser.Initialise(isAntGun);
			Laser.Launch(Position);
			
		}

		return null;
	}

	/* - Block commented out by MW.
	 * - Obsolete. Shooting a Laser should be done in Fire(Position);
	
	public void shootLine()
	{
		//Instantiate(laserLine, BarrelEndSocket.position, Quaternion.identity); // - Original
		InstantiateProjectile(); // - MW.
		//lineRenderer = laserLine.GetComponent<LineRenderer>(); // - Commented out by MW.

		RaycastHit hit;

		if (Physics.Raycast(BarrelEndSocket.position, transform.forward, out hit, 5000, ~kIgnoreFromWeaponRaycasts))
		{
			//float beamLength = Vector3.Distance(transform.position, hit.point); // Commented out by MW.
			Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.yellow);
			// Debug.Log("Did Hit");
		}
		else
		{
			Debug.DrawRay(transform.position, transform.forward * 1000, Color.white, 2f);
			// Debug.Log("Did not Hit");
		}
	}

#if UNITY_EDITOR
	void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			shootLine();
		}
	}
#endif
	*/

}
