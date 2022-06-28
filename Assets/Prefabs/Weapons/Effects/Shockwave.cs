using UnityEngine;

public class Shockwave : MonoBehaviour
{
	[SerializeField] ExplosionSFX SFX;

	void Start()
	{
		Instantiate(SFX, transform.position, Quaternion.identity);
	}
}
