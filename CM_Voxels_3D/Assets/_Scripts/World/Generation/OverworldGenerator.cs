using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldGenerator : WorldGenerator {

	private const float RAINFALL_STEP = 0.01f;
	private const float TEMPERATURE_STEP = 0.0025f;

	public OverworldGenerator(World world, WorldGeneratorSettings settings) : base(world, settings) { /*MT*/ }

	private float GetRainfall(Vector2 position) {
		return Mathf.PerlinNoise(position.x * RAINFALL_STEP, position.y * RAINFALL_STEP);
	}

	private float GetTemperature(Vector2 position) {
		return Mathf.PerlinNoise(position.x * TEMPERATURE_STEP, position.y * TEMPERATURE_STEP);
	}

	private Biome GetBiome(Vector2 position) {

		float rainfall = GetRainfall(position);
		float temperature = GetTemperature(position);

		int biomeIndex = -1;
		float difference = float.MaxValue;

		for (int i = 0; i < biomes.Count; i++) {
			float newDifference = biomes[i].GetAccuracy(rainfall, temperature);
			if (newDifference < difference) {
				biomeIndex = i;
				difference = newDifference;
			}
		}

		if (biomeIndex == -1) return Biome.DEFAULT;
		return biomes[biomeIndex];
	}

	public override Biome GetBiome(Vector3 position) {
		return GetBiome(new Vector2(position.x, position.z));
	}

	public float GetRainfall(Vector3 position) {
		return GetRainfall(new Vector2(position.x, position.z));
	}

	public float GetTemperature(Vector3 position) {
		return GetTemperature(new Vector2(position.x, position.z));
	}

	private int GetTerrainHeight(Vector3 position, Biome biome) {

		float hStep = biome.GetHorizontalStep();
		float vStep = biome.GetVerticalStep();
		float height = biome.GetBaseTerrainHeight();

		return Mathf.FloorToInt((Mathf.PerlinNoise((position.x / WorldSizeInVoxels) * hStep, (position.z / WorldSizeInVoxels) * hStep) * (ChunkHeight * vStep)) + (ChunkHeight * height));
	}

	private int GetHeightByBiome(Vector3 position) {

		Vector2 biomePosition = new Vector2(position.x, position.z);

		float rainfall = GetRainfall(biomePosition);
		float temperature = GetTemperature(biomePosition);

		float weightedHeight = 0;
		float totalWeight = 0;

		for (int i = 0; i < biomes.Count; i++) {
			float weight = 1f / (biomes[i].GetAccuracy(rainfall, temperature));

			weightedHeight += GetTerrainHeight(position, biomes[i]) * weight;
			totalWeight += weight;
		}

		return Mathf.RoundToInt(weightedHeight / totalWeight);
	}

	public override void PopulateChunkTerrain(Vector2Int chunk) {

		int worldX = chunk.x * ChunkWidth;
		int worldZ = chunk.y * ChunkWidth;

		for (int x = 0; x < ChunkWidth; x++) {
			for (int z = 0; z < ChunkWidth; z++) {

				Biome biome = GetBiome(new Vector2(worldX + x, worldZ + z));
				int terrainHeight = GetHeightByBiome(new Vector3(worldX + x, 0, worldZ + z));

				for (int y = 0; y <= terrainHeight; y++) {

					Vector3Int position = new Vector3Int(worldX + x, y, worldZ + z);
					if (position.y < terrainHeight) world.SetVoxel(position, biome.PopulateVoxel(terrainHeight, position), false);
				}
			}
		}
	}

	public override void PopulateChunkStructures(Vector2Int chunk) {
		int worldX = chunk.x * ChunkWidth;
		int worldZ = chunk.y * ChunkWidth;

		for (int x = 0; x < ChunkWidth; x++) {
			for (int z = 0; z < ChunkWidth; z++) {

				int terrainHeight = GetHeightByBiome(new Vector3(worldX + x, 0, worldZ + z));

				Vector3Int position = new Vector3Int(worldX + x, terrainHeight, worldZ + z);

				for (int i = 0; i < structureGenerators.Count; i++) {
					StructureGenerator generator = structureGenerators[i];

					if (generator.CanPlaceAt(world, position)) {
						Structure structure = generator.GetRandomStructure();
						structure.PlaceStructure(world, Vector3Int.FloorToInt(position));
					}
				}
			}
		}
	}

}
