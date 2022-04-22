using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCentipedeSegmentEvents : MonoBehaviour
{
	MCentipedeEvents Listener;

	/// <summary>Listen to OnTriggerEnter events on this Segment.</summary>
	/// <param name="Listener">The main Centipede body that executes Trigger events.</param>
	public void Initialise(MCentipedeEvents Listener)
	{
		this.Listener = Listener;
	}
}
