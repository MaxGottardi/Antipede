using UnityEngine;

public class Launcher : Weapon
{
	/// Projectile is <see cref="LaunchedProjectile"/>
	
	[Header(nameof(Launcher) + " Settings.")]
	[Tooltip("Minimum launch height.")] public float LaunchHeight;

	const int kArcResolution = 30;
	static LineRenderer ArcRenderer;

	Vector3 LaunchVelocity;
	bool bDetached = false;

	public override Projectile Fire(Vector3 Position)
	{
		if (CanFire(Position))
		{
			Vector3 Velocity = LaunchVelocity;
			if (!MMathStatics.DiagnosticCheckNaN(LaunchVelocity))
			{
				Projectile LaunchedProjectile = InstantiateProjectile();
				if (transform.parent.parent != null && isAntGun)
					LaunchedProjectile.Initialise(isAntGun, transform.parent.parent.parent.parent.parent.gameObject.GetComponent<Collider>());
				else//must be on the main menu so cancel
					LaunchedProjectile.Initialise(isAntGun, null);
				LaunchedProjectile.Launch(Velocity);
				sfxManager.ShootLauncher(LaunchedProjectile.gameObject);

				return LaunchedProjectile;
			}
		}

		return null;
	}

	public override void LookAt(Vector3 Direction)
	{
		if (!bIsRegistered)
			return;

		if (Direction == Vector3.zero || !InRange(ref Direction))
		{
			ClearArc();
			return;
		}

		if (!ArcRenderer)
		{
			ArcRenderer = GetComponent<LineRenderer>();
			ArcRenderer.positionCount = kArcResolution + 1;
		}

		if (ArcRenderer.positionCount == 0)
			ArcRenderer.positionCount = kArcResolution + 1;

		LaunchVelocity = MMathStatics.ComputeLaunchVelocity(BarrelEndSocket.position, Direction, LaunchHeight, out float Time, true);

		Vector3 lookPos = transform.position;
		transform.GetChild(0).GetChild(0).GetChild(0).LookAt(lookPos + LaunchVelocity);

		// Somebody disabled the Dotted-line arc. Do not compute the arc if it's disabled.
		if (WeaponsComponent && ArcRenderer && ArcRenderer.gameObject.activeSelf && ArcRenderer.gameObject == gameObject)
		{
			DrawArc(LaunchVelocity, Time);
		}
	}

	void DrawArc(Vector3 LaunchVelocity, float Time)
	{
		for (int i = 0; i <= kArcResolution; ++i)
		{
			float Simulation = i / (float)kArcResolution * Time;

			Vector3 PointOnArcOfTime = LaunchVelocity * Simulation + Physics.gravity * Simulation * Simulation / 2f;

			Vector3 Arc = BarrelEndSocket.position + PointOnArcOfTime;
			ArcRenderer.SetPosition(i, Arc);
		}
	}

	void OnDestroy()
	{
		if (!bDetached)
			ArcRenderer = null;
	}

	void ClearArc()
	{
		if (ArcRenderer)
			ArcRenderer.positionCount = 0;
	}

	public override void Deregister()
	{
		base.Deregister();

		ClearArc();
		bDetached = true;
		ArcRenderer = null;
	}
}
