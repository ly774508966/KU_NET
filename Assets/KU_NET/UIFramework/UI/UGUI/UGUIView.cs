using UnityEngine;
using System.Collections;
using System;


namespace Kubility
{

		public class UGUIView :BaseView
		{
				[SerializeField]
				public Animator ani;
				protected int height;

				protected int width;

				public AbstractTrans tr;
				public GameObject gggg;

				public UGUIView (AbstractTrans trans) : base (trans)
				{
						height = 800;
						width = 600;
				}

				public override void Visit (object obj)
				{

				}

				protected override void OnEnter ()
				{
						base.OnEnter ();

				}

				protected override void OnExit ()
				{
						base.OnExit ();

				}

				protected override void OnCreate ()
				{
						base.OnCreate ();

				}
		}
}



