using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxels.Builders;

namespace Voxels {
	public class Voxels {

		public static readonly Voxel AIR = new VoxelBuilder("air", "NULL").SetTransparency(1, 1, 1).SetSeeThrough(true).BuildAndRegister();
		public static readonly Voxel GRASS = new VoxelBuilder("grass", "Grass").SetSolid(true).BuildAndRegister();
		public static readonly Voxel SNOW = new VoxelBuilder("snow", "Snow").SetSolid(true).BuildAndRegister();
		public static readonly Voxel SAND = new VoxelBuilder("sand", "Sand").SetSolid(true).BuildAndRegister();
		public static readonly Voxel DIRT = new VoxelBuilder("dirt", "Dirt").SetSolid(true).BuildAndRegister();
		public static readonly Voxel ROCK = new VoxelBuilder("rock", "Stone").SetSolid(true).BuildAndRegister();
		public static readonly Voxel LOG = new VoxelBuilder("log", "Log").SetSolid(true).BuildAndRegister();

		public static readonly Voxel CACTUS_FLOWER = new VoxelBuilder("cactus_flower", "Flower").SetSeeThrough(true).SetTransparency(1, 1, 1).BuildAndRegister();
		public static readonly Voxel CACTUS = new VoxelBuilder(new VoxelProducer("cactus", "Cactus", CACTUS_FLOWER, Vector3Int.up, 0.05f)).SetSolid(true).BuildAndRegister();

		public static readonly Voxel LEAF = new VoxelBuilder("leaf", "Leaf").SetSolid(true).BuildAndRegister();
		public static readonly Voxel APPLE = new VoxelBuilder("apple", "Apple").SetSolid(true).SetSeeThrough(true).SetTransparency(1, 1, 1).BuildAndRegister();
		public static readonly Voxel APPLE_LEAF = new VoxelBuilder(new VoxelProducer("apple_leaf", "Leaf", APPLE, Vector3Int.down, 0.05f)).SetSolid(true).BuildAndRegister();

		public static readonly Voxel LAMP = new VoxelBuilder("lamp", "Lamp").SetTransparency(1, 1, 1).SetSeeThrough(true).SetEmmision(1, 0.75f, 0).BuildAndRegister();

	}
}
