using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMVoxels;

public class Structure {

	private (Vector3Int offset, Voxel voxel)[] voxels;

	public Structure(string jsonStructure) {
		StructureData data = JsonUtility.FromJson<StructureData>(jsonStructure);

		voxels = new (Vector3Int offset, Voxel voxel)[data.voxels.Length];
		for (int i = 0; i < voxels.Length; i++) {
			voxels[i].offset = data.voxels[i].offset.ToVector3Int();
			voxels[i].voxel = DataManager.VoxelTypes.GetVoxelType(data.voxels[i].voxelID);
		}
	}

	public void PlaceStructure(World world, Vector3Int position) {
		foreach ((Vector3Int offset, Voxel voxel) in voxels) {
			world.SetVoxel(position + offset, voxel, false);
		}
	}
}
