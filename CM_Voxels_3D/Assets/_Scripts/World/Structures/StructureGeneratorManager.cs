using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureGeneratorManager {

	private Dictionary<string, StructureGenerator> cachedStructureGenerators = new Dictionary<string, StructureGenerator>();


	/// <summary>
	/// Loads Structure Generator from file. Stores Structure Generator by name for easy referencing
	/// </summary>
	/// <param name="name">The file to load from</param>
	/// <returns>Loaded biome, or NULL if input is NULL</returns>
	public StructureGenerator LoadResource(string name) {
		// If we already have the model loaded just return the model
		if (cachedStructureGenerators.ContainsKey(name)) return cachedStructureGenerators[name];

		// Get the models folder in Resources
		string path = @$"structure_generators\{name}";

		// Load the model
		TextAsset jsonModel = Resources.Load<TextAsset>(path);
		if (jsonModel) {
			StructureGenerator loadedModel = Load(jsonModel);

			// Unload the resources file. Not sure if this is needed or if it unloads out of scope
			Resources.UnloadAsset(jsonModel);

			return loadedModel;
		} else Debug.LogWarning($"[Structure Generator Load Resource] Structure Generator {name} at {path} not found");

		return null;
	}

	/// <summary>
	/// Loads model from model file. Stores model by name for easy referencing
	/// </summary>
	/// <param name="structureGeneratorFile">The file to load from</param>
	/// <param name="replace">Should replace a loaded model</param>
	/// <returns>Loaded model, or NULL if input is NULL</returns>
	private StructureGenerator Load(TextAsset structureGeneratorFile, bool replace = false) {
		if (!structureGeneratorFile) return null;

		if (cachedStructureGenerators.ContainsKey(structureGeneratorFile.name)) {
			if (replace) {
				StructureGenerator replacingStructuregenerator = new StructureGenerator(structureGeneratorFile.text);

				cachedStructureGenerators[structureGeneratorFile.name] = replacingStructuregenerator;

				return replacingStructuregenerator;
			} else {
				return cachedStructureGenerators[structureGeneratorFile.name];
			}
		}

		StructureGenerator newStructureGenerator = new StructureGenerator(structureGeneratorFile.text);

		cachedStructureGenerators.Add(structureGeneratorFile.name, newStructureGenerator);

		return newStructureGenerator;
	}

}
