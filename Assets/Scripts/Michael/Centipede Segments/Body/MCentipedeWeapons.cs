using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MCentipedeBody))]
public class MCentipedeWeapons : MonoBehaviour
{

	MCentipedeBody Body;

	[Header("Weapons Settings.")]
	[SerializeField] bool bUsePropagationDelay;
	[SerializeField] float PropagationDelay;

	Vector3 MouseToWorld;

	void Start()
	{
		Body = GetComponent<MCentipedeBody>();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (!bUsePropagationDelay)
			{
				foreach (Weapon W in Body)
				{
					W?.Fire(W.transform.forward);
				}
			}
			else
			{
				StartCoroutine(IE_Fire());
			}
		}
	}

	public void ReceiveMouseCoords(Vector3 MouseToWorld)
	{
		this.MouseToWorld = MouseToWorld;
				Debug.DrawLine(MouseToWorld + Vector3.up * 2, MouseToWorld, Color.cyan);
		UpdateWeaponOrientations(ref MouseToWorld);
	}

	void UpdateWeaponOrientations(ref Vector3 MouseToWorld)
	{
		foreach (Weapon W in Body)
		{
			if (W)
			{
				W.LookAt(MouseToWorld);
			}
		}
	}

	IEnumerator IE_Fire()
	{
		MSegment[] Segments = Body.GetSegments();

		foreach (Weapon W in Segments)
		{
			if (W)
			{
				W.Fire(MouseToWorld);
				yield return new WaitForSeconds(PropagationDelay);
			}
		}
	}
}
