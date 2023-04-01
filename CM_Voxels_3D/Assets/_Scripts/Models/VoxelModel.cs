using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMVoxels.VoxelModels.Animation;

namespace CMVoxels.VoxelModels {
	public class VoxelModel {

		private VoxelMesh mesh;
		private VoxelArmature armature;

		public bool IsLoaded {get; private set;}

		public VoxelModel(string jsonModel) {
			if (!string.IsNullOrEmpty(jsonModel)) {
				Serialization.VoxelModelData modelData = JsonUtility.FromJson<Serialization.VoxelModelData>(jsonModel);
				mesh = new VoxelMesh(modelData);
				armature = new VoxelArmature(modelData);

				IsLoaded = true;
			}
		}

		public VoxelModel(Serialization.VoxelModelData modelData) {
			mesh = new VoxelMesh(modelData);
			armature = new VoxelArmature(modelData);

			IsLoaded = true;
		}

		public VoxelArmature GetArmature() {
			return new VoxelArmature(armature);
		}

		public VoxelMesh GetMesh() {
			return mesh;
		}

	}
}