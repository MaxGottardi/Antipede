using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightIntensityController : MonoBehaviour
{
	[SerializeField, ReadOnly] Light Light;
	[SerializeField] AnimationCurve IntensityOverTime;
	[SerializeField] float Duration = .417f; // The duration for Explosion Effect to finish its animation.

	float t = 0;
	float InverseDuration;

	void Start()
	{
		InverseDuration = 1f / Duration;
	}

	void Update()
	{
		if (t <= 1f)
		{
			Light.intensity = IntensityOverTime.Evaluate(t);

			t += Time.deltaTime * InverseDuration;
		}
	}

	void OnValidate()
	{
		Light = GetComponent<Light>();
	}
}
