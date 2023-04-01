using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMVoxels {
	public class VoxelManager {

		private Dictionary<string, Voxel> voxels = new Dictionary<string, Voxel>();

		public Voxel GetVoxelType(string voxelID) {
			if (voxels.ContainsKey(voxelID)) {
				return voxels[voxelID];
			}

			Debug.LogWarning($"[Get Voxel Type] WARNING! Voxel with ID \"{voxelID}\" not found.");
			return null;
		}

		public Voxel RegisterVoxel(Voxel voxel) {
			string id = voxel.GetVoxelID();
			if (voxels.ContainsKey(id)) {
				Debug.LogWarning($"[Register Voxel] WARNING! Multiple voxels registered as \"{id}\". Overwriting existing entry");
				voxels[id] = voxel;
			} else {
				voxels.Add(id, voxel);
			}

			return voxel;
		}

	}
}
