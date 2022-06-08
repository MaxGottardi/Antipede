using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Weapon
{
	[Header("Shield Settings")]
	bool shieldActive;
	[SerializeField] SFXManager sfxManager;
	float shieldStartTime = 0;
	float shieldDuration;
	[SerializeField] GameObject centipede;
	MCentipedeBody mcb;
	[SerializeField] GameObject ShieldEffect;

	private void Awake()
	{
		sfxManager = FindObjectOfType<SFXManager>();
		mcb = GameManager1.mCentipedeBody;
		//mcb = centipede.GetComponent<MCentipedeBody>();
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
