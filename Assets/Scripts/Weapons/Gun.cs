using UnityEngine;

public class Gun : Weapon
{
	[Header(nameof(Gun) + " Settings.")]
	[SerializeField] float LaunchSpeed;
	[SerializeField] SFXManager sfxManager;

	public override Projectile Fire(Vector3 Direction)
	{
		//sfxManager.ShootGun();
		Projectile StraightProjectile = InstantiateProjectile();
		StraightProjectile.Initialise(isAntGun);

		// Ignore the Direction param. For a Gun, we don't need it.
		if (isAntGun)
			StraightProjectile.Launch(transform.forward * LaunchSpeed/4.5f);
		else
		{
			StraightProjectile.Launch(transform.forward * LaunchSpeed);
		}

		return StraightProjectile;
	}
}
