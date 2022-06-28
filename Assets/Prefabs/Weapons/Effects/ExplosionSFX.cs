using UnityEngine;

/// <summary>Simple Explosion SFX Controller.</summary>
[RequireComponent(typeof(AudioSource))]
public class ExplosionSFX : MonoBehaviour
{

	[SerializeField] AudioClip[] Explosions;
	[SerializeField, ReadOnly] AudioSource Source;

	void Start()
	{
		if (!Source)
			Source = GetComponent<AudioSource>();
		Source.clip = Explosions[Random.Range(0, Explosions.Length - 1)];
		Source.Play();

		Destroy(gameObject, Source.clip.length);
	}

	void OnValidate()
	{
		Source = GetComponent<AudioSource>();
	}
}
