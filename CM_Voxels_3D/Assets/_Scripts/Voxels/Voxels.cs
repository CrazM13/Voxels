using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMVoxels.Builders;

namespace CMVoxels {
	public class Voxels {

		public static readonly Voxel AIR = new VoxelBuilder("air", "NULL").SetTransparency(1, 1, 1).SetSeeThrough(true).Build();
		public static readonly Voxel GRASS = new VoxelBuilder("grass", "Grass").SetSolid(true).Build();
		public static readonly Voxel SNOW = new VoxelBuilder("snow", "Snow").SetSolid(true).Build();
		public static readonly Voxel SAND = new VoxelBuilder("sand", "Sand").SetSolid(true).Build();
		public static readonly Voxel DIRT = new VoxelBuilder("dirt", "Dirt").SetSolid(true).Build();
		public static readonly Voxel ROCK = new VoxelBuilder("rock", "Stone").SetSolid(true).Build();
		public static readonly Voxel LOG = new VoxelBuilder("log", "Log").SetSolid(true).Build();

		public static readonly Voxel CACTUS_FLOWER = new VoxelBuilder("cactus_flower", "Flower").SetSeeThrough(true).SetTransparency(1, 1, 1).Build();
		public static readonly Voxel CACTUS = new VoxelBuilder(new VoxelProducer("cactus", "Cactus", CACTUS_FLOWER, Vector3Int.up, 0.05f)).SetSolid(true).Build();

		public static readonly Voxel LEAF = new VoxelBuilder("leaf", "Leaf").SetSolid(true).SetSeeThrough(true).SetTransparency(1, 1, 1).Build();
		public static readonly Voxel APPLE = new VoxelBuilder("apple", "Apple").SetSolid(true).SetSeeThrough(true).SetTransparency(1, 1, 1).Build();
		public static readonly Voxel APPLE_LEAF = new VoxelBuilder(new VoxelProducer("apple_leaf", "Leaf", APPLE, Vector3Int.down, 0.05f)).SetSeeThrough(true).SetTransparency(1, 1, 1).SetSolid(true).Build();

		public static readonly Voxel LAMP = new VoxelBuilder("lamp", "Lamp").SetTransparency(1, 1, 1).SetSeeThrough(true).SetEmmision(1f, 0.75f, 0).Build();

		public static void Register() {
			DataManager.VoxelTypes.RegisterVoxel(AIR);
			DataManager.VoxelTypes.RegisterVoxel(GRASS);
			DataManager.VoxelTypes.RegisterVoxel(SNOW);
			DataManager.VoxelTypes.RegisterVoxel(SAND);
			DataManager.VoxelTypes.RegisterVoxel(DIRT);
			DataManager.VoxelTypes.RegisterVoxel(ROCK);
			DataManager.VoxelTypes.RegisterVoxel(LOG);

			DataManager.VoxelTypes.RegisterVoxel(CACTUS_FLOWER);
			DataManager.VoxelTypes.RegisterVoxel(CACTUS);

			DataManager.VoxelTypes.RegisterVoxel(LEAF);
			DataManager.VoxelTypes.RegisterVoxel(APPLE);
			DataManager.VoxelTypes.RegisterVoxel(APPLE_LEAF);

			DataManager.VoxelTypes.RegisterVoxel(LAMP);
		}

	}
}
