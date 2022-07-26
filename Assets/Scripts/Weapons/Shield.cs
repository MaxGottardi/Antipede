using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Weapon
{
	[Header("Shield Settings")]
	bool shieldActive;
	public float shieldStartTime = 0;
	float shieldDuration;
	MCentipedeBody mcb;
	[SerializeField] GameObject ShieldEffect;

	public override void Awake()
	{
		base.Awake();
		shieldActive = false;
	}
    private void Start()
    {
		mcb = GameManager1.mCentipedeBody;
	}
	void Update()
	{
		mcb.shieldActive = shieldActive;

		//if (Input.GetKeyDown(KeyCode.Y))
		//{
		//	ActivateShield(15.0f);
		//}

		if (shieldStartTime > 0)
		{
			shieldStartTime -= Time.deltaTime;
			if (shieldStartTime > 0)
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

		shieldStartTime = shieldDuration;
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
