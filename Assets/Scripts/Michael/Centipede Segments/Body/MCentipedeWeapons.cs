//#define SHOW_WEAPON_RANGE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(MCentipedeBody))]
public class MCentipedeWeapons : MonoBehaviour
{

	// MCentipedeBody Body;

	[Header("Weapons Settings.")]
	[SerializeField] bool bUsePropagationDelay;
	[SerializeField] float PropagationDelay;

#if SHOW_WEAPON_RANGE
	[Header("Weapons HUD")]
	[SerializeField] TextMeshProUGUI HUD;
#endif

	[HideInInspector] public List<MSegment> SegmentsWithWeapons;
	bool bHasWeapons;

	/// <summary>The World Position under the Mouse</summary>
	/// <remarks>Vector3.zero when nothing is under the mouse.</remarks>
	Vector3 MouseToWorld;
	bool bHasLineOfSight;

	bool bIsFireActive = false;

	void Start()
	{
		// Body = GetComponent<MCentipedeBody>();
		SegmentsWithWeapons = new List<MSegment>();
	}

	void Update()
	{
		bHasWeapons = SegmentsWithWeapons.Count > 0;

		if (bHasWeapons)
		{
			bHasLineOfSight = HasLineOfSight();

			//if enabled set the toggle for automatically firing when the mouse key is pressed
			if (Input.GetMouseButtonDown(0) && SettingsVariables.boolDictionary["bWeaponToggle"])
			{
				if (bIsFireActive)
					bIsFireActive = false;
				else
					bIsFireActive = true;
			}

			if (bHasLineOfSight && (Input.GetMouseButton(0) || bIsFireActive && SettingsVariables.boolDictionary["bWeaponToggle"]))
			{
				if (!bUsePropagationDelay)
				{
					foreach (Weapon W in SegmentsWithWeapons)
						W.Fire(MouseToWorld);
				}
				else
				{
					StartCoroutine(FireWithPropagation());
				}
			}
		}

		if (Input.GetMouseButtonUp(0))
        {
			foreach (Weapon W in SegmentsWithWeapons)
				W.Deactivate();
		}

#if SHOW_WEAPON_RANGE
		if (HUD)
			UpdateWeaponHUD();
#endif

		//if auto firing and the toggle is disabled, stop auto firing
		if (bIsFireActive && !SettingsVariables.boolDictionary["bWeaponToggle"])
			bIsFireActive = false;
	}

	static Vector3 ZeroVector = Vector3.zero;

	public void ReceiveMouseCoords(Vector3 MouseToWorld)
	{
		this.MouseToWorld = MouseToWorld;
		Debug.DrawLine(MouseToWorld + Vector3.up * 2, MouseToWorld, Color.cyan);

		if (SegmentsWithWeapons.Count > 0 && MouseToWorld != Vector3.zero && bHasLineOfSight)
			UpdateWeaponOrientations(ref MouseToWorld);
		else
			UpdateWeaponOrientations(ref ZeroVector);
	}

	void UpdateWeaponOrientations(ref Vector3 MouseToWorld)
	{
		foreach (Weapon W in SegmentsWithWeapons)
			W.LookAt(MouseToWorld);
	}

	bool HasLineOfSight()
	{
		// Cast a ray between the Centipede's position and the position under the mouse.
		// Checking if there's anything in between these two positions sometimes returns true
		// because it treats the position under the mouse as an intercept sometimes.
		//
		// This Offset should prevent that by adding an Up off the ground.

		//                                                                               Trick to avoid Normalising a Vector
		return !Physics.Linecast(transform.position + transform.up * .5f, MouseToWorld + (transform.position - MouseToWorld) * .1f, 256) &&
			MouseToWorld != Vector3.zero; // There is nothing under the mouse. The mouse is pointing at the sky / nothing.
	}

	IEnumerator FireWithPropagation()
	{
		// If an InvalidOperationException is thrown here. It's because either the player has
		// dragged a Weapon onto a Segment while firing.
		// -- OR --
		// if the Centipede has lost a Segment while firing.
		//
		// This only happens when bUsePropagationDelay is on; switching it off loses the delay,
		// so all Weapons fire simultaneously.
		//
		// Apparently you can't use a try-catch block on an IEnumerator.
		// It sounds bad, but just ignore it; it cancels the firing anyway.
		//
		// It *should* be fixed. See MSegment.Deregister() and its call to MCentipedeWeapons.StopAllCoroutines().

		foreach (Weapon W in SegmentsWithWeapons)
		{
			W.Fire(MouseToWorld);
			yield return new WaitForSeconds(PropagationDelay);
		}
	}

#if SHOW_WEAPON_RANGE
	static readonly Color DistanceColour = new Color(1, 1, 1, .5f);
	static readonly Color NoSightColour = new Color(1, 0, 0, .5f);

	void UpdateWeaponHUD()
	{
		if (!bHasWeapons || MouseToWorld == Vector3.zero)
		{
			HUD.text = "";
			return;
		}

		Vector3 MouseScreenPosition = Input.mousePosition;

		if (MouseScreenPosition.x >= Screen.width * .8f)
			HUD.rectTransform.pivot = new Vector2(.5f, 0);
		else if (MouseScreenPosition.x < Screen.width * .2f)
			HUD.rectTransform.pivot = Vector2.zero;

		HUD.rectTransform.position = MouseScreenPosition;
		if (bHasLineOfSight)
		{
			HUD.text = "Distance: " + Vector3.Distance(transform.position, MouseToWorld).ToString("F0") + "m";
			HUD.color = DistanceColour;
		}
		else
		{
			HUD.text = "NO LINE OF SIGHT";
			HUD.color = NoSightColour;
		}
	}
#endif
}
