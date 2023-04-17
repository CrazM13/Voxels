using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMVoxels.VoxelModels.Serialization;

namespace CMVoxels.VoxelModels.Animation {
	public class VoxelArmature {

		private VoxelBoneTransform root;

		private VoxelBoneTransform[] bones;
		private int[] parents;

		public VoxelArmature() {
			root = new VoxelBoneTransform();
		}

		public VoxelArmature(VoxelModelData voxelModelData) {
			root = new VoxelBoneTransform();

			if (voxelModelData.bones != null) {
				bones = new VoxelBoneTransform[voxelModelData.bones.Length];
				parents = new int[voxelModelData.bones.Length];

				for (int i = 0; i < voxelModelData.bones.Length; i++) {
					VoxelModelData.VoxelModelBone bone = voxelModelData.bones[i];
					bones[i] = new VoxelBoneTransform();

					bones[i].Origin = new Vector3(bone.origin.x, bone.origin.y, bone.origin.z);
					bones[i].Position = new Vector3(bone.offset.x, bone.offset.y, bone.offset.z);
					bones[i].Rotation = new Vector3(bone.rotation.x, bone.rotation.y, bone.rotation.z);
					bones[i].Scale = new Vector3(bone.size.x, bone.size.y, bone.size.z);

					parents[i] = bone.parent;

				}

				ApplyParents();
			}

		}

		private void ApplyParents() {
			for (int i = 0; i < bones.Length; i++) {
				if (parents[i] >= 0) bones[i].Parent = bones[parents[i]];
				else bones[i].Parent = root;
			}
		}

		public VoxelArmature(VoxelArmature copy) {
			root = new VoxelBoneTransform();
			root.Position = copy.root.Position;
			root.Rotation = copy.root.Rotation;
			root.Scale = copy.root.Scale;

			bones = new VoxelBoneTransform[copy.bones.Length];
			parents = new int[copy.parents.Length];

			for (int i = 0; i < bones.Length; i++) {
				bones[i] = new VoxelBoneTransform();

				bones[i].Position = copy.bones[i].Position;
				bones[i].Rotation = copy.bones[i].Rotation;
				bones[i].Scale = copy.bones[i].Scale;

				parents[i] = copy.parents[i];
			}

			ApplyParents();
		}

		public VoxelBoneTransform GetBone(int bone) {
			if (bones == null || bone < 0 || bone >= bones.Length) return root;

			return bones[bone];
		}

	}
}
