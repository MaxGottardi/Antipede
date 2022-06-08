using UnityEngine;

/// <summary>The base class for a weapon.</summary>
public abstract class Weapon : MonoBehaviour
{
	[Header("Base Weapon Settings.")]
	[HideInInspector] public MCentipedeWeapons WeaponsComponent;

	[SerializeField, Tooltip("Where should projectiles shoot from?")] protected Transform BarrelEndSocket;
	[SerializeField, Tooltip("The " + nameof(Projectile) + " to Fire.")] protected Projectile ProjectileObject;
	public GameObject weaponPickup;
	public bool isAntGun = false;
	[Tooltip("This Weapon can fire at this rate per second."), Min(0)] public float FireRate = .1f;
	protected float TimeLastFired = 0;

	[Header("Weapon Card UI References.")]
	public Texture2D Art;
	public Material ArtMaterial;
	public Color TextColour;

	/// <summary><see langword="true"/> if this Weapon is accepting Firing commands and is attached to an <see cref="MSegment"/>.</summary>
	protected bool bIsRegistered = true;
	/// <summary>The <see cref="MSegment"/> that controls this Weapon.</summary>
	protected MSegment Owner;

	//                                          | UI    | PLAYER | W. PICKUP | BOUNDARY
	public const int kIgnoreFromWeaponRaycasts = 1 << 5 | 1 << 6 | 1 << 9    | 1 << 11;

	/// <summary>Fires Projectile towards Position.</summary>
	/// <param name="Position">Intended target.</param>
	/// <returns>The <see cref="Projectile"/> gameobject that was fired.</returns>
	public abstract Projectile Fire(Vector3 Position);

	/// <returns><see langword="true"/> if the time between now and <see cref="TimeLastFired"/> is &gt; <see cref="FireRate"/>.</returns>
	protected bool CanFire()
	{
		bool bCanFire = bIsRegistered && Time.time - TimeLastFired > FireRate;
		if (bCanFire)
			TimeLastFired = Time.time;

		return bCanFire;
	}

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

	/// <summary>Called when this <see cref="Weapon"/> is attached onto an <see cref="MSegment"/>.</summary>
	/// <param name="Parent">The <see cref="MSegment"/> this Weapon is attached to.</param>
	public virtual void OnAttach(MSegment Parent) { Owner = Parent; }
}
