using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelLightColour {

	public readonly static VoxelLightColour NONE = new VoxelLightColour(0, 0, 0);

	public float R { get; set; }
	public float G { get; set; }
	public float B { get; set; }
	public float SkyInfluence { get; set; }

	public VoxelLightColour(float r, float g, float b, float skyInfluence = 0) {
		R = r;
		G = g;
		B = b;
		SkyInfluence = skyInfluence;
	}

	public void ApplyDiffusion() {
		R = Mathf.Clamp01(R - VoxelLightingData.LightFalloff);
		G = Mathf.Clamp01(G - VoxelLightingData.LightFalloff);
		B = Mathf.Clamp01(B - VoxelLightingData.LightFalloff);

		SkyInfluence = Mathf.Clamp01(SkyInfluence - VoxelLightingData.LightFalloff);
	}

	public void Increase(float r, float g, float b) {
		R = Mathf.Clamp01(R + r);
		G = Mathf.Clamp01(G + g);
		B = Mathf.Clamp01(B + b);
	}

	public VoxelLightColour Copy() {
		return new VoxelLightColour(R, G, B, SkyInfluence);
	}

	public Color ToColour() {
		return new Color(R, G, B, SkyInfluence);
	}

	public bool IsActive => R != 0 || B != 0 || G != 0 || SkyInfluence != 0;

}
