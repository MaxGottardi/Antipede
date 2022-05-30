using UnityEngine;

/// <summary>The base class for a weapon.</summary>
public abstract class Weapon : MonoBehaviour
{
	[HideInInspector] public MCentipedeWeapons Owner;

	[SerializeField, Tooltip("Where should projectiles shoot from?")] protected Transform BarrelEndSocket;
	[SerializeField, Tooltip("The " + nameof(Projectile) + " to Fire.")] protected Projectile ProjectileObject;
	public GameObject weaponPickup;
	public bool isAntGun = false;

	protected bool bIsRegistered = true;

	//                                          | UI    | PLAYER  | W. PICKUP | BOUNDARY
	public const int kIgnoreFromWeaponRaycasts = 1 << 5 | 1 << 6 | 1 << 9 | 1 << 11;

	/// <summary>Fires Projectile towards Position.</summary>
	/// <param name="Position">Intended target.</param>
	/// <returns>The <see cref="Projectile"/> gameobject that was fired.</returns>
	public abstract Projectile Fire(Vector3 Position);

	/// <summary>Make this Weapon look towards where it is firing.</summary>
	/// <param name="Direction">The direction to look at.</param>
	public virtual void LookAt(Vector3 Direction)
	{
		if (!bIsRegistered)
			return;

		if (Direction == Vector3.zero)
			return;
		if (transform.childCount > 0 && transform.GetChild(0).childCount > 0 && transform.GetChild(0).GetChild(0).childCount > 0)
			transform.GetChild(0).GetChild(0).GetChild(0).LookAt(Direction);
		else
			transform.LookAt(Direction);
	}

	/// <summary>Spawn a <see cref="Projectile"/> to fire.</summary>
	/// <returns>The newly spawned <see cref="Projectile"/> object for <see cref="Projectile.Launch(Vector3)"/>.</returns>
	protected Projectile InstantiateProjectile() { return Instantiate(ProjectileObject, BarrelEndSocket.position, transform.rotation); }

	/// <summary>Marks this Segment as 'Deregistered'. Ignores Firing / Look At calls.</summary>
	public virtual void Deregister() { bIsRegistered = false; }

	public override int GetHashCode() => name.GetHashCode();

	public virtual void OnAttatch() { }
}
