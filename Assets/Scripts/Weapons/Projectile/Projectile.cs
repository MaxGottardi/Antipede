using UnityEngine;
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
	public int DamageAmount, tarantDamage = 1;
	public bool isFlame = false;
	public int enemyCollisionCounter;

	/// <remarks>Use as Awake/Start method.</remarks>
	public virtual void Initialise(bool isEnemyProjectile)
	{
		enemyCollisionCounter = 0;
		this.isEnemyProjectile = isEnemyProjectile;
		rb = GetComponent<Rigidbody>();
		Destroy(gameObject, 10f);
	}

	public virtual void Initialise(bool isEnemyProjectile, bool isFlame)
	{
		enemyCollisionCounter = 0;
		this.isEnemyProjectile = isEnemyProjectile;
		this.isFlame = isFlame;
		rb = GetComponent<Rigidbody>();
		Destroy(gameObject, 10f);
	}

	/// <summary>Launches this projectile at LaunchVelocity.</summary>
	/// <param name="LaunchVelocity">The magnitude and direction to launch this projectile.</param>
	public virtual void Launch(Vector3 LaunchVelocity)
	{
		rb.AddForce(LaunchVelocity);
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		//Destroy(this);
		if (isFlame && !isEnemyProjectile && collision.gameObject.CompareTag("Enemy") && collision.transform.parent.gameObject.GetComponent<GuardAnt>() == null)
        {
			enemyCollisionCounter++;
			if (enemyCollisionCounter == 3)
            {
				//collide and destroy
				collision.transform.parent.gameObject.GetComponent<GenericAnt>().ReduceHealth(DamageAmount);
				Instantiate(bloodParticles, transform.position + Vector3.up * 0.5f, Quaternion.identity);
				Destroy(gameObject);
			}
			else
            {
				//collide and return
				collision.transform.parent.gameObject.GetComponent<GenericAnt>().ReduceHealth(DamageAmount);
				Instantiate(bloodParticles, transform.position + Vector3.up * 0.5f, Quaternion.identity);
				return;
			}
        }
		if (!isEnemyProjectile && collision.gameObject.CompareTag("Enemy") && collision.transform.parent.gameObject.GetComponent<GuardAnt>() == null)
		{
			collision.transform.parent.gameObject.GetComponent<GenericAnt>().ReduceHealth(DamageAmount);
			Instantiate(bloodParticles, transform.position + Vector3.up * 0.5f, Quaternion.identity);
			Destroy(gameObject);
		}
		if (!isEnemyProjectile && collision.gameObject.CompareTag("Tarantula")
			&& collision.gameObject.TryGetComponent(out Tarantula T) && T.healthSlider.value >= .5f)
		{
			T.DecreaseHealth(tarantDamage);
			Instantiate(bloodParticles, transform.position + Vector3.up * 0.5f, Quaternion.identity);
		}
		else if (collision.gameObject.CompareTag("Quit"))
		{
			Application.Quit();
		}
		else if (collision.gameObject.CompareTag("Play") && !hasSeenChanged)
		{
			hasSeenChanged = true;
			SceneManager.LoadScene("Environment Test");
		}
		else if (collision.gameObject.CompareTag("Back") && !hasSeenChanged)
		{
			hasSeenChanged = true;
			Camera.main.gameObject.GetComponent<CameraAnimate>().MoveToMainMenu();
			if(UIManager.enableKeyChange)
            {
				UIManager.RebindKeyPanel.SetActive(false);
				UIManager.enableKeyChange = false;
			}
			//SceneManager.LoadScene("MainMenu");
		}
		else if (collision.gameObject.CompareTag("Credits") && !hasSeenChanged)
		{
			hasSeenChanged = true;
			Camera.main.gameObject.GetComponent<CameraAnimate>().MoveToCredits();
			//SceneManager.LoadScene("Credits");
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
			Camera.main.gameObject.GetComponent<CameraAnimate>().MoveToSettings();
			//SceneManager.LoadScene("SettingsScene");
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
			GameManager1.mCentipedeBody.RemoveSegment(DamageAmount, transform.position + Vector3.up * 0.5f);
			Destroy(gameObject);
		}
	}
}