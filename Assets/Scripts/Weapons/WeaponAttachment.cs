using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

/// <summary>Class that holds a reference to a pickup-able/droppable <see cref="Weapon"/> in the game.</summary>
public class WeaponAttachment : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
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

	RectTransform RT;
	Rect R;
	bool bIsMouseHovering = false;
	float t = 0;
	Vector3 OriginalPosition;
	Vector3 LerpTargetPosition;
	RawImage RI;
	Color OriginalColour;
	Color LerpTargetColour;
	Vector2 OWH;
	Vector2 WH;

	public const float kIncreaseBy = 100f;

	void Start()
	{
		CalculateTargets();
	}

	public void CalculateTargets()
	{
		if (!K_bAnimateMouseHovering)
			return;

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

	Vector2 SizeNow;
	// Set to false if you don't the Cards to animate.
	const bool K_bAnimateMouseHovering = true;

	void Update()
	{
		if (!K_bAnimateMouseHovering)
			return;

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

	void LateUpdate()
	{
		PointUnderMouse = CameraToWorld(out Hit);
	}

	#region Maybe Unused Events

	public void OnPointerClick(PointerEventData EventData) { }

	public void OnPointerDown(PointerEventData EventData) { }

	public void OnPointerEnter(PointerEventData EventData) { bIsMouseHovering = true; }

	public void OnPointerExit(PointerEventData EventData) { bIsMouseHovering = false; }

	public void OnPointerUp(PointerEventData EventData) { }

	#endregion

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
