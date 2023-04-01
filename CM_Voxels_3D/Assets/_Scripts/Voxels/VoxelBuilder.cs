using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMVoxels.Builders {
	public class VoxelBuilder {

		private Voxel voxel;

		public VoxelBuilder(string voxelID, string modelID) {
			voxel = new Voxel(voxelID, modelID);
		}

		public VoxelBuilder(Voxel voxel) {
			this.voxel = voxel;
		}

		public Voxel Build() => voxel;

		public Voxel BuildAndRegister() {

			DataManager.VoxelTypes.RegisterVoxel(voxel);

			return voxel;
		}

		public VoxelBuilder SetSolid(bool value) {
			voxel.SetSolid(value);
			return this;
		}

		public VoxelBuilder SetTransparency(float r, float g, float b) {
			voxel.SetTransparency(new VoxelLightColour(r, g, b));
			return this;
		}

		public VoxelBuilder SetEmmision(float r, float g, float b) {
			voxel.SetEmmision(new VoxelLightColour(r, g, b));
			return this;
		}

		public VoxelBuilder SetSeeThrough(bool value) {
			voxel.SetRenderNeighborFaces(value);
			return this;
		}

	}
}
