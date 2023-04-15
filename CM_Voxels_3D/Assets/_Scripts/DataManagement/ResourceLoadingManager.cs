using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceLoadingManager<ResourceType, FileType> where FileType : UnityEngine.Object {

	private string path;
	private Dictionary<string, ResourceType> cachedResources = new Dictionary<string, ResourceType>();

	public ResourceLoadingManager(string path) {
		this.path = path;
	}

	/// <summary>
	/// Loads resource from file. Stores resource by name for easy referencing
	/// </summary>
	/// <param name="name">The file to load from</param>
	/// <returns>Loaded resource</returns>
	public ResourceType LoadResource(string name) {
		// If we already have the model loaded just return the model
		if (cachedResources.ContainsKey(name)) return cachedResources[name];

		// Get the models folder in Resources
		string path = @$"{this.path}\{name}";

		// Load the model
		FileType fileData = Resources.Load<FileType>(path);
		if (!fileData) Debug.LogWarning($"[Load Resource] File {name} at {path} not found");
		ResourceType loadedModel = LoadIntoCache(fileData.name, fileData);

		// Unload the resources file. Not sure if this is needed or if it unloads out of scope
		Resources.UnloadAsset(fileData);

		return loadedModel;
	}

	/// <summary>
	/// Loads resource from file
	/// </summary>
	/// <param name="resourceFile">The file to load from</param>
	/// <returns>Loaded resource</returns>
	protected abstract ResourceType Load(FileType resourceFile);

	private ResourceType LoadIntoCache(string resourceID, FileType file) {
		if (cachedResources.ContainsKey(resourceID)) {
			return cachedResources[resourceID];
		}

		ResourceType newResource = Load(file);

		cachedResources.Add(resourceID, newResource);

		return newResource;
	}

}
