using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StructureData {

	[System.Serializable]
	public class VoxelPlacementData {

		public VoxelPositionVector3Int offset;
		public string voxelID;

	}

	[System.Serializable]
	public class VoxelPositionVector3Int {
		public int x, y, z;

		public VoxelPositionVector3Int() { x = y = z = 0; }

		public Vector3Int ToVector3Int() => new Vector3Int(x, y, z);
	}

	public VoxelPlacementData[] voxels;

}
