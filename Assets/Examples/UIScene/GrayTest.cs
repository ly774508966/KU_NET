using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrayTest : MonoBehaviour {

	public Material Mat;


	void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		RenderTexture tempRtLowA = RenderTexture.GetTemporary(source.width/8 , source.height/8,0,RenderTextureFormat.Default);

		tempRtLowA.filterMode = FilterMode.Bilinear;

		Graphics.Blit(source, tempRtLowA, Mat, 1);

		Mat.SetTexture("_Gray", tempRtLowA);

		Graphics.Blit(source, dest, Mat,0);

		RenderTexture.ReleaseTemporary(tempRtLowA);
	}
}
