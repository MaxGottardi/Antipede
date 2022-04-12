using System;
using UnityEngine;

public partial class MCentipedeBody : MonoBehaviour
{
	/// <summary>Converts an unsigned integer to a signed integer.</summary>
	/// <param name="U">Unsigned integer to convert.</param>
	/// <returns>Signed integer.</returns>
	int U2I(uint U)
	{
		return Convert.ToInt32(U);
	}

	/// <summary>The last Segment of this centipede.</summary>
	/// <param name="Offset">The offset from the last position, default is zero.</param>
	/// <returns>The last Segment, or null if there are no segments (offset will be applied).</returns>
	MSegment GetLast(byte Offset = 0)
	{
		return this[U2I(SegmentsInfo.End - Offset)];
	}

	public MSegment this[int i] => Segments[i];
	public MSegment this[uint i] => Segments[U2I(i)];

}
