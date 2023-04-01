using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMVoxels;

public class Biome {

	public readonly static Biome DEFAULT = new Biome("DEFAULT", "{\"layers\": [{\"size\": 1, \"voxels\": [{\"voxelID\": \"grass\"}]}, {\"size\": 1, \"voxels\": [{\"voxelID\": \"dirt\"}]}, {\"voxels\": [{\"voxelID\": \"rock\"}]}], \"rainfall\": -100, \"temperature\": -100, \"terrainSettings\": {\"height\": 0.05, \"hillSize\": 0.95, \"smoothness\": 10}}");

	private BiomeData biomeData;

	public string BiomeName { get; private set; }

	public float Rainfall { get; private set; }
	public float Temperature { get; private set; }

	private float horizontalStep;
	private float verticalStep;
	private float terrainHeight;

	private (int size, Voxel[] voxels)[] layers;

	public Biome(string biomeName, string jsonBiome) {
		if (!string.IsNullOrEmpty(jsonBiome)) {
			biomeData = JsonUtility.FromJson<BiomeData>(jsonBiome);
			BiomeName = biomeName;

			Rainfall = biomeData.rainfall;
			Temperature = biomeData.temperature;

			horizontalStep = biomeData.terrainSettings.smoothness;
			verticalStep = biomeData.terrainSettings.hillSize;
			terrainHeight = biomeData.terrainSettings.height;

			layers = new (int size, Voxel[] voxel)[biomeData.layers.Length];
			for (int i = 0; i < layers.Length; i++) {
				layers[i].size = biomeData.layers[i].size;
				layers[i].voxels = new Voxel[biomeData.layers[i].voxels.Length];
				for (int j = 0; j < layers[i].voxels.Length; j++) {
					layers[i].voxels[j] = DataManager.VoxelTypes.GetVoxelType(biomeData.layers[i].voxels[j].voxelID);
				}
			}

		}
	}

	public float GetAccuracy(float rainfall, float temperature) {
		return (Mathf.Abs((this.Temperature - temperature)) + Mathf.Abs((this.Rainfall - rainfall))) / 2f;
	}

	public float GetHorizontalStep() => horizontalStep;
	public float GetVerticalStep() => verticalStep;
	public float GetBaseTerrainHeight() => terrainHeight;

	public Voxel PopulateVoxel(int terrainHeight, Vector3Int position) {

		int layerDepth = terrainHeight - Mathf.RoundToInt(position.y);

		int targetLayer = GetLayer(layerDepth);

		int voxelIndex = Mathf.FloorToInt(Mathf.PerlinNoise(position.x, position.z) * layers[targetLayer].voxels.Length);

		return layers[targetLayer].voxels[voxelIndex];
	}

	private int GetLayer(int depth) {
		int virtualLayer = 0;
		for (int i = 0; i < layers.Length; i++) {
			if (virtualLayer + layers[i].size >= depth) return i;
			virtualLayer += layers[i].size;
		}

		return layers.Length - 1;
	}

}
