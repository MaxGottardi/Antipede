using UnityEngine;

public class Launcher : Weapon
{

	[Header(nameof(Launcher) + " Settings.")]
	[Tooltip("Minimum launch height.")] public float LaunchHeight;

	const int kArcResolution = 30;
	static LineRenderer ArcRenderer;

	Vector3 LaunchVelocity;
	bool bDetached = false;

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
		if (Direction == Vector3.zero)
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
		transform.GetChild(0).GetChild(0).LookAt(lookPos + LaunchVelocity);

		if (Owner && ArcRenderer && ArcRenderer.gameObject == gameObject)
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
		ClearArc();
		bDetached = true;
		ArcRenderer = null;
	}
}
