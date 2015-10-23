using UnityEngine;
using System.Collections;
using System;


namespace Kubility
{

    public class UGUIView : BaseView
    {
        [SerializeField]
        public Animator ani;
        protected int height;
        protected int width;

        public UGUIView(AbstractTrans trans)
            : base(trans)
        {
            height = 800;
            width = 600;
        }

        public override void Visit(object obj)
        {

        }

		public override void OnCreate ()
    	{
    		base.OnCreate ();
    	}

    	public override void OnPause ()
    	{
    		base.OnPause ();
    	}

    	public override void OnResume ()
    	{
    		base.OnResume ();
    	}

    	public override void OnDestroy ()
    	{
    		base.OnDestroy ();
    	}

    	public override void OnEnter ()
    	{
    		base.OnEnter ();
    	}

    	public override void OnExit ()
    	{
    		base.OnExit ();
    	}
    }
}



