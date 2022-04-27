using UnityEngine;

public class Launcher : Weapon
{

	[Header(nameof(Launcher) + " Settings.")]
	[Tooltip("Minimum launch height.")] public float LaunchHeight;

	Vector3 LaunchVelocity;

	public override Projectile Fire(Vector3 Position)
	{
		Vector3 Velocity = LaunchVelocity;
		if (!MMathStatics.DiagnosticCheckNaN(LaunchVelocity))
		{
			Projectile LaunchedProjectile = InstantiateProjectile();
			LaunchedProjectile.Initialise(isAntGun);
			LaunchedProjectile.Launch(Velocity);

			return LaunchedProjectile;
		}

		return null;
	}

	public override void LookAt(Vector3 Direction)
	{
		LaunchVelocity = MMathStatics.ComputeLaunchVelocity(BarrelEndSocket.position, Direction, LaunchHeight, true);
		transform.LookAt(transform.position + LaunchVelocity);
	}
}
