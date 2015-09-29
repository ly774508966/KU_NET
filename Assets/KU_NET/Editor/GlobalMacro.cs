using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;

namespace Kubility
{

		public class GlobalMacro :EditorWindow
		{
		
				[MenuItem ("Tools/globalMacros")]
				public static void Add ()
				{
						GlobalMacro gwindow = EditorWindow.GetWindow<GlobalMacro> ();
						gwindow.Init ();
						gwindow.name = "globalMacros";
						gwindow.Show ();
				}

				string Macros;
				string EditorMacros;
				string TagName;
				List<string> mList = new List<string> ();
				List<string> Elist = new List<string> ();

				string[] tag = { "Macros", "EditorMacros" };
				int selected = 0;

				Vector2 curpos = new Vector2 ();

				string rootPath = Path.GetDirectoryName (Application.dataPath);

				string csharp_file_1;
				string csharp_file_2;
		
				string editor_file_1;
				string editor_file_2;

				string smcs = "smcs.rsp";
				string gmcs = "gmcs.rsp";

				bool Write (string path, string newMacros)
				{
						if (File.Exists (path)) {
								FileStream fs = new FileStream (path, FileMode.Open, FileAccess.ReadWrite);
								StreamReader sr = new StreamReader (fs);
								string str = sr.ReadLine ();

								ByteBuffer buffer = new ByteBuffer (10240);
								bool bvalue = false;
								bool isChange = false;
								while (str != null) {
										bvalue = str.StartsWith ("	<DefineConstants>DEBUG;");
										if (bvalue) {
												str = str.Insert (str.IndexOf ("DEBUG") + 6, newMacros + ";");
												isChange = true;
										}

										buffer += System.Text.Encoding.UTF8.GetBytes (str + "\n");

										str = sr.ReadLine ();
								}

								fs.SetLength (0);

								var bs = buffer.ConverToBytes ();
								fs.Write (bs, 0, bs.Length);
								fs.Flush ();
								fs.Close ();
								sr.Close ();

								return isChange;
						}

						return false;
				}

				void Read (string path, List<string> list)
				{
						if (File.Exists (path)) {
								FileStream fs = new FileStream (path, FileMode.Open, FileAccess.ReadWrite);
								StreamReader sr = new StreamReader (fs);
								string str = sr.ReadLine ();

								bool bvalue = false;
								while (str != null) {
										bvalue = str.StartsWith ("	<DefineConstants>");
										if (bvalue) {
												int index = str.IndexOf ("DEBUG");
												var substr = str.Substring (index, str.Length - index - 18);
												var arr = substr.Split (';');

												foreach (var sub in arr) {
														list.Add (sub);
												}

												break;
										}

										str = sr.ReadLine ();
								}

								fs.Close ();
								sr.Close ();
						}
				}

				void Init ()
				{
						csharp_file_1 = rootPath + "/Assembly-CSharp.csproj";
						csharp_file_2 = rootPath + "/Assembly-CSharp-vs.csproj";
			
						editor_file_1 = rootPath + "/Assembly-CSharp-Editor.csproj";
						editor_file_2 = rootPath + "/Assembly-CSharp-Editor-vs.csproj";
						Read (csharp_file_1, mList);
						Read (csharp_file_2, mList);

						Read (editor_file_1, Elist);
						Read (editor_file_2, Elist);
				}


		
		
				void OnGUI ()
				{

						GUILayout.Space (10);
						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Input Global Macros ", GUILayout.Width (200));
						GUILayout.Space (10);
						Macros = EditorGUILayout.TextField (Macros, GUILayout.Width (100));

						GUILayout.EndHorizontal ();

						GUILayout.BeginHorizontal ();
						if (GUILayout.Button ("Add  ", GUILayout.Width (150))) {
								if (mList.Contains (Macros)) {
										LogMgr.LogError ("Contains");
								} else {
										if (Write (csharp_file_1, Macros) && Write (csharp_file_2, Macros)) {
												mList.Add (Macros);
										}

										UpdateRsp (smcs, Macros);
					
								}
						}


						GUILayout.EndHorizontal ();

						GUILayout.Space (10);
						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Input Global Editor Macros ", GUILayout.Width (200));
						GUILayout.Space (10);
						EditorMacros = EditorGUILayout.TextField (EditorMacros, GUILayout.Width (100));
						GUILayout.EndHorizontal ();

						GUILayout.BeginHorizontal ();
						if (GUILayout.Button ("Add  ", GUILayout.Width (150))) {
								if (Elist.Contains (EditorMacros)) {
										LogMgr.LogError ("Contains");
								} else {
										if (Write (editor_file_1, EditorMacros) && Write (editor_file_2, EditorMacros)) {
												Elist.Add (EditorMacros);
										}

										UpdateRsp (gmcs, EditorMacros);

								}
						}


						GUILayout.EndHorizontal ();

						GUILayout.Space (10);
						GUILayout.BeginHorizontal ();

						GUILayout.Label ("Tag  Name", GUILayout.Width (210));

						TagName = EditorGUILayout.TextField (TagName, GUILayout.Width (100));

						GUILayout.EndHorizontal ();

						if (GUILayout.Button ("Add Tag", GUILayout.Width (200))) {
								KbEditorUtils.AddTag (TagName);
						}
			
						GUILayout.Space (20);

						selected = GUILayout.Toolbar (selected, tag, GUILayout.Width (400));

						if (selected == 0) {
								KbEditorUtils.ListToolbar<string> (this.mList, ref curpos);
						} else
								KbEditorUtils.ListToolbar<string> (this.Elist, ref curpos);

				}

				void UpdateRsp (string path, string newMacros)
				{

						string rspPath = Application.dataPath + "/" + path;

						try {
								FileStream fs = new FileStream (rspPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

								fs.Seek (0, SeekOrigin.End);
								byte[] bs = System.Text.Encoding.UTF8.GetBytes (" -define:" + newMacros);
								fs.Write (bs, 0, bs.Length);
								fs.Flush ();
								fs.Close ();
						} catch (Exception ex) {

								LogMgr.LogError (ex);
						}


				}


		}



}


