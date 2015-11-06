using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Kubility.Editor
{
	public static class KGUILayout
	{
		static Vector2 startPos;
		static float LeftWid;
		static float LeftHeight;
		static int nextElementCount;

		public static void InitSize(Rect size)
		{
			startPos = new Vector2(size.x,size.y);
		}

		public static void BeginArea(Rect rect)
		{

		}

		public static void EndArea()
		{

		}

		public static void BeginVertical()
		{

		}
  	}
}


