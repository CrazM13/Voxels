using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelLightingData {

	public static readonly float MinLightLevel = 0.15f;
	public static readonly float MaxLightLevel = 0.8f;
	public static readonly float LightFalloff = 0.1f;
	public static readonly int LightPasses = 8;

	public static void InitShaderLighting() {
		Shader.SetGlobalFloat("MinLightLevel", MinLightLevel);
		Shader.SetGlobalFloat("MaxLightLevel", MaxLightLevel);
	}
}
