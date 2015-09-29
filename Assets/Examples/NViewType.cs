using UnityEngine;
using System.Collections;
using Kubility;


public class NViewType:UIType
{
	public NViewType (string name) : base (name)
	{
		
	}
	
	public NViewType (string name, int value) : base (name, value)
	{
		
	}
	
	public static NViewType N1View = new NViewType ("N1View");
	public static NViewType N2View = new NViewType ("N2View");
	public static NViewType N3View = new NViewType ("N3View");
}