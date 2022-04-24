using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{

	public Weapon Weapon;

    private void Update()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0, Space.Self);
    }
}
