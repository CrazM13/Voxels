using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager {
	

	private static Dictionary<string, Structure> cachedStructures = new Dictionary<string, Structure>();

	/// <summary>
	/// Loads Structure from file. Stores Structure by name for easy referencing
	/// </summary>
	/// <param name="name">The file to load from</param>
	/// <returns>Loaded biome, or NULL if input is NULL</returns>
	public Structure LoadResource(string name) {
		// If we already have the model loaded just return the model
		if (cachedStructures.ContainsKey(name)) return cachedStructures[name];

		// Get the models folder in Resources
		string path = @$"structures\{name}";

		// Load the model
		TextAsset jsonModel = Resources.Load<TextAsset>(path);

		if (jsonModel) {
			Structure loadedModel = Load(jsonModel);

			// Unload the resources file. Not sure if this is needed or if it unloads out of scope
			Resources.UnloadAsset(jsonModel);

			return loadedModel;
		} else Debug.LogWarning($"[Structure Load Resource] Structure {name} at {path} not found");

		return null;
	}

	/// <summary>
	/// Loads model from model file. Stores model by name for easy referencing
	/// </summary>
	/// <param name="structureFile">The file to load from</param>
	/// <param name="replace">Should replace a loaded model</param>
	/// <returns>Loaded model, or NULL if input is NULL</returns>
	private static Structure Load(TextAsset structureFile, bool replace = false) {
		if (!structureFile) return null;

		if (cachedStructures.ContainsKey(structureFile.name)) {
			if (replace) {
				Structure replacingStructure = new Structure(structureFile.text);

				cachedStructures[structureFile.name] = replacingStructure;

				return replacingStructure;
			} else {
				return cachedStructures[structureFile.name];
			}
		}

		Structure newStructure = new Structure(structureFile.text);

		cachedStructures.Add(structureFile.name, newStructure);

		return newStructure;
	}

	

}
