using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponCardUI : MonoBehaviour
{
	public static WeaponCardUI Instance;


	[SerializeField] WeaponAttachment TemplateCard;
	[SerializeField] float PaddingBetweenCards;
	float Alpha;

	[SerializeField] TextMeshProUGUI AttachUI;

	[SerializeField] Weapon[] WeaponCheats;

	static Dictionary<Weapon, AttachmentUIInfo> WeaponsInventory;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Debug.LogWarning("Ensure there is only one " + nameof(WeaponCardUI) + " in the scene!" +
				"\nInstance is: " + Instance.name + ".\tDuplicate is: " + name);
			Destroy(gameObject);
		}
	}

	void Start()
	{
		WeaponsInventory = new Dictionary<Weapon, AttachmentUIInfo>();
		Alpha = TemplateCard.GetComponent<RawImage>().color.a;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Slash) && Input.GetKey(KeyCode.RightShift))
		{
			foreach (Weapon W in WeaponCheats)
			{
				Add(W);
			}
		}
	}

	/// <summary>Add Weapon to the Inventory.</summary>
	/// <param name="Weapon">The Weapon that was picked up.</param>
	public static WeaponAttachment Add(Weapon Weapon)
	{
		return Instance.RegisterWeapon(Weapon);
	}

	/// <summary>Register Weapon as being 'Used'.</summary>
	/// <param name="Weapon">The Weapon that was dragged and dropped onto a Segment.</param>
	public static void Sub(Weapon Weapon)
	{
		Debug.Assert(WeaponsInventory.ContainsKey(Weapon), "Attempt at removing a Weapon that doesn't exist in the Card UI.");

		AttachmentUIInfo AUII = WeaponsInventory[Weapon];
		AUII.Remaining--;
		WeaponsInventory[Weapon] = AUII;

		if (AUII.Remaining <= 0)
		{
			if (WeaponsInventory.Count > 1)
			{
				int IndexToShuffle = AUII.PositionIndex;
				List<KeyValuePair<Weapon, AttachmentUIInfo>> Modified = new List<KeyValuePair<Weapon, AttachmentUIInfo>>();

				// For each Entry in the Weapons Inventory.
				foreach (KeyValuePair<Weapon, AttachmentUIInfo> E in WeaponsInventory)
				{
					if (E.Value.PositionIndex <= IndexToShuffle)
						continue;

					// Get the size and padding according to the template.
					Instance.GetPositionInfo(out _, out Vector2 SizeOfTemplate, out float AdjustedPadding);
					// Assign the index position one away from the index being removed.
					Vector3 ShuffledPosition = GetPositionFromInfo(E.Value.PositionIndex - 1, SizeOfTemplate, AdjustedPadding);

					// Update the UI-position and index.
					E.Value.Card.position = ShuffledPosition;
					AttachmentUIInfo EAUII = E.Value;
					EAUII.PositionIndex--;

					E.Value.WeaponAttachment.CalculateTargets();

					// Mark this Entry for modification.
					Modified.Add(new KeyValuePair<Weapon, AttachmentUIInfo>(E.Key, EAUII));
				}

				// Apply modifications.
				foreach (KeyValuePair<Weapon, AttachmentUIInfo> E in Modified)
					WeaponsInventory[E.Key] = E.Value;
			}

			// Remove and Destroy this Weapon from the Weapons Inventory and the game.
			WeaponsInventory.Remove(Weapon);
			Destroy(AUII.Card.gameObject);
		}
		else
		{
			Instance.UpdateText(ref AUII);
		}
	}

	public static void RemoveAll()
	{
		int C = WeaponsInventory.Count;
		KeyValuePair<Weapon, AttachmentUIInfo>[] ForUpdateAndRemove = new KeyValuePair<Weapon, AttachmentUIInfo>[C];
		int U = 0;

		foreach (KeyValuePair<Weapon, AttachmentUIInfo> WAUII in WeaponsInventory)
		{
			ForUpdateAndRemove[U++] = WAUII;
		}

		for (int i = 0; i < C; ++i)
		{
			AttachmentUIInfo T = ForUpdateAndRemove[i].Value;
			T.Remaining = 0;
			WeaponsInventory[ForUpdateAndRemove[i].Key] = T;
			Sub(ForUpdateAndRemove[i].Key);
		}
	}

	WeaponAttachment RegisterWeapon(Weapon PickedUp)
	{
		if (WeaponsInventory.ContainsKey(PickedUp))
		{
			// If the Picked Up Weapon already has Inventory, add another to the Inventory.

			AttachmentUIInfo AUII = WeaponsInventory[PickedUp];
			AUII.Remaining++;
			UpdateText(ref AUII);
			WeaponsInventory[PickedUp] = AUII;

			return AUII.WeaponAttachment;
		}
		else
		{
			// This Weapon does NOT exist in the Inventory; add it.

			// Get the relevant information to calculate Card positions.
			GetPositionInfo(out int NoOfCards, out Vector2 SizeOfTemplate, out float AdjustedPadding);

			// Spawn a new Card with the above info.
			WeaponAttachment NewCard = Instantiate(TemplateCard, GetPositionFromInfo(NoOfCards, SizeOfTemplate, AdjustedPadding), Quaternion.identity);
			NewCard.transform.SetParent(transform);
			NewCard.AttachUI = AttachUI;

			RawImage RI = NewCard.GetComponent<RawImage>();
			RI.texture = PickedUp.Art;
			RI.material = PickedUp.ArtMaterial;

			TextMeshProUGUI Text = NewCard.GetComponentInChildren<TextMeshProUGUI>();
			Text.color = PickedUp.TextColour;

			// Mark as having one Inventory and initialise the Card.
			AttachmentUIInfo AUII = new AttachmentUIInfo(1, NewCard, PickedUp, NoOfCards);
			NewCard.name = AUII.WeaponAttachment.Attachment.name + " Card";

			WeaponsInventory.Add(PickedUp, AUII);
			UpdateText(ref AUII);

			// Update the colour of the Card to match the Weapon GameObject.
			//			Color WeaponColour = AUII.WeaponAttachment.Attachment.GetComponent<MeshRenderer>().sharedMaterial.color;
			//			AUII.Background.color = new Color(WeaponColour.r, WeaponColour.g, WeaponColour.b, Alpha);

			return NewCard;
		}
	}

	void GetPositionInfo(out int NoOfCards, out Vector2 SizeOfTemplate, out float AdjustedPadding)
	{
		NoOfCards = WeaponsInventory.Count;
		SizeOfTemplate = new Vector2(((RectTransform)TemplateCard).rect.width, ((RectTransform)TemplateCard).rect.height) * .5f;
		AdjustedPadding = -SizeOfTemplate.x * 2 - PaddingBetweenCards;
	}

	static Vector3 GetPositionFromInfo(int NoOfCards, Vector2 SizeOfTemplate, float AdjustedPadding)
	{
		return new Vector3(2f * SizeOfTemplate.x - AdjustedPadding * NoOfCards, SizeOfTemplate.y);
	}

	void UpdateText(ref AttachmentUIInfo AUII)
	{
		Debug.Log("updatetext");
		//AUII.Update(AUII.WeaponAttachment.Attachment.name + "\nx" + AUII.Remaining);
		AUII.Update("x" + AUII.Remaining);
	}

	struct AttachmentUIInfo
	{
		/// <summary>The number of this type of Weapon in the Inventory.</summary>
		public int Remaining;
		/// <summary>The <see cref="RectTransform"/> of the Card UI.</summary>
		public RectTransform Card;
		/// <summary>The <see cref="TextMeshProUGUI"/> of this Card.</summary>
		public TextMeshProUGUI Text;
		/// <summary>The background <see cref="RawImage"/> of this Card.</summary>
		public RawImage Background;
		/// <summary>The draggable <see cref="Weapon"/> associated with this Card.</summary>
		public WeaponAttachment WeaponAttachment;
		/// <summary>The position of this Card on the screen (UI) as an index.</summary>
		public int PositionIndex;

		public AttachmentUIInfo(int Remaining, RectTransform Card, Weapon Weapon, int DictionaryIndex)
		{
			this.Remaining = Remaining;
			this.Card = Card;
			Text = Card.GetComponentInChildren<TextMeshProUGUI>();
			Background = Card.GetComponent<RawImage>();
			WeaponAttachment = Card.GetComponent<WeaponAttachment>();
			WeaponAttachment.Attachment = Weapon;
			this.PositionIndex = DictionaryIndex;
		}

		public void Update(string S)
		{
			Text.text = S;
		}
	}
}