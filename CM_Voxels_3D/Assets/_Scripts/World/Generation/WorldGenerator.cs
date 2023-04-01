using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldGenerator {

	protected World world;
	protected WorldGeneratorSettings settings;

	protected List<Biome> biomes;
	protected List<StructureGenerator> structureGenerators;

	public int WorldSizeInVoxels => settings.worldSizeInChunks * settings.chunkWidth;
	public int WorldSizeInChunks => settings.worldSizeInChunks;
	public int ChunkWidth => settings.chunkWidth;
	public int ChunkHeight => settings.chunkHeight;
	public int ViewDistanceInChunks => settings.viewDistanceInChunks;

	public WorldGenerator(World world, WorldGeneratorSettings settings) {
		this.world = world;
	
		if (settings == null) this.settings = new WorldGeneratorSettings();
		else this.settings = settings;
	
		this.biomes = new List<Biome>();
		for (int i = 0; i < this.settings.biomes.Length; i++) {
			Biome newBiome = DataManager.Biomes.LoadResource(this.settings.biomes[i]);
			if (newBiome != null) biomes.Add(newBiome);
		}
		if (biomes.Count == 0) biomes.Add(Biome.DEFAULT);
	
		this.structureGenerators = new List<StructureGenerator>();
		for (int i = 0; i < this.settings.structureGenerators.Length; i++) {
			StructureGenerator newStructureGenerator = DataManager.StructureGenerators.LoadResource(this.settings.structureGenerators[i]);
			if (newStructureGenerator != null) structureGenerators.Add(newStructureGenerator);
		}
	}

	public abstract Biome GetBiome(Vector3 position);

	public abstract void PopulateChunkTerrain(Vector2Int chunk);
	public abstract void PopulateChunkStructures(Vector2Int chunk);

	public WorldGeneratorSettings GetSettings() => settings;

}
