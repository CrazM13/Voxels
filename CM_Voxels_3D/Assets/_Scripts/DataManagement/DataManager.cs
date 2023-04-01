using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMVoxels;
using CMVoxels.VoxelModels;

public static class DataManager {

	public static VoxelManager VoxelTypes { get; private set; } = new VoxelManager();
	public static VoxelModelManager Models { get; private set; } = new VoxelModelManager();
	public static BiomeManager Biomes { get; private set; } = new BiomeManager();
	public static StructureGeneratorManager StructureGenerators { get; private set; } = new StructureGeneratorManager();
	public static StructureManager Structures { get; private set; } = new StructureManager();

	public static void LoadData() {
		CMVoxels.Voxels.Register();
	}

}
