using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldGeneratorSettings {

	public int chunkWidth;
	public int chunkHeight;

	public int worldSizeInChunks;
	public int viewDistanceInChunks;

	public string[] biomes;
	public string[] structureGenerators;

	public WorldGeneratorSettings() {
		chunkWidth = 8;
		chunkHeight = 24;

		worldSizeInChunks = 100;

		viewDistanceInChunks = 5;

		biomes = new string[0];
		structureGenerators = new string[0];
	}

}
