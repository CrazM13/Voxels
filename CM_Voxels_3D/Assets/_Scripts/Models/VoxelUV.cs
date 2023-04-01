using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMVoxels.VoxelModels {

	public class VoxelUV {

		public static readonly int TEXTURE_ATLAS_WIDTH = 16;
		public static readonly int PIXELS_PER_VOXEL = 8;

		public static float NormalizedBlockTextureSize => 1f / TEXTURE_ATLAS_WIDTH;

	}

}
