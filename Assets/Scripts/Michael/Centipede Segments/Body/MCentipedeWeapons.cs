using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MCentipedeBody))]
public class MCentipedeWeapons : MonoBehaviour
{

	// MCentipedeBody Body;

	[Header("Weapons Settings.")]
	[SerializeField] bool bUsePropagationDelay;
	[SerializeField] float PropagationDelay;

	[HideInInspector] public List<MSegment> SegmentsWithWeapons;

	Vector3 MouseToWorld;

	void Start()
	{
		// Body = GetComponent<MCentipedeBody>();
		SegmentsWithWeapons = new List<MSegment>();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (!bUsePropagationDelay)
			{
				foreach (Weapon W in SegmentsWithWeapons)
					W.Fire(MouseToWorld);
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

		if (SegmentsWithWeapons.Count > 0)
			UpdateWeaponOrientations(ref MouseToWorld);
	}

	void UpdateWeaponOrientations(ref Vector3 MouseToWorld)
	{
		foreach (Weapon W in SegmentsWithWeapons)
			W.LookAt(MouseToWorld);
	}

	IEnumerator IE_Fire()
	{
		foreach (Weapon W in SegmentsWithWeapons)
		{
				W.Fire(MouseToWorld);
				yield return new WaitForSeconds(PropagationDelay);
		}
	}
}
