using UnityEngine;
using System.Collections;

namespace Kubility
{

	public class HideTrans :AbstractTrans
	{
		[SerializeField]
		protected GameObject _gobj;

		public GameObject gobj
		{
			get
			{
				return _gobj;
			}
			protected set
			{
				_gobj = value;
			}
		}

		public HideTrans(GameObject obj)
		{
			this.gobj = obj;
		}

		public override void OnCreateTrans ()
		{
			base.OnCreateTrans ();
			if(!gobj.activeSelf)
				gobj.SetActive(true);
		}

		public override void OnEnterTrans ()
		{
			base.OnEnterTrans ();
			if(!gobj.activeSelf)
				gobj.SetActive(true);
		}

		public override void OnExitTrans ()
		{
			base.OnExitTrans ();
			if(gobj.activeSelf)
				gobj.SetActive(false);
		}

		public override void OnDestroyTrans ()
		{
			base.OnDestroyTrans ();
			if(gobj.activeSelf)
				gobj.SetActive(false);
		}

		public override void OnPauseTrans ()
		{
			base.OnPauseTrans ();
			if(gobj.activeSelf)
				gobj.SetActive(false);
		}

		public override void OnResumeTrans ()
		{
			base.OnResumeTrans ();
			if(!gobj.activeSelf)
				gobj.SetActive(true);
		}

	}
}


