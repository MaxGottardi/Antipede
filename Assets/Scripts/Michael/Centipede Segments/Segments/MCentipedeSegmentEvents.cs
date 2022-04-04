using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCentipedeSegmentEvents : MonoBehaviour
{
	MCentipedeEvents Listener;

	public void Initialise(MCentipedeEvents Listener)
	{
		this.Listener = Listener;
	}

	void OnTriggerEnter(Collider other)
	{
		Listener.OnSegmentTriggerEnter?.Invoke(other);
	}

}
