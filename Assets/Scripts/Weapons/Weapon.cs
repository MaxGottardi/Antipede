//#define OVERRIDE_FIRE_RATE

using UnityEngine;

/// <summary>The base class for a weapon.</summary>
public abstract class Weapon : MonoBehaviour
{
	/// Links because I'm too lazy to go through the folder structure.
	/// <see cref="Gun"/>
	/// <see cref="Launcher"/>
	/// <see cref="Shield"/>
	/// <see cref="Laser"/>
	/// 
	/// <see cref="Projectile"/>

	[HideInInspector] public MCentipedeWeapons WeaponsComponent;

	[Header("Base Weapon Settings.")]
	[SerializeField, Tooltip("Where should projectiles shoot from?")] protected Transform BarrelEndSocket;
	[SerializeField, Tooltip("The " + nameof(Projectile) + " to Fire.")] protected Projectile ProjectileObject;
	public GameObject weaponPickup;
	public bool isAntGun = false;
	[SerializeField, Tooltip("This Weapon can fire at this rate per second."), Min(0)] float FireRate = .1f;
	protected float TimeLastFired = 0;
	[SerializeField, Tooltip("The Range of this Weapon"), Min(1)] float Range = 100f;

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

	[SerializeField, ReadOnly] public SFXManager sfxManager;

	public virtual void Awake()
	{
		sfxManager = FindObjectOfType<SFXManager>();
	}

	/// <summary>Fires Projectile towards Position.</summary>
	/// <param name="Position">Intended target.</param>
	/// <returns>The <see cref="Projectile"/> gameobject that was fired.</returns>
	public abstract Projectile Fire(Vector3 Position);

	/// <param name="Position">Intended target.</param>
	/// <returns><see langword="true"/> if:
	/// <br>
	/// The time between now and <see cref="TimeLastFired"/> is &gt; <see cref="FireRate"/>.
	/// </br>
	/// <br>&amp;&amp;</br>
	/// <br>
	/// The distance between <see cref="BarrelEndSocket"/> and the Target Position &lt;= <see cref="Range"/>.
	/// </br>
	/// </returns>
	protected bool CanFire(Vector3 Position)
	{
#if OVERRIDE_FIRE_RATE
		bool bCanFire = true;
#else
		// Has this Weapon cooled down?
		bool bCanFire = bIsRegistered && Time.time - TimeLastFired > FireRate;
#endif

		// Is this Weapon In-Range?
		bCanFire &= InRange(ref Position);

		return bCanFire;
	}

	protected bool InRange(ref Vector3 Position)
	{
		return MMathStatics.HasReached(BarrelEndSocket.position, Position, Range);
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
	protected Projectile InstantiateProjectile()
	{
		TimeLastFired = Time.time;

		return Instantiate(ProjectileObject, BarrelEndSocket.position, transform.rotation);
	}

	/// <summary>Marks this Segment as 'Deregistered'. Ignores Firing / Look At calls.</summary>
	public virtual void Deregister() { bIsRegistered = false; }

	public override int GetHashCode() => name.GetHashCode();

	/// <summary>Called when this <see cref="Weapon"/> is attached onto an <see cref="MSegment"/>.</summary>
	/// <param name="Parent">The <see cref="MSegment"/> this Weapon is attached to.</param>
	public virtual void OnAttach(MSegment Parent) { Owner = Parent; }

	/// <summary>Deactivates weapons that are intended to be fired constantly</summary>
	public virtual void Deactivate() { }


#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (!isAntGun && BarrelEndSocket)
		{
			Gizmos.color = TextColour;
			Gizmos.DrawWireSphere(BarrelEndSocket.position, Range);
		}
	}
#endif
}
