using UnityEngine;

[RequireComponent(typeof(MCentipedeBody))]
public class MCentipedeEvents : MonoBehaviour
{
	// MCentipedeBody Body;
	// void Awake() { Body = GetComponent<MCentipedeBody>(); }

	void OnTriggerEnter(Collider other)
	{
		// Handle Centipede Trigger Entries here...

		if (other.CompareTag("Weapon Pickup"))
		{
			WeaponPickup PickedUp = other.GetComponent<WeaponPickup>();

			if (PickedUp != null)
			{
				WeaponCardUI.Add(PickedUp.Weapon);
			}
			else
			{
				Debug.LogError("Weapon Pickup has no WeaponPickup Component: " + other.name);
			}

			Destroy(other.gameObject);
		}
	}
}
