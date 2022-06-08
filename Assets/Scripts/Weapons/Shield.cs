using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Weapon
{
	[Header("Shield Settings")]
	bool shieldActive;
	float shieldStartTime = 0;
	float shieldDuration;
	MCentipedeBody mcb;
	[SerializeField] GameObject ShieldEffect;

	public override void Awake()
	{
		base.Awake();

		mcb = GameManager1.mCentipedeBody;
		shieldActive = false;
	}

	void Update()
	{
		mcb.shieldActive = shieldActive;

		if (Input.GetKeyDown(KeyCode.Y))
		{
			ActivateShield(15.0f);
		}

		if (shieldStartTime > 0)
		{
			if (Time.time <= shieldStartTime + shieldDuration)
			{
				shieldActive = true;
			}
			else
			{
				DeactivateShield();
			}
		}
	}

	public override Projectile Fire(Vector3 Position) => null;

	public override void LookAt(Vector3 Direction) { /* do nothing instead */ }

	public void ActivateShield(float duration)
	{
		ShieldEffect.SetActive(true);
		shieldDuration = duration;
		sfxManager.ActivateShield();
		shieldStartTime = Time.time;
	}

	public void DeactivateShield()
	{
		Debug.Log("dsfsdfs");
		shieldStartTime = 0;
		shieldActive = false;
		sfxManager.DeactivateShield();
		ShieldEffect.SetActive(false);

		Owner.DetachWeapon();
	}

	public override void OnAttach(MSegment Parent)
	{
		base.OnAttach(Parent);

		ActivateShield(5.0f);
	}
}
