using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureResourceManager : ResourceLoadingManager<Texture2D, Texture2D> {
	
	public TextureResourceManager() : base("textures") {}
	
	protected override Texture2D Load(Texture2D resourceFile) {
		return resourceFile;
	}
}
