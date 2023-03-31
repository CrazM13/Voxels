using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxels {
	public class VoxelProducer : Voxel {

		private Voxel produce;
		private Vector3Int produceOffset;
		private float produceChance;

		public VoxelProducer(string voxelID, string modelID, Voxel produce, Vector3Int produceOffset, float produceChance = 1) : base(voxelID, modelID) {
			this.produce = produce;
			this.produceChance = produceChance;
			this.produceOffset = produceOffset;
		}

		//public override void OnRandomTick(World world, Vector3Int position) {
		//	if (produce == null) return;
		//
		//	if (Random.value < produceChance) {
		//		Vector3Int producePosition = position + produceOffset;
		//		if (world.GetVoxel(producePosition) == VoxelTypes.AIR.GetVoxelID()) {
		//			world.SetVoxel(producePosition, produce);
		//		}
		//	}
		//
		//}

		//public override void OnInteract(World world, Vector3Int position, VoxelEntity source) {
		//	Vector3Int producePosition = position + produceOffset;
		//	if (world.GetVoxel(producePosition) == VoxelTypes.AIR.GetVoxelID()) {
		//		world.SetVoxel(producePosition, produce);
		//	}
		//}

	}
}
