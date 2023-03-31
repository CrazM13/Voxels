using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxels.VoxelModels {
	public class VoxelModelManager {

		private Dictionary<string, VoxelModel> cachedModels = new Dictionary<string, VoxelModel>();

		/// <summary>
		/// Loads model from model file. Stores model by name for easy referencing
		/// </summary>
		/// <param name="name">The file to load from</param>
		/// <returns>Loaded model, or NULL if input is NULL</returns>
		public VoxelModel LoadResource(string name) {
			// If we already have the model loaded just return the model
			if (cachedModels.ContainsKey(name)) return cachedModels[name];

			// Get the models folder in Resources
			string path = @$"\models\{name}.json";

			// Load the model
			TextAsset jsonModel = Resources.Load<TextAsset>(path);
			VoxelModel loadedModel = Load(jsonModel);

			// Unload the resources file. Not sure if this is needed or if it unloads out of scope
			Resources.UnloadAsset(jsonModel);

			return loadedModel;
		}

		/// <summary>
		/// Loads model from model file. Stores model by name for easy referencing
		/// </summary>
		/// <param name="modelFile">The file to load from</param>
		/// <param name="replace">Should replace a loaded model</param>
		/// <returns>Loaded model, or NULL if input is NULL</returns>
		private VoxelModel Load(TextAsset modelFile, bool replace = false) {
			if (!modelFile) return new VoxelModel(string.Empty);

			if (cachedModels.ContainsKey(modelFile.name)) {
				if (replace) {
					VoxelModel replacingModel = new VoxelModel(modelFile.text);

					cachedModels[modelFile.name] = replacingModel;

					return replacingModel;
				} else {
					return cachedModels[modelFile.name];
				}
			}

			VoxelModel newModel = new VoxelModel(modelFile.text);

			cachedModels.Add(modelFile.name, newModel);

			return newModel;
		}

		public VoxelModel GetModel(string modelID) {
			if (modelID == "NULL") return new VoxelModel(string.Empty);

			if (cachedModels.ContainsKey(modelID)) {
				return cachedModels[modelID];
			}

			Debug.LogWarning($"[Get Model] WARNING! Model with name \"{modelID}\" not found. Loading empty model.");
			return new VoxelModel(string.Empty);
		}

	}
}
