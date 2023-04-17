using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMVoxels.VoxelModels {
	public class VoxelBoneTransform {

		public VoxelBoneTransform Parent { get; set; } = null;

		public Vector3 Origin { get; set; } = Vector3.zero;
		public Vector3 Position { get; set; } = Vector3.zero;
		public Vector3 Scale { get; set; } = Vector3.one;
		public Vector3 Rotation { get; set; } = Vector3.zero;

		public Vector3 TransformVertex(Vector3 vertex) {
			Vector3 newVertex = vertex;

			// Move to origin
			newVertex -= Origin;

			// Scale
			newVertex = new Vector3(Scale.x * newVertex.x, Scale.y * newVertex.y, Scale.z * newVertex.z);
			// Rotate
			newVertex = Quaternion.Euler(Rotation) * newVertex;
			// Offset
			newVertex += Position;

			// Apply parent
			if (Parent != null) newVertex = Parent.TransformVertex(newVertex);

			newVertex += Origin;

			return newVertex;
		}

		public VoxelBoneTransform GetTopParent() {
			VoxelBoneTransform topParent = this;
			while (topParent.Parent != null) {
				topParent = topParent.Parent;
			}
			return topParent;
		}

	}
}
