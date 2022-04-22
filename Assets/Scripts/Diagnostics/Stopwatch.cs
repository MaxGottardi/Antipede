

/// <summary>Track execution time of code.</summary>
public class Stopwatch
{
#if UNITY_EDITOR
	readonly System.Diagnostics.Stopwatch sw;
#endif

	/// <summary>Creates a new Stopwatch.</summary>
	/// <param name="bStartOnCreation">Immediately starting timing upon instantiating?</param>
	public Stopwatch(bool bStartOnCreation = true)
	{
#if UNITY_EDITOR
		sw = new System.Diagnostics.Stopwatch();

		if (bStartOnCreation)
		{
			sw.Start();
		}
#endif
	}

	/// <summary>Start Stopwatch.</summary>
	public void Start()
	{
#if UNITY_EDITOR
		sw.Start();
#endif
	}

	/// <summary>Stop Stopwatch and get the elapsed <see cref="Time"/>.</summary>
	/// <docs>Stop Stopwatch and get the elapsed Time.</docs>
	/// <ret>The elapsed time before stopping.</ret>
	/// <returns>The elapsed <see cref="Time"/> before stopping.</returns>
	public long Stop()
	{
#if UNITY_EDITOR
		sw.Stop();
		return Time();
#else
		return 0;
#endif
	}

	/// <summary>Restarts Stopwatch and get the <see cref="Time"/> before restarting.</summary>
	/// <docs>Restarts Stopwatch and get the Time before restarting.</docs>
	/// <returns>The elapsed time before restarting.</returns>
	public long Restart()
	{
#if UNITY_EDITOR
		long now = Time();
		sw.Restart();

		return now;
#else
		return 0;
#endif
	}

#if UNITY_EDITOR
	/// <summary>Get the current elapsed time.</summary>
	public long Time() => sw.ElapsedMilliseconds;

	/// <summary>Get the current elapsed <see cref="Time"/> in seconds.</summary>
	/// <docs>Get the current elapsed time in seconds.</docs>
	public long TimeInSeconds() => ToSeconds(Time());

	/// <summary>Converts milliseconds to seconds.</summary>
	/// <param name="Milliseconds">Milliseconds to convert.</param>
	public static long ToSeconds(long Milliseconds) => Milliseconds * (long).001;
#else
	public long Time() => 0;
	public static long TimeInSeconds() => 0;
	public static long ToSeconds(long Milliseconds) => 0;
#endif
}