using UnityEngine;

public class Gun : Weapon
{
	[Header(nameof(Gun) + " Settings.")]
	[SerializeField] float LaunchSpeed;

	public override Projectile Fire(Vector3 Direction)
	{
		Projectile StraightProjectile = InstantiateProjectile();
		StraightProjectile.Initialise();
		StraightProjectile.Launch(transform.forward * LaunchSpeed);

		return StraightProjectile;
	}
}
