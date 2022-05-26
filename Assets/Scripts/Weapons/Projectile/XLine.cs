using UnityEngine;

public class XLine : Projectile
{
	public GameObject Line;
	public GameObject FXef;//激光击中物体的粒子效果
			       // Particle effect of laser hitting object

/* - Block commented out by MW.
 * - We don't need an Update function; we're only firing a laser ONCE.
 
	public void Update()
	{
 */
	public override void Launch(Vector3 Position)
	{
		// RaycastHit hit; // - Original.

		Vector3 Sc;// 变换大小
			   // Transform size.

		Sc.x = 0.5f;
		Sc.z = 0.5f;
		Sc.y = Vector3.Distance(transform.position, Position);

		FXef.transform.position = Position;
		Line.transform.localScale = Sc;

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
}
