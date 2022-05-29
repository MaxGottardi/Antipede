using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool backupPlayerExists = false;
    private GameObject player;
    private GameObject[] checkPoints;
    private bool updatedPlayer = false;
    private GameObject currentBackup;
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
        if (Input.GetKeyDown(KeyCode.X))
        {
            //Debug.Log(player.GetComponent<WeaponCardUI>().);
        }

        if (player == null)
        {
            player = GameObject.Find("Centipede");
        }

        if (currentBackup != null && (player.GetComponent<MCentipedeBody>().Segments.Count > currentBackup.GetComponent<MCentipedeBody>().Segments.Count
            || player.GetComponent<MCentipedeWeapons>().SegmentsWithWeapons.Count > currentBackup.GetComponent<MCentipedeWeapons>().SegmentsWithWeapons.Count))
        {
            player = GameObject.Find("Centipede");
            updatedPlayer = true;
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

            foreach (MSegment segment in player.GetComponent<MCentipedeBody>().Segments)
            {
                if (segment.Weapon != null)
                {
                    segment.Weapon.Owner = null;
                    segment.Weapon.isAntGun = false;
                    weapons.Add(segment.Weapon);
                }
            }

            foreach (WeaponAttachment weaponAttachment in GameObject.Find("Weapon Card System Interface").GetComponentsInChildren<WeaponAttachment>())
            {
                weaponAttachment.Attachment.Owner = null;
                weaponAttachment.Attachment.isAntGun = false;
                weapons.Add(weaponAttachment.Attachment);
            }

            GameObject backupPlayer = Instantiate(player, new Vector3(transform.position.x, transform.position.y - 5, transform.position.z), player.transform.rotation);
            backupPlayer.tag = "backup";
            backupPlayer.name = "backupPlayer";
            currentBackup = backupPlayer;
            backupPlayer.SetActive(false);
            player.GetComponent<MCentipedeBody>().newPlayer = backupPlayer;
            backupPlayerExists = true;
            updatedPlayer = false;
        }
    }

    public void SpawnBackupWeapons()
    {
        foreach (WeaponAttachment weaponAttachment in GameObject.Find("Weapon Card System Interface").GetComponentsInChildren<WeaponAttachment>())
        {
            WeaponCardUI.Sub(weaponAttachment.Attachment);
        }

        foreach (Weapon weapon in weapons)
        {
            weapon.Owner = null;
            weapon.isAntGun = false;
            WeaponCardUI.Add(weapon);
        }
    }
}
