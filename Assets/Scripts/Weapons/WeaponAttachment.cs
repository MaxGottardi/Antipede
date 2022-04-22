using UnityEngine;
using UnityEngine.EventSystems;

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

	/// <summary>The Weapon currently being dragged; if it exists.</summary>
	Weapon DraggingAttachment;

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

		DraggingAttachment = Instantiate(Attachment, CameraToWorld(out _), Quaternion.identity);
		MeshRenderer MR = DraggingAttachment.GetComponent<MeshRenderer>();
		Color RGB = MR.material.color;
		MR.material.color = new Color(RGB.r, RGB.g, RGB.b, .5f);
	}

	public void OnDrag(PointerEventData EventData)
	{
		// ...

		CameraToWorld(out RaycastHit Hit);
		if (TryGetSegment(ref Hit, out MSegment Segment)                // If the GameObject under the mouse has a Segment.
			&& Segment.TryGetWeaponSocket(out Transform Socket))    // If the Segment has a Weapon Socket.
		{
			DraggingAttachment.transform.position = Socket.position;
		}
		else
		{
			DraggingAttachment.transform.position = Hit.point;
		}
	}

	public void OnEndDrag(PointerEventData EventData)
	{
		// ...

		CameraToWorld(out RaycastHit Hit);
		if (TryGetSegment(ref Hit, out MSegment Segment))
		{
			if ((Weapon)Segment == null && !Segment.bIgnoreFromWeapons)
			{
				Segment.SetWeapon(Attachment);
				WeaponCardUI.Sub(Attachment);
			}
		}

		Destroy(DraggingAttachment.gameObject);
		DraggingAttachment = null;
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
}
