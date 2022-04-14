using UnityEngine;

public class LaunchedProjectile : Projectile
{

	public override void Launch(Vector3 LaunchVelocity)
	{
		rb.velocity = LaunchVelocity;
	}

}
