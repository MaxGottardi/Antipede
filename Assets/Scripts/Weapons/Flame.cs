using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : Weapon
{

	[Header(nameof(Flame) + " Settings.")]
	[SerializeField] float LaunchSpeed;

    public override Projectile Fire(Vector3 Direction)
    {
		if (!CanFire(Direction))
			return null;

		sfxManager.ShootFlame();
		Projectile StraightProjectile = InstantiateProjectile();
		StraightProjectile.Initialise(isAntGun, true);
		
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
