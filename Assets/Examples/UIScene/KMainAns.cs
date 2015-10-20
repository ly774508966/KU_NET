using UnityEngine;
using System.Collections;
using Kubility;

public class KMainAns :AnimationTrans
{
	public KMainAns(Animator an):base(an)
	{

	}

	public override void OnCreateTrans ()
	{
		base.OnCreateTrans ();

		ani.SetBool("return",false);
	}

	public override void OnEnterTrans ()
	{
		base.OnEnterTrans ();

	}

	public override void OnExitTrans ()
	{
		base.OnExitTrans ();
		ani.SetBool("EnterServer",false);
		ani.SetBool("return",true);
	}

	public override void OnDestroyTrans ()
	{
		base.OnDestroyTrans ();
		ani.SetBool("EnterServer",false);
		ani.SetBool("return",false);
	}

	public override void OnPauseTrans ()
	{
		base.OnPauseTrans ();
	}

	public override void OnResumeTrans ()
	{
		base.OnResumeTrans ();
	}


}
