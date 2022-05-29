using UnityEngine;

public class Gun : Weapon
{
	[Header(nameof(Gun) + " Settings.")]
	[SerializeField] float LaunchSpeed;
	[SerializeField] SFXManager sfxManager;

    public void Awake()
    {
		sfxManager = FindObjectOfType<SFXManager>();
    }
    public override Projectile Fire(Vector3 Direction)
	{
		if (!bIsRegistered)
			return null;

		//sfxManager.ShootGun();
		sfxManager.ShootGun();
		Projectile StraightProjectile = InstantiateProjectile();
		StraightProjectile.Initialise(isAntGun);

		// Ignore the Direction param. For a Gun, we don't need it.
		if (isAntGun)
			StraightProjectile.Launch(transform.GetChild(0).GetChild(0).GetChild(0).forward * LaunchSpeed/4.5f);
		else
		{
			StraightProjectile.Launch(transform.GetChild(0).GetChild(0).GetChild(0).forward * LaunchSpeed);
		}

		return StraightProjectile;
	}
}
