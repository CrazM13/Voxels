using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMVoxels {
	public class VoxelState {

		public static readonly VoxelState EMPTY = new VoxelState();

		private Voxel cachedVoxelType = Voxels.AIR;

		private string id;
		public string ID {
			get => id;
			set {
				if (id != value) {
					id = value;
					cachedVoxelType = DataManager.VoxelTypes.GetVoxelType(id);
				}
			}
		}

		public Voxel @VoxelType {
			get => cachedVoxelType;
			set {
				if (cachedVoxelType != value) {
					cachedVoxelType = value;
					id = cachedVoxelType.GetVoxelID();
				}
			}
		}

		public Color voxelLighting;

		public VoxelState() {
			this.id = Voxels.AIR.GetVoxelID();
			this.voxelLighting = Color.clear;
		}

		public VoxelState(string id) {
			this.id = id;
			this.voxelLighting = Color.clear;
		}

		public Voxel GetVoxelType() {
			return cachedVoxelType;
		}

	}
}