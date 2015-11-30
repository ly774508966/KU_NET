using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;


public class DValue<T>  where T:struct, IConvertible

{
	public T old;
	public T cur;
	
	string Target;
	private Material _mat;
	public Material mat
	{
		get
		{
			return _mat;
		}
		set
		{
			if(_mat == null && value != null)
			{
				_mat = value;
			}
		}
	}

	
	public DValue(T t1,T t2,string target)
	{
		old = t1;
		cur = t2;		
		Target  = target;
	}
	
	public void Check()
	{
		if(!old.Equals(cur))
		{
			LogMgr.Log("change ="+ cur);
			if( mat != null)
			{
				if(cur.GetType() == typeof(float))
				{
					mat.SetFloat(Target,(float)System.Convert.ChangeType(cur,typeof(float))) ;
				}
				else if(cur.Equals(true) )
				{
					mat.EnableKeyword(Target);
//					mat.SetFloat(Target,1f);
				}
				else if (!cur.Equals(true) )
				{
					mat.DisableKeyword(Target);
				}
//					mat.SetFloat(Target,0f);;
			}
			old  = cur;
		}
	}
	
	public T GetDefault()
	{

		if(typeof(T) == typeof(float))
		{
			float val =0f;
			if(mat.HasProperty(Target))
			{
				val = mat.GetFloat(Target);
			}

			return (T)Convert.ChangeType(val ,typeof(T));
		}
		else if(typeof(T) == typeof(bool))
		{
			return (T)Convert.ChangeType(mat.IsKeywordEnabled(Target) ,typeof(T));
		}

		return default(T);

	}
}

public class SimpleShaderEditor :ShaderGUI  
{

	#region vf_shader_vars
	
	DValue<bool> Phone_Value = new DValue<bool>(false,false,"_PToggle");
	DValue<bool> Diffuse_Value = new DValue<bool>(false,false,"_DToggle");
	DValue<bool> Ambient_Value= new DValue<bool>(false,false,"_AToggle");
	DValue<float> phone_power = new DValue<float>(1f,1f,"phone_power");

	Color Dcol;
	Color Pcol;
	Color Acol;
	
	// string PhoneString ="_PToggle";
	// string DiffuseString ="_DToggle";
	// string AmbientString ="_AToggle";
	#endregion
	
	#region surface_vars
	
	#endregion
	
	Material _mat;
	Material mat 
	{
		get
		{
			return _mat;
		}
		set
		{
			if(_mat == null && value != null)
			{
				_mat = value;

				Phone_Value.mat = _mat;
				Phone_Value.cur = Phone_Value.GetDefault() ;
				Phone_Value.old = Phone_Value.cur;

				Diffuse_Value.mat = _mat;
				Diffuse_Value.cur = Diffuse_Value.GetDefault();
				Diffuse_Value.old = Diffuse_Value.cur;

				Ambient_Value.mat = _mat;
				Ambient_Value.cur = Ambient_Value.GetDefault() ;
				Ambient_Value.old =Ambient_Value.cur;

				phone_power.mat = _mat;
				phone_power.cur = phone_power.GetDefault();
				phone_power.old = phone_power.cur;
				
				if(_mat.HasProperty("Diffuse_Color"))
					Dcol = _mat.GetColor("Diffuse_Color");
					
				if(_mat.HasProperty("Ambient_Color"))
					Acol = _mat.GetColor("Ambient_Color");
					
				if(_mat.HasProperty("phone_Color"))	
					Pcol = _mat.GetColor("phone_Color");
        
     		 }
      
		}
	}
	
	string shaderName
	{
		get
		{
			return mat.shader.name;
		}
	}

	public SimpleShaderEditor()
	{
		Dcol = Color.white;
		Acol = Color.white;
		Pcol = Color.white;
	}

	public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties)
	{

		base.OnGUI (materialEditor, properties);
		mat = materialEditor.target as Material;

		if(mat  != null)
		{
			int index = shaderName.IndexOf("Vf");
			if(index != -1)
			{
				vf_shaderGUI_Phone();
				vf_shaderGUI_Diffuse();
				vf_shaderGUI_Ambient();
			}
			else//默认其为surface
			{
				
			}

		}

		MaterialEditor.ApplyMaterialPropertyDrawers(_mat);

		for(int i=0 ; i < properties.Length;++i)
		{

//			properties[i].
		}




	}
	
	#region SURFACE_SHADER
	
	
	#endregion
	
	#region VF_SHADER
	
	void vf_shaderGUI_Phone()
	{
		Phone_Value.cur = EditorGUILayout.Toggle("镜面高光 开关",Phone_Value.cur);
		
		if(Phone_Value.old )
		{
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("镜面高光系数");
			phone_power.cur = EditorGUILayout.Slider(phone_power.cur,0,100f);
			EditorGUILayout.EndVertical();
			phone_power.Check();
			
			Color old = Pcol;

			Pcol = EditorGUILayout.ColorField("镜面反射自定义着色",Pcol);
			if(old != Pcol)
			{
				mat.SetColor("phone_Color",Pcol);
			}
		}
		
		Phone_Value.Check();

	}
	
	void vf_shaderGUI_Diffuse()
	{
		Diffuse_Value.cur = EditorGUILayout.Toggle("漫反射 开关",Diffuse_Value.cur);
		Diffuse_Value.Check();

		if(Diffuse_Value.cur)
		{
			var old = Dcol;
			Dcol = EditorGUILayout.ColorField("漫反射自定义着色",Dcol);
			
			if(old != Dcol)
			{
				mat.SetColor("Diffuse_Color",Dcol);
			}
		}
			
	}
	
	void vf_shaderGUI_Ambient()
	{
		Ambient_Value.cur = EditorGUILayout.Toggle("环境光 开关",Ambient_Value.cur);
		
		Ambient_Value.Check();

		if(Ambient_Value.cur)
		{
			var old = Acol;
			Acol = EditorGUILayout.ColorField("环境光自定义着色",Acol);
			
			if(old != Acol)
			{
				mat.SetColor("Ambient_Color",Acol);
			}
		}

			
	}
	
	#endregion

	public override void OnMaterialPreviewGUI (MaterialEditor materialEditor, Rect r, GUIStyle background)
	{
		base.OnMaterialPreviewGUI (materialEditor, r, background);
	}

	public override void OnMaterialInteractivePreviewGUI (MaterialEditor materialEditor, Rect r, GUIStyle background)
	{
		base.OnMaterialInteractivePreviewGUI (materialEditor, r, background);
	}

	public override void OnMaterialPreviewSettingsGUI (MaterialEditor materialEditor)
	{
		base.OnMaterialPreviewSettingsGUI (materialEditor);
	}

	public override void AssignNewShaderToMaterial (Material material, Shader oldShader, Shader newShader)
	{
		base.AssignNewShaderToMaterial (material, oldShader, newShader);
	}

}
