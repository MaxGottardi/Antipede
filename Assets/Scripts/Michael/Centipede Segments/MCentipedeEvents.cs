using UnityEngine;

[RequireComponent(typeof(MCentipedeBody))]
public class MCentipedeEvents : MonoBehaviour
{
	// MCentipedeBody Body;
	// void Awake() { Body = GetComponent<MCentipedeBody>(); }

	void OnTriggerEnter(Collider other)
	{
		// Handle Centipede Trigger Entries here...

		if (other.gameObject.CompareTag("Weapon Pickup"))
		{
			Debug.Log("Colledted Weapon");
			WeaponPickup PickedUp = other.gameObject.GetComponent<WeaponPickup>();
#if UNITY_EDITOR
			if (GameManager1.uiButtons)
			{
				GameManager1.uiButtons.ShootUI();
			}
			else
			{
				Debug.LogWarning("No " + nameof(GameManager1) + " " + nameof(GameManager1.uiButtons));
			}
#else
			GameManager1.uiButtons.ShootUI();
#endif

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
