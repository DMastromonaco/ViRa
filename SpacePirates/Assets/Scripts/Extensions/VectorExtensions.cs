using UnityEngine;
using System.Collections;

public static class VectorExtensions 
{
	public static Vector3 FromStrings(this Vector3 v3, string x, string y, string z)
	{
		return new Vector3( float.Parse((string)x),
		                    float.Parse((string)y),
		                    float.Parse((string)z));
	}

	public static string ToCSString(this Vector3 v3)
	{
		return v3.x.ToString() + "," + v3.y.ToString() + "," + v3.z.ToString();
	}
}
