using UnityEngine;

public class Gun : Weapon
{
	/// Projectile is <see cref="Projectile"/>

	[Header(nameof(Gun) + " Settings.")]
	[SerializeField] float LaunchSpeed;

	public override Projectile Fire(Vector3 Direction)
	{
		if (!CanFire(Direction))
			return null;

		//sfxManager.ShootGun();
		sfxManager.ShootGun();
		Projectile StraightProjectile = InstantiateProjectile();
		StraightProjectile.Initialise(isAntGun);

		// Ignore the Direction param. For a Gun, we don't need it.
		if (isAntGun)
			StraightProjectile.Launch(transform.GetChild(0).GetChild(0).GetChild(0).forward * LaunchSpeed / 4.5f);
		else
		{
			StraightProjectile.Launch(transform.GetChild(0).GetChild(0).GetChild(0).forward * LaunchSpeed);
		}

		return StraightProjectile;
	}
}
