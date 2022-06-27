#define WITH_MOUSE_HOVER_ANIMATIONS

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
#if WITH_MOUSE_HOVER_ANIMATIONS
using UnityEngine.UI;
#endif

/// <summary>Class that holds a reference to a pickup-able/droppable <see cref="Weapon"/> in the game.</summary>
public class WeaponAttachment : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	static Camera PrivateCamera;

	static Camera MainCamera
	{
		get
		{
			if (PrivateCamera)
				return PrivateCamera;
			PrivateCamera = Camera.main;
			return PrivateCamera;
		}
		set
		{
			PrivateCamera = value;
		}
	}

	[Tooltip("The Weapon that will be attached to the Centipede.")]
	[ReadOnly] public Weapon Attachment;

	[ReadOnly] public TextMeshProUGUI AttachUI;

	/// <summary>The Weapon currently being dragged; if it exists.</summary>
	Weapon DraggingAttachment;

	Vector3 PointUnderMouse;
	RaycastHit Hit;
	MSegment Segment;

	/// <summary><see langword="True"/> to disable <see cref="MInput.MouseToWorldCoords"/> by making it return <see cref="Vector3.zero"/>.</summary>
	public static bool bDisableWeaponFiring = false;

	bool bIsMouseHovering = false;

#if WITH_MOUSE_HOVER_ANIMATIONS
	RectTransform RT;

	Rect R;

	// Positions when increasing size.
	Vector3 OriginalPosition;
	Vector3 LerpTargetPosition;

	// Alpha-channels only.
	RawImage RI;
	Color OriginalColour;
	Color LerpTargetColour;
	
	// Original and Current Widths and Heights.
	Vector2 OWH;
	Vector2 WH;

	public const float kIncreaseBy = 100f;

	void Start()
	{
		CalculateTargets();
	}

	public void CalculateTargets()
	{
		RT = (RectTransform)transform;
		R = RT.rect;

		OWH = R.size;
		WH = new Vector2(OWH.x + kIncreaseBy, OWH.y + kIncreaseBy);

		OriginalPosition = RT.position;
		LerpTargetPosition = OriginalPosition;
		LerpTargetPosition.y += kIncreaseBy * .5f;
		RI = GetComponent<RawImage>();
		OriginalColour = RI.color;
		LerpTargetColour = new Color(OriginalColour.r, OriginalColour.g, OriginalColour.b, 1f);
	}

	float t = 0;
	Vector2 SizeNow;

	void Update()
	{
		if (bIsMouseHovering)
		{
			t += Time.deltaTime * 5;

			Vector2 Interp = Vector2.Lerp(SizeNow, WH, t);
			RT.position = Vector3.Lerp(RT.position, LerpTargetPosition, t);
			RI.color = Color.Lerp(RI.color, LerpTargetColour, t);

			RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Interp.x);
			RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Interp.y);
			SizeNow = Interp;
		}
		else
		{
			t -= Time.deltaTime * 5;

			Vector2 Interp = Vector2.Lerp(OWH, SizeNow, t);
			RT.position = Vector3.Lerp(OriginalPosition, RT.position, t);
			RI.color = Color.Lerp(OriginalColour, RI.color, t);

			RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Interp.x);
			RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Interp.y);
			SizeNow = Interp;
		}

		t = Mathf.Clamp01(t);
	}
#else
	public void CalculateTargets() { }
#endif // WITH_MOUSE_HOVER_ANIMATIONS

	void LateUpdate()
	{
		PointUnderMouse = CameraToWorld(out Hit);
	}

	public void OnPointerEnter(PointerEventData EventData)
	{
		bIsMouseHovering = true;
		bDisableWeaponFiring = true;
	}

	public void OnPointerExit(PointerEventData EventData)
	{
		bIsMouseHovering = false;
		bDisableWeaponFiring = false;
	}

	public void OnBeginDrag(PointerEventData EventData)
	{
		// ...

		DraggingAttachment = Instantiate(Attachment, PointUnderMouse, Quaternion.identity);
		////MeshRenderer MR = DraggingAttachment.GetComponent<MeshRenderer>();
		////Color RGB = MR.material.color;
		////MR.material.color = new Color(RGB.r, RGB.g, RGB.b, .5f);
	}

	public void OnDrag(PointerEventData EventData)
	{
		// ...

		if (TryGetSegment(ref Hit, out Segment)                // If the GameObject under the mouse has a Segment.
			&& !Segment.bIgnoreFromWeapons                          // If the Segment is NOT ignoring Weapons.
			&& Segment.TryGetWeaponSocket(out Transform Socket))    // If the Segment has a Weapon Socket.
		{
			DraggingAttachment.transform.position = Socket.position;
			DraggingAttachment.transform.rotation = Socket.rotation;
			DraggingAttachment.transform.parent = Socket;

			AttachUI.gameObject.SetActive(true);
			AttachUI.text = (Weapon)Segment == null ? "Attach!" : "Replace";
			AttachUI.rectTransform.position = Input.mousePosition + Vector3.up * 75f; // Mouse Position because I want to
												  // avoid deprojecting WorldToScreen.
		}
		else
		{
			DraggingAttachment.transform.position = Hit.point;

			DraggingAttachment.transform.parent = null;
			AttachUI.gameObject.SetActive(false);
		}

		bDisableWeaponFiring = true;
	}

	public void OnEndDrag(PointerEventData EventData)
	{
		// ...

		if (Segment)
		{
			if (!Segment.bIgnoreFromWeapons)
			{
				if ((Weapon)Segment == null)
				{
					Segment.SetWeapon(Attachment);
				}
				else
				{
					Segment.ReplaceWeapon(Attachment);
				}

				WeaponCardUI.Sub(Attachment);
			}
		}

		Destroy(DraggingAttachment.gameObject);
		DraggingAttachment = null;
		AttachUI.gameObject.SetActive(false);
		bDisableWeaponFiring = false;
	}

	static Vector3 CameraToWorld(out RaycastHit Hit)
	{
		Ray Deprojection = MainCamera.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(Deprojection, out Hit, MainCamera.farClipPlane);
		return Hit.point;
	}

	static bool TryGetSegment(ref RaycastHit Hit, out MSegment Segment)
	{
		if (Hit.collider)
			return Hit.collider.TryGetComponent(out Segment);

		Segment = null;
		return false;
	}

	public static implicit operator Transform(WeaponAttachment WA) => WA.transform;
	public static implicit operator RectTransform(WeaponAttachment WA) => (RectTransform)(Transform)WA;
}
