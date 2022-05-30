using System.Collections;
using UnityEngine;

public class XLine : Projectile
{
	public GameObject Line;
	public GameObject FXef;//激光击中物体的粒子效果
			       // Particle effect of laser hitting object

	[SerializeField] bool bFireNormallyAsYingfaInitiallyDesignedItToBe;

	Vector3 Sc;// 变换大小
		   // Transform size.

	// If = 12, it means a 12th of a second.
	const float kFractionOfASecond = 12f;

	/* - Block commented out by MW.
	 * - We don't need an Update function; we're only firing a laser ONCE.

		public void Update()
		{
	 */
	public override void Launch(Vector3 Position)
	{
		// RaycastHit hit; // - Original.

		Sc.x = 0.5f;
		Sc.z = 0.5f;

		if (bFireNormallyAsYingfaInitiallyDesignedItToBe)
		{
			float Distance = Vector3.Distance(transform.position, Position);
			Sc.y = Distance;

			FXef.transform.position = Position;
			Line.transform.localScale = Sc;

			const float kActualFraction = 2 * (1 / kFractionOfASecond);
			Destroy(Line, kActualFraction);
			Destroy(FXef, kActualFraction);
		}
		else
		{
			StartCoroutine(FireLaser(Position));
		}

		/* - Block commented out by MW.
		 * - We don't need to raycast every frame to determine the position of the laser;
		 * - We already have a Launch(Position) defined where Position = Mouse Position.
		
		////发射射线，通过获取射线碰撞后返回的距离来变换激光模型的y轴上的值
		//// Fires a ray, transforming the value on the y-axis of the laser model
		//// by taking the distance returned after the ray collides.
		//if (Physics.Raycast(transform.position, transform.forward, out hit, 500, ~Weapon.kIgnoreFromWeaponRaycasts))
		//{
		//	Debug.DrawLine(transform.position, hit.point);
		//	Sc.y = hit.distance;
		//	FXef.transform.position = hit.point;//让激光击中物体的粒子效果的空间位置与射线碰撞的点的空间位置保持一致；
		//					    // Make the spatial position of the particle effect of the
		//					    // laser hitting the object consistent with the spatial
		//					    // position of the point where the ray collides;
		//	FXef.SetActive(true);
		//}
		////当激光没有碰撞到物体时，让射线的长度保持为500m，并设置击中效果为不显示
		//// When the laser does not hit the object, keep the length of the ray at 500m,
		//// and set the hit effect to not display.
		//else
		//{
		//	Sc.y = 500;
		//	FXef.SetActive(false);
		//}

		Line.transform.localScale = Sc;

		*/
	}

	IEnumerator FireLaser(Vector3 Position)
	{

		Sc.y = Vector3.Distance(transform.position, Position);

		float t = 0f;

		// Stretch the laser from the Weapon to Position.
		while (t <= 1f)
		{
			Line.transform.localScale = Vector3.Lerp(Vector3.zero, Sc, t);

			yield return null;
			t += Time.deltaTime * kFractionOfASecond;
		}

		// Apply damage.
		if (Physics.Linecast(transform.position, Position, out RaycastHit Hit, 128))
		{
			Transform T = Hit.collider.transform.parent;
			if (T && !T.TryGetComponent(out GuardAnt GUA) && T.TryGetComponent(out GenericAnt GA))
			{
				GA.ReduceHealth(100);
				Instantiate(bloodParticles, Hit.point + Vector3.up * 0.5f, Quaternion.identity);
			}
			else if(Hit.collider.gameObject.CompareTag("Tarantula") && Hit.collider.gameObject.TryGetComponent(out Tarantula tarantula)
				&& tarantula.healthSlider.value >= .5f)
            {
				tarantula.DecreaseHealth(3);
				Instantiate(bloodParticles, transform.position + Vector3.up * 0.5f, Quaternion.identity);
			}
		}

		// Inverse the positions, preparing for negating the Stretch from above.
		FXef.transform.position = Position;
		Line.transform.position = Position;

		// Inverse Scale.
		Vector3 TargetInterp = new Vector3(Sc.x, -Sc.y, Sc.z);

		t = 0f;

		// Un-stretch from the Weapon to Position.
		while (t <= 1f)
		{
			Vector3 Interp = Vector3.Lerp(Vector3.zero, TargetInterp, t);
			Vector3 Mirror = TargetInterp - Interp;

			Line.transform.localScale = Mirror;

			yield return null;
			t += Time.deltaTime * kFractionOfASecond;
		}

		// Destroy this laser after 2 * kFractions Of A Second.
		Destroy(gameObject);
	}
}
