using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeaponPickup : MonoBehaviour
{

	public bool isLaser, isGun, isShield, isLauncher, isFlame;
	public Weapon Weapon;

	float InitialHeight;
	const float kBobHeight = .25f;

	void Start()
	{
		InitialHeight = transform.position.y * .5f;
	}

	void Update()
	{
		transform.Rotate(0, 50 * Time.deltaTime, 0, Space.Self);
		transform.position = new Vector3(transform.position.x, (Mathf.Sin(Time.time) * kBobHeight + 1) * .5f + 0.8f, transform.position.z);
	}
}
