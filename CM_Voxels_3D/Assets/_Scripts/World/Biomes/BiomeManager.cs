using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeManager {

	private Dictionary<string, Biome> cachedBiomes = new Dictionary<string, Biome>();

	/// <summary>
	/// Loads Biome from biome file. Stores biome by name for easy referencing
	/// </summary>
	/// <param name="name">The file to load from</param>
	/// <returns>Loaded biome, or NULL if input is NULL</returns>
	public Biome LoadResource(string name) {
		// If we already have the model loaded just return the model
		if (cachedBiomes.ContainsKey(name)) return cachedBiomes[name];

		// Get the models folder in Resources
		string path = @$"biomes\{name}";

		// Load the model
		TextAsset jsonModel = Resources.Load<TextAsset>(path);

		if (jsonModel) {
			Biome loadedModel = Load(jsonModel);

			// Unload the resources file. Not sure if this is needed or if it unloads out of scope
			Resources.UnloadAsset(jsonModel);

			return loadedModel;
		}
		return null;
	}

	/// <summary>
	/// Loads model from model file. Stores model by name for easy referencing
	/// </summary>
	/// <param name="biomeFile">The file to load from</param>
	/// <param name="replace">Should replace a loaded model</param>
	/// <returns>Loaded model, or NULL if input is NULL</returns>
	private Biome Load(TextAsset biomeFile, bool replace = false) {
		if (!biomeFile) return Biome.DEFAULT;

		if (cachedBiomes.ContainsKey(biomeFile.name)) {
			if (replace) {
				Biome replacingBiome = new Biome(biomeFile.name, biomeFile.text);

				cachedBiomes[biomeFile.name] = replacingBiome;

				return replacingBiome;
			} else {
				return cachedBiomes[biomeFile.name];
			}
		}

		Biome newBiome = new Biome(biomeFile.name, biomeFile.text);

		cachedBiomes.Add(biomeFile.name, newBiome);

		return newBiome;
	}

}
