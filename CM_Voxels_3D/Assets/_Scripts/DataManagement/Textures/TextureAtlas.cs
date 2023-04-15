using CMVoxels.VoxelModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAtlas {

	private Texture2D atlasTexture;
	private Dictionary<string, Rect> textureRects;

	public TextureResourceManager TextureManager { get; private set; } = new TextureResourceManager();

	public bool IsBuilt => atlasTexture != null;

	private Dictionary<string, Texture2D> unbuiltTextures;

	public TextureAtlas() {
		unbuiltTextures = new Dictionary<string, Texture2D>();
	}

	public void Register(Texture2D newTexture, string textureID) {
		if (IsBuilt) {
			Debug.LogWarning($"[Texture Atlas Regestry] Warning! Attempting to register Texture ID \"{textureID}\" after atlas built!");
			return;
		}

		if (unbuiltTextures.ContainsKey(textureID)) {
			Debug.LogWarning($"[Texture Atlas Regestry] Warning! Texture ID \"{textureID}\" being replaced!");
			unbuiltTextures[textureID] = newTexture;
		} else {
			unbuiltTextures.Add(textureID, newTexture);
		}
	}

	public void Register(string resourceLocation) {
		if (IsBuilt) {
			Debug.LogWarning($"[Texture Atlas Regestry] Warning! Attempting to register Texture ID \"{resourceLocation}\" after atlas built!");
			return;
		}

		Texture2D loadedResource = TextureManager.LoadResource(resourceLocation);
		Register(loadedResource, resourceLocation);
	}

	public void Build() {
		textureRects = new Dictionary<string, Rect>();

		atlasTexture = new Texture2D(128, 128);

		string[] keys = new string[unbuiltTextures.Count];
		unbuiltTextures.Keys.CopyTo(keys, 0);

		Texture2D[] values = new Texture2D[unbuiltTextures.Count];
		unbuiltTextures.Values.CopyTo(values, 0);

		unbuiltTextures.Clear();

		Rect[] rects = atlasTexture.PackTextures(values, 1);

		for (int i = 0; i < keys.Length; i++) {
			textureRects.Add(keys[i], rects[i]);
		}
	}

	public Rect GetUVs(string textureID) {
		if (!IsBuilt) {
			Debug.LogWarning($"[Texture Atlas Get UVs] Warning! Attempting to get UVs before atlas is built!");
			return Rect.zero;
		}

		if (!textureRects.ContainsKey(textureID)) {
			Debug.LogWarning($"[Texture Atlas Get UVs] Warning! Attempting to get UVs from unregistered Texture ID \"{textureID}\"!");
			return Rect.zero;
		}

		return textureRects[textureID];
	}

	public Texture2D GetAtlasTexture() {
		if (!IsBuilt) {
			Debug.LogWarning($"[Texture Atlas Get Texture] Warning! Attempting to get texture before atlas is built!");
			return null;
		}

		return atlasTexture;
	}

}
