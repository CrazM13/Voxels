using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMVoxels.VoxelModels;
using CMVoxels.VoxelModels.Animation;

namespace CMVoxels.Rendering {
	public class VoxelModelRenderer {

		private MeshRenderer meshRenderer;
		private MeshFilter meshFilter;
		private Mesh mesh;
		public int VertexIndex { get; set; } = 0;
		public List<Vector3> Vertices { get; private set; } = new List<Vector3>();
		public List<int> Triangles { get; private set; } = new List<int>();
		public List<Vector2> UVs { get; private set; } = new List<Vector2>();
		public List<Color> Colours { get; private set; } = new List<Color>();

		public VoxelModelRenderer(GameObject voxelObject, Material voxelMaterial) {
			meshRenderer = voxelObject.AddComponent<MeshRenderer>();
			meshFilter = voxelObject.AddComponent<MeshFilter>();

			this.meshRenderer.material = voxelMaterial;
			mesh = new Mesh();
		}

		public void Render(VoxelModel model, VoxelBoneTransform transform, VoxelArmature armature = null, VoxelMesh.VoxelMeshCulling culling = null) {
			// No Model Loaded. Stop
			if (!model.IsLoaded) return;

			RenderVoxelMesh(model, transform, armature);
		}

		private void RenderVoxelMesh(VoxelModel model, VoxelBoneTransform transform, VoxelArmature armature = null, VoxelMesh.VoxelMeshCulling culling = null) {
			VoxelMesh voxelMesh = model.GetMesh();

			if (culling == null) voxelMesh.RebuildMesh();
			else {
				voxelMesh.RebuildMesh(culling);
			}

			for (int v = 0; v < voxelMesh.Vertices.Count; v++) {
				int boneIndex = voxelMesh.Bones[v];

				Vector3 newVertex;
				newVertex = voxelMesh.Vertices[v];

				if (armature != null) {
					VoxelBoneTransform topParent = transform.GetTopParent();
					topParent.Parent = model.GetArmature().GetBone(boneIndex);

					newVertex = transform.TransformVertex(newVertex);

					topParent.Parent = null;
				} else {
					newVertex = transform.TransformVertex(newVertex);
				}

				Vertices.Add(newVertex);

				Colours.Add(new Color(0, 0, 0, 1));
			}

			for (int t = 0; t < voxelMesh.Triangles.Count; t++) {
				Triangles.Add(voxelMesh.Triangles[t]);
			}

			for (int uv = 0; uv < voxelMesh.UVs.Count; uv++) {
				UVs.Add(voxelMesh.UVs[uv]);
			}
		}

		public void CreateMesh() {
			mesh.Clear();

			mesh.vertices = Vertices.ToArray();
			mesh.triangles = Triangles.ToArray();
			mesh.uv = UVs.ToArray();
			mesh.colors = Colours.ToArray();


			mesh.RecalculateNormals();

			meshFilter.mesh = mesh;
		}

		public void ClearMesh() {
			VertexIndex = 0;
			Vertices.Clear();
			Triangles.Clear();
			UVs.Clear();
			Colours.Clear();
		}
	}
}
