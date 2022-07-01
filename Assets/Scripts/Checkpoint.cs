using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public bool backupPlayerExists = false;
	private GameObject player;
	private GameObject[] checkPoints;
	private bool updatedPlayer = false;
	private MCentipedeBody currentBackup;
	private List<Weapon> weapons = new List<Weapon>();


	// Start is called before the first frame update
	void Start()
	{
		player = GameObject.Find("Centipede");
		checkPoints = GameObject.FindGameObjectsWithTag("CheckPoint");
	}

	// Update is called once per frame
	void Update()
	{
		if (player == null)
		{
			player = GameObject.Find("Centipede");
		}

		if (currentBackup != null)
		{
			MCentipedeBody Player = player.GetComponent<MCentipedeBody>();
			MCentipedeBody Backup = currentBackup.GetComponent<MCentipedeBody>();

			if (Player && Backup)
			{
				if (Player.Segments.Count > Backup.Segments.Count
				    || Player.Weapons.SegmentsWithWeapons.Count > Backup.Weapons.SegmentsWithWeapons.Count)
				{
					player = GameObject.Find("Centipede");
					updatedPlayer = true;
				}
			}
		}

	}

	private void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.CompareTag("PlayerSegment") && (backupPlayerExists == false || updatedPlayer == true))
		{
			if (updatedPlayer == true)
			{
				foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
				{
					if (gameObject.name == "backupPlayer")
					{
						Destroy(gameObject);
					}
				}
			}
			weapons.Clear();

			if (player && player.TryGetComponent(out MCentipedeBody Body))
			{
				foreach (MSegment segment in Body.Segments)
				{
					if (segment.Weapon != null)
					{
						Weapon W = Instantiate(segment.Weapon);

						if (W.TryGetComponent(out LineRenderer Arc))
							Arc.enabled = false;

						weapons.Add(W);
					}
				}

				if (Body)
				{
					MCentipedeBody backupPlayer = Instantiate(Body, new Vector3(transform.position.x, transform.position.y - 5, transform.position.z), player.transform.rotation);
					backupPlayer.tag = "backup";
					backupPlayer.name = "backupPlayer";
					currentBackup = backupPlayer;
					backupPlayer.gameObject.SetActive(false);
					MCentipedeBody.newPlayer = backupPlayer;
					backupPlayerExists = true;
					updatedPlayer = false;
				}
			}

			// This loop and the same loop in SpawnBackupWeapons() counteract each other.
			//foreach (WeaponAttachment weaponAttachment in GameObject.Find("Weapon Card System Interface").GetComponentsInChildren<WeaponAttachment>())
			//{
			//	weaponAttachment.Attachment.Owner = null;
			//	weaponAttachment.Attachment.isAntGun = false;
			//	weapons.Add(weaponAttachment.Attachment);
			//}
		}
	}

	/// <summary>Adds the <see cref="Weapon"/>s that were previously attached before reaching the Checkpoint to <see cref="WeaponCardUI"/>.</summary>
	public void SpawnBackupWeapons()
	{
		// Comment this loop to maintain the Weapon Cards in the UI.
		// Uncomment this loop to remove all unattached Weapons.
		WeaponCardUI.RemoveAll();

		foreach (Weapon weapon in weapons)
		{
			WeaponCardUI.Add(weapon.weaponPickup.GetComponent<WeaponPickup>().Weapon);
		}
	}
}
