using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

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
	public Weapon Attachment;

	public TextMeshProUGUI AttachUI;

	/// <summary>The Weapon currently being dragged; if it exists.</summary>
	Weapon DraggingAttachment;

	Vector3 PointUnderMouse;
	RaycastHit Hit;

	void LateUpdate()
	{
		PointUnderMouse = CameraToWorld(out Hit);
	}

	#region Maybe Unused Events

	public void OnPointerClick(PointerEventData EventData) {  }

	public void OnPointerDown(PointerEventData EventData) {  }

	public void OnPointerEnter(PointerEventData EventData) {  }

	public void OnPointerExit(PointerEventData EventData) {  }

	public void OnPointerUp(PointerEventData EventData) {  }

	#endregion

	public void OnBeginDrag(PointerEventData EventData)
	{
		// ...

		DraggingAttachment = Instantiate(Attachment, PointUnderMouse, Quaternion.identity);
		MeshRenderer MR = DraggingAttachment.GetComponent<MeshRenderer>();
		Color RGB = MR.material.color;
		MR.material.color = new Color(RGB.r, RGB.g, RGB.b, .5f);
	}

	public void OnDrag(PointerEventData EventData)
	{
		// ...

		if (TryGetSegment(ref Hit, out MSegment Segment)                // If the GameObject under the mouse has a Segment.
			&& !Segment.bIgnoreFromWeapons                          // If the Segment is NOT ignoring Weapons.
			&& Segment.TryGetWeaponSocket(out Transform Socket))    // If the Segment has a Weapon Socket.
		{
			DraggingAttachment.transform.position = Socket.position;
			DraggingAttachment.transform.parent = Socket;

			AttachUI.gameObject.SetActive(true);
			AttachUI.text = (Weapon)Segment == null ? "Attach!" : "Replace";
			AttachUI.rectTransform.position = Input.mousePosition + Vector3.up * 75f;
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

		if (TryGetSegment(ref Hit, out MSegment Segment))
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
