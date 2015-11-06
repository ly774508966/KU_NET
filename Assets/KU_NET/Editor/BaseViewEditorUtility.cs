using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Kubility.Editor
{
	public class BaseViewEditorUtility 
	{
		

	}


	public class BaseViewEditorItem
	{
		public BaseView view;
		public GameObject Place;
		public GameObject parent;
		public bool selected;
	}

	public class TransEditorItem
	{
		public BaseView view;
		public GameObject Place;
		public GameObject parent;
		public bool selected;
	}

	public class DelegateEditorItem
	{
		public BaseView view;
		public GameObject Place;
		public GameObject parent;
		public bool selected;
	}

}


