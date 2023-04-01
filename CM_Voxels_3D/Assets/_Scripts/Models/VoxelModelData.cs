using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMVoxels.VoxelModels.Serialization {
	[System.Serializable]
	public class VoxelModelData {
		[System.Serializable]
		public class VoxelModelComponent {
			public bool canCull;
			public int bone;
			public VoxelModelSize3 size;
			public VoxelModelVector3 offset;
			public VoxelModelVector3 rotation;
			public VoxelModelTexturesComponent textures;

			public VoxelModelComponent() {
				canCull = false;
				bone = -1;
				size = new VoxelModelSize3();
				offset = new VoxelModelVector3();
				rotation = new VoxelModelVector3();
				textures = new VoxelModelTexturesComponent();
			}
		}

		[System.Serializable]
		public class VoxelModelBone {
			public VoxelModelSize3 size;
			public VoxelModelVector3 offset;
			public VoxelModelVector3 rotation;
			public int parent;

			public VoxelModelBone() {
				size = new VoxelModelSize3();
				offset = new VoxelModelVector3();
				rotation = new VoxelModelVector3();
				parent = -1;
			}
		}

		[System.Serializable]
		public class VoxelModelTexturesComponent {
			public VoxelModelTextureFace south, north, up, down, east, west;

			public VoxelModelTexturesComponent() {
				south = new VoxelModelTextureFace();
				north = new VoxelModelTextureFace();
				up = new VoxelModelTextureFace();
				down = new VoxelModelTextureFace();
				east = new VoxelModelTextureFace();
				west = new VoxelModelTextureFace();
			}
		}

		[System.Serializable]
		public class VoxelModelTextureFace {
			public int textureID;
			public VoxelModelOffset2 offset;
			public VoxelModelSize2 size;

			public VoxelModelTextureFace() {
				textureID = 0;
				offset = new VoxelModelOffset2();
				size = new VoxelModelSize2();
			}
		}

		[System.Serializable]
		public class VoxelModelVector3 {
			public float x, y, z;

			public VoxelModelVector3() { x = y = z = 0; }
		}

		[System.Serializable]
		public class VoxelModelOffset2 {
			public float x, y;

			public VoxelModelOffset2() { x = y = 0; }
		}

		[System.Serializable]
		public class VoxelModelSize2 {
			public float x, y;

			public VoxelModelSize2() { x = y = 1; }
		}

		[System.Serializable]
		public class VoxelModelSize3 {
			public float x, y, z;

			public VoxelModelSize3() { x = y = z = 1; }
		}

		public VoxelModelComponent[] voxels;
		public VoxelModelBone[] bones;

	}
}


