using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>The base class for a launched projectile from a <see cref="Weapon"/>.</summary>
/// <remarks>Default behaviour is a generic straight-line bullet.</remarks>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
	/// Links because I'm too lazy to go through the folder structure.
	/// This file is the <see cref="Gun"/>'s Projectile.
	/// <see cref="LaunchedProjectile"/>
	/// <see cref="Shield"/>
	/// <see cref="XLine"/>

	/// <see cref="Weapon"/>

	/// Weapon is <see cref="Gun"/>

	protected Rigidbody rb;

	/// <summary><see langword="true"/> if this Projectile shot by an enemy.</summary>
	[Header("Inherited from " + nameof(Projectile))]

	public bool isEnemyProjectile = false;
	public GameObject hitParticles, bloodParticles;
	public static bool hasSeenChanged = false;
	public int DamageAmount, tarantDamage = 1;
	public bool isFlame = false;
	public int enemyCollisionCounter;

	bool bIgnoreAllCollions = false; //if hit something, say the terrain or something then ignore all collisions and do not give damage

	[SerializeField] MCentipedeBody body;

	/// <remarks>Use as Awake/Start method.</remarks>
	public virtual void Initialise(bool isEnemyProjectile, Collider firedObjCollider)
	{
		body = Object.FindObjectOfType<MCentipedeBody>();
		enemyCollisionCounter = 0;
		this.isEnemyProjectile = isEnemyProjectile;
		if(isEnemyProjectile && TryGetComponent(out Collider collider)) //ignore collison with the object that fires the bullet
        {
			Physics.IgnoreCollision(collider, firedObjCollider);
        }
		////////////////////////if(isEnemyProjectile)
  ////////////////////////      {
		////////////////////////	int safeEnemyLayer = LayerMask.NameToLayer("EnemySafeProjectile");
		////////////////////////	gameObject.layer = safeEnemyLayer;
  ////////////////////////      }
		////////////////////////else
  ////////////////////////      {
		////////////////////////	int safePlayerLayer = LayerMask.NameToLayer("PlayerSafeProjectile");
		////////////////////////	gameObject.layer = safePlayerLayer;
		////////////////////////}
		rb = GetComponent<Rigidbody>();
		Destroy(gameObject, 10f);
	}

	public virtual void Initialise(bool isEnemyProjectile, bool isFlame, Collider firedObjCollider)
	{
		Initialise(isEnemyProjectile, firedObjCollider);

		this.isFlame = isFlame;
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
		if (!bIgnoreAllCollions)
		{
			if (isFlame && !isEnemyProjectile && collision.gameObject.CompareTag("Enemy") && collision.transform.parent.gameObject.GetComponent<GuardAnt>() == null)
			{
				body.IncreaseMultiplier();
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
				body.IncreaseMultiplier();
				collision.transform.parent.gameObject.GetComponent<GenericAnt>().ReduceHealth(DamageAmount);
				Instantiate(bloodParticles, transform.position + Vector3.up * 0.5f, Quaternion.identity);
				Destroy(gameObject);
			}
			if (!isEnemyProjectile && collision.gameObject.CompareTag("Tarantula")
				&& collision.gameObject.TryGetComponent(out Tarantula T) && T.healthSlider.value >= .5f)
			{
				body.IncreaseMultiplier();
				T.DecreaseHealth(tarantDamage);
				Instantiate(bloodParticles, transform.position + Vector3.up * 0.5f, Quaternion.identity);
			}
			else if (collision.gameObject.CompareTag("Quit"))
			{
				Application.Quit();
			}
			else if (collision.gameObject.CompareTag("Play") && !hasSeenChanged)
			{
				Camera.main.gameObject.GetComponent<CameraAnimate>().MoveToSave();

				//hasSeenChanged = true;
				//SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
				//SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
			}
			else if (collision.gameObject.CompareTag("NewGame") && !hasSeenChanged)
			{
				hasSeenChanged = true;
				LoadingScene.nextScene = "IntroCutScene";
				LoadingScene.prevScene = "MainMenu";
				PersistentDataManager.bIsNewGame = true;
				SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
			}
			else if (collision.gameObject.CompareTag("Back") && !hasSeenChanged)
			{
				hasSeenChanged = true;
				Camera.main.gameObject.GetComponent<CameraAnimate>().MoveToMainMenu();
				if (UIManager.enableKeyChange)
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
				UIManager.graphicsPanel.SetActive(false);

				UIManager.RebindKeyPanel.SetActive(false);
				UIManager.enableKeyChange = false;
			}
			else if (collision.gameObject.CompareTag("Graphics"))
			{
				UIManager.soundPanel.SetActive(false);
				UIManager.otherPanel.SetActive(false);
				UIManager.controlsPanel.SetActive(false);
				UIManager.graphicsPanel.SetActive(true);

				UIManager.RebindKeyPanel.SetActive(false);
				UIManager.enableKeyChange = false;
			}
			else if (collision.gameObject.CompareTag("Other"))
			{
				UIManager.soundPanel.SetActive(false);
				UIManager.otherPanel.SetActive(true);
				UIManager.controlsPanel.SetActive(false);
				UIManager.RebindKeyPanel.SetActive(false);
				UIManager.graphicsPanel.SetActive(false);

				UIManager.enableKeyChange = false;
			}
			else if (collision.gameObject.CompareTag("Controls"))
			{
				UIManager.soundPanel.SetActive(false);
				UIManager.otherPanel.SetActive(false);
				UIManager.controlsPanel.SetActive(true);
				UIManager.graphicsPanel.SetActive(false);

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

			bIgnoreAllCollions = true;
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