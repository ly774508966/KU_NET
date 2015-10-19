using UnityEngine;
using System.Collections;
using Kubility;

public class MainViewType:UIType
{
	public MainViewType (string name) : base (name)
	{
		
	}
	
	public MainViewType (string name, int value) : base (name, value)
	{
		
	}
	
	public static MainViewType U1View = new MainViewType ("U1View");
	public static MainViewType U2View = new MainViewType ("U2View");
	public static MainViewType U3View = new MainViewType ("U3View");
}
