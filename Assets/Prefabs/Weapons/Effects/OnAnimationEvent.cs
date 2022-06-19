using UnityEngine;

/// <summary>Attach onto anything with an <see cref="Animator"/> and call this script's functions.</summary>
[RequireComponent(typeof(Animator))]
public class OnAnimationEvent : MonoBehaviour
{
	public void DestroyThis()
	{
		Destroy(gameObject);
	}
}
