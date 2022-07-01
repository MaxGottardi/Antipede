using UnityEngine;

public class EnforceFaceCamera : MonoBehaviour
{
	[SerializeField] EFaceMethod FaceMethod;

	static Camera CachedMainCamera;

	void Start()
	{
		if (FaceMethod == EFaceMethod.OnStart)
			FaceMainCamera();
	}

	void Update()
	{
		if (FaceMethod == EFaceMethod.OnUpdate)
			FaceMainCamera();
	}

	void LateUpdate()
	{
		if (FaceMethod == EFaceMethod.OnLateUpdate)
			FaceMainCamera();
	}

	void FaceMainCamera()
	{
		if (!CachedMainCamera)
			CachedMainCamera = MInput.MainCamera;
		transform.LookAt(CachedMainCamera.transform);
	}
}

public enum EFaceMethod : byte
{
	OnStart, OnUpdate, OnLateUpdate
}