using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelLightingData {

	public static readonly float MinLightLevel = 0.15f;
	public static readonly float MaxLightLevel = 0.8f;
	public static readonly float LightFalloff = 0.25f;
	public static readonly int LightPasses = 0;//6; // 0 Disables diffuse lighting

	public static void InitShaderLighting() {
		Shader.SetGlobalFloat("MinLightLevel", MinLightLevel);
		Shader.SetGlobalFloat("MaxLightLevel", MaxLightLevel);
	}
}
