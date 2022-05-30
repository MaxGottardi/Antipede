﻿using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>The base class for a launched projectile from a <see cref="Weapon"/>.</summary>
/// <remarks>Default behaviour is a generic straight-line bullet.</remarks>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
	protected Rigidbody rb;
	public bool isEnemyProjectile = false; //is this projectile shot by an enemy or not
	public GameObject hitParticles, bloodParticles;
	public static bool hasSeenChanged = false;

	/// <remarks>Use as Awake/Start method.</remarks>
	public virtual void Initialise(bool isEnemyProjectile)
	{
		this.isEnemyProjectile = isEnemyProjectile;
		rb = GetComponent<Rigidbody>();
		Destroy(gameObject, 10f);
	}

	/// <summary>Launches this projectile at LaunchVelocity.</summary>
	/// <param name="LaunchVelocity">The magnitude and direction to launch this projectile.</param>
	public virtual void Launch(Vector3 LaunchVelocity)
	{
		rb.AddForce(LaunchVelocity);
	}

	private void OnCollisionEnter(Collision collision)
	{
		//Destroy(this);
		if (!isEnemyProjectile && collision.gameObject.CompareTag("Enemy"))
		{
			collision.transform.parent.gameObject.GetComponent<GenericAnt>().ReduceHealth(30);
			Instantiate(bloodParticles, transform.position + Vector3.up * 0.5f, Quaternion.identity);
			Destroy(gameObject);
		}
		if (!isEnemyProjectile && collision.gameObject.CompareTag("Tarantula")
			&& collision.gameObject.TryGetComponent(out Tarantula T) && T.healthSlider.value >= .5f)
		{
			T.DecreaseHealth();
			Instantiate(bloodParticles, transform.position + Vector3.up * 0.5f, Quaternion.identity);
		}
		else if (collision.gameObject.CompareTag("Play") && !hasSeenChanged)
		{
			hasSeenChanged = true;
			SceneManager.LoadScene("Environment Test");
		}
		else if (collision.gameObject.CompareTag("Back") && !hasSeenChanged)
		{
			hasSeenChanged = true;
			SceneManager.LoadScene("MainMenu");
		}
		else if (collision.gameObject.CompareTag("Credits") && !hasSeenChanged)
		{
			hasSeenChanged = true;
			SceneManager.LoadScene("Credits");
		}
		else if (collision.gameObject.CompareTag("Sound"))
		{
			UIManager.soundPanel.SetActive(true);
			UIManager.otherPanel.SetActive(false);
			UIManager.controlsPanel.SetActive(false);

			UIManager.RebindKeyPanel.SetActive(false);
			UIManager.enableKeyChange = false;
		}
		else if (collision.gameObject.CompareTag("Other"))
		{
			UIManager.soundPanel.SetActive(false);
			UIManager.otherPanel.SetActive(true);
			UIManager.controlsPanel.SetActive(false);
			UIManager.RebindKeyPanel.SetActive(false);
			UIManager.enableKeyChange = false;
		}
		else if (collision.gameObject.CompareTag("Controls"))
		{
			UIManager.soundPanel.SetActive(false);
			UIManager.otherPanel.SetActive(false);
			UIManager.controlsPanel.SetActive(true);

			UIManager.RebindKeyPanel.SetActive(false);
			UIManager.enableKeyChange = false;
		}
		else if (collision.gameObject.CompareTag("Settings") && !hasSeenChanged)
		{
			hasSeenChanged = true;
			SceneManager.LoadScene("SettingsScene");
		}
		else if (collision.gameObject.layer != LayerMask.NameToLayer("Projectile") && (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "SettingsScene"))
			Destroy(this);
		if (!isEnemyProjectile && collision.gameObject.CompareTag("Tarantula"))
		{
			Destroy(gameObject);
		}


	}

    private void OnTriggerEnter(Collider other)
    {
        if(isEnemyProjectile && other.gameObject.CompareTag("PlayerSegment"))
        {
			GameManager1.mCentipedeBody.RemoveSegment(30, transform.position + Vector3.up * 0.5f);
			Destroy(gameObject);
		}
	}
}