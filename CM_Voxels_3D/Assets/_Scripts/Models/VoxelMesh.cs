using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMVoxels.VoxelModels.Serialization;

namespace CMVoxels.VoxelModels {
	public class VoxelMesh {

		#region Read Only
		public static readonly Vector3Int[] voxelNormals = new Vector3Int[6] {
			new Vector3Int(0, 0, -1),
			new Vector3Int(0, 0, 1),
			new Vector3Int(0, 1, 0),
			new Vector3Int(0, -1, 0),
			new Vector3Int(-1, 0, 0),
			new Vector3Int(1, 0, 0)
		};

		public static readonly Vector3[] voxelVertices = new Vector3[8] {
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(0.5f, -0.5f, -0.5f),
			new Vector3(0.5f, 0.5f, -0.5f),
			new Vector3(-0.5f, 0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, 0.5f),
			new Vector3(0.5f, -0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, 0.5f, 0.5f)
		};

		public static readonly int[,] voxelTris = new int[6, 4] {
			// 0 1 2 2 1 3
			{ 0, 3, 1, 2}, // Back Face
			{ 5, 6, 4, 7}, // Front Face
			{ 3, 7, 2, 6}, // Top Face
			{ 1, 5, 0, 4}, // Bottom Face
			{ 4, 7, 0, 3}, // Left Face
			{ 1, 2, 5, 6} // Right Face
		};
		#endregion

		#region Subclass
		private class VoxelMeshFace {
			public byte faceIndex;
			public int bone;
			public bool canCull;

			public Vector3[] vertices = new Vector3[4];
			public int[] triangles = new int[6];
			public Vector2[] uvs = new Vector2[4];
		}

		public class VoxelMeshCulling {
			private bool[] culledFaces;

			public VoxelMeshCulling(bool back, bool front, bool top, bool bottom, bool left, bool right) {
				culledFaces = new bool[6];
				culledFaces[0] = back;
				culledFaces[1] = front;
				culledFaces[2] = top;
				culledFaces[3] = bottom;
				culledFaces[4] = left;
				culledFaces[5] = right;
			}

			public bool IsCulling(byte faceIndex) {
				return culledFaces[faceIndex];
			}

			public void Invert() {
				for (byte face = 0; face < 6; face++) {
					culledFaces[face] = !culledFaces[face];
				}
			}

			public override bool Equals(object obj) {
				VoxelMeshCulling cullingObj = (VoxelMeshCulling) obj;

				for (byte face = 0; face < 6; face++) {
					if (IsCulling(face) != cullingObj.IsCulling(face)) return false;
				}
				return true;
			}

			public override int GetHashCode() {
				return base.GetHashCode();
			}

			public override string ToString() {
				string output = "[";
				for (int i = 0; i < culledFaces.Length; i++) {
					output += culledFaces[i];
					if (i + 1 < culledFaces.Length) output += ",";
				}
				output += "]";

				return output;
			}

			public static bool operator ==(VoxelMeshCulling left, VoxelMeshCulling right) {
				if (left is null) {
					return right is null;
				} else if (right is null) return false;

				return left.Equals(right);
			}

			public static bool operator !=(VoxelMeshCulling left, VoxelMeshCulling right) {
				if (left is null) {
					return !(right is null);
				} else if (right is null) return true;

				return !left.Equals(right);
			}
		}
		#endregion

		private List<VoxelMeshFace> voxelMeshFaces = new List<VoxelMeshFace>();

		private int vertexIndex = 0;
		public List<Vector3> Vertices { get; private set; } = new List<Vector3>();
		public List<int> Triangles { get; private set; } = new List<int>();
		public List<Vector2> UVs { get; private set; } = new List<Vector2>();
		public List<Vector3Int> FaceNormals { get; private set; } = new List<Vector3Int>();
		public List<int> Bones { get; private set; } = new List<int>();

		private VoxelMeshCulling cachedCulling;

		public int FaceCount => voxelMeshFaces.Count;


		public VoxelMesh(VoxelModelData model) {
			// No Model Loaded. Stop
			if (model == null) return;

			// Add each Voxel to the renderer's cached model. This allows each model to be made of multiple voxels
			for (int i = 0; i < model.voxels.Length; i++) {
				LoadVoxel(model, i);
			}

			// Force build the mesh
			RebuildMesh(true);
		}

		private void LoadVoxel(VoxelModelData model, int voxelIndex) {
			VoxelModelData.VoxelModelComponent voxel = model.voxels[voxelIndex];
			Vector3 offset = new Vector3(voxel.offset.x, voxel.offset.y, voxel.offset.z);
			Vector3 size = new Vector3(voxel.size.x, voxel.size.y, voxel.size.z);
			Vector3 rotation = new Vector3(voxel.rotation.x, voxel.rotation.y, voxel.rotation.z);

			VoxelBoneTransform voxelTransform = new VoxelBoneTransform() {
				Position = offset,
				Scale = size,
				Rotation = rotation
			};

			for (byte face = 0; face < 6; face++) {
				LoadVoxelFace(voxel, face, voxelTransform);
			}
		}

		protected void LoadVoxelFace(VoxelModelData.VoxelModelComponent voxel, byte faceIndex, VoxelBoneTransform transform) {
			VoxelMeshFace newFace = new VoxelMeshFace();
		
			// Add Verts
			// Each face only needs four verts so we can use fewer verts by doing it manually. Mosty an optimization
			// Order of operations -> SCALE, ROTATION, OFFSET
			newFace.vertices[0] = transform.TransformVertex(VoxelMesh.voxelVertices[VoxelMesh.voxelTris[faceIndex, 0]]);
			newFace.vertices[1] = transform.TransformVertex(VoxelMesh.voxelVertices[VoxelMesh.voxelTris[faceIndex, 1]]);
			newFace.vertices[2] = transform.TransformVertex(VoxelMesh.voxelVertices[VoxelMesh.voxelTris[faceIndex, 2]]);
			newFace.vertices[3] = transform.TransformVertex(VoxelMesh.voxelVertices[VoxelMesh.voxelTris[faceIndex, 3]]);
		
			newFace.bone = voxel.bone;
			newFace.faceIndex = faceIndex;
		
			// Add UVs
			// Hand off the calculation of UVs since this method is already doing a lot
			LoadTexture(GetTextureID(voxel, faceIndex), ref newFace);
		
			// Add Tris
			// We still need six indecies per face
			newFace.triangles[0] = 0;
			newFace.triangles[1] = 1;
			newFace.triangles[2] = 2;
			newFace.triangles[3] = 2;
			newFace.triangles[4] = 1;
			newFace.triangles[5] = 3;
		
			newFace.canCull = voxel.canCull;
		
			voxelMeshFaces.Add(newFace);
		}

		private void LoadTexture(VoxelModelData.VoxelModelTextureFace faceData, ref VoxelMeshFace face) {
			int textureID = faceData.textureID;
		
			float y = textureID / VoxelUV.TEXTURE_ATLAS_WIDTH;
			float x = textureID - (y * VoxelUV.TEXTURE_ATLAS_WIDTH);
			
			// Normalize to the texture size in our atlas
			y *= VoxelUV.NormalizedBlockTextureSize;
			x *= VoxelUV.NormalizedBlockTextureSize;
			
			x += VoxelUV.NormalizedBlockTextureSize * faceData.offset.x;
			y += VoxelUV.NormalizedBlockTextureSize * faceData.offset.y;
			
			float width = VoxelUV.NormalizedBlockTextureSize * faceData.size.x;
			float height = VoxelUV.NormalizedBlockTextureSize * faceData.size.y;
			
			face.uvs[0] = new Vector2(x, y);
			face.uvs[1] = new Vector2(x, y + height);
			face.uvs[2] = new Vector2(x + width, y);
			face.uvs[3] = new Vector2(x + width, y + height);
		}

		private VoxelModelData.VoxelModelTextureFace GetTextureID(VoxelModelData.VoxelModelComponent voxel, int faceIndex) {
			return faceIndex switch {
				0 => voxel.textures.south,
				1 => voxel.textures.north,
				2 => voxel.textures.up,
				3 => voxel.textures.down,
				4 => voxel.textures.east,
				5 => voxel.textures.west,
				_ => null
			};
		}

		public void ClearMesh() {
			vertexIndex = 0;

			Vertices.Clear();
			Triangles.Clear();
			UVs.Clear();
			Bones.Clear();
			FaceNormals.Clear();
		}

		public void RebuildMesh(bool force = false) {

			if (!force && null == cachedCulling) return;

			ClearMesh();

			for (int i = 0; i < voxelMeshFaces.Count; i++) {
				BuildFace(i);
			}

			cachedCulling = null;
		}

		public void RebuildMesh(VoxelMeshCulling voxelMeshCulling, bool force = false) {

			if (!force && voxelMeshCulling == cachedCulling) return;

			ClearMesh();

			vertexIndex = 0;

			for (int i = 0; i < voxelMeshFaces.Count; i++) {
				if (!voxelMeshFaces[i].canCull || !voxelMeshCulling.IsCulling(voxelMeshFaces[i].faceIndex)) BuildFace(i);
			}

			cachedCulling = voxelMeshCulling;
		}

		private void BuildFace(int meshFaceIndex) {
			Vertices.Add(voxelMeshFaces[meshFaceIndex].vertices[0]);
			Vertices.Add(voxelMeshFaces[meshFaceIndex].vertices[1]);
			Vertices.Add(voxelMeshFaces[meshFaceIndex].vertices[2]);
			Vertices.Add(voxelMeshFaces[meshFaceIndex].vertices[3]);

			UVs.Add(voxelMeshFaces[meshFaceIndex].uvs[0]);
			UVs.Add(voxelMeshFaces[meshFaceIndex].uvs[1]);
			UVs.Add(voxelMeshFaces[meshFaceIndex].uvs[2]);
			UVs.Add(voxelMeshFaces[meshFaceIndex].uvs[3]);

			Triangles.Add(vertexIndex + voxelMeshFaces[meshFaceIndex].triangles[0]);
			Triangles.Add(vertexIndex + voxelMeshFaces[meshFaceIndex].triangles[1]);
			Triangles.Add(vertexIndex + voxelMeshFaces[meshFaceIndex].triangles[2]);
			Triangles.Add(vertexIndex + voxelMeshFaces[meshFaceIndex].triangles[3]);
			Triangles.Add(vertexIndex + voxelMeshFaces[meshFaceIndex].triangles[4]);
			Triangles.Add(vertexIndex + voxelMeshFaces[meshFaceIndex].triangles[5]);

			Bones.Add(voxelMeshFaces[meshFaceIndex].bone);
			Bones.Add(voxelMeshFaces[meshFaceIndex].bone);
			Bones.Add(voxelMeshFaces[meshFaceIndex].bone);
			Bones.Add(voxelMeshFaces[meshFaceIndex].bone);

			FaceNormals.Add(voxelNormals[voxelMeshFaces[meshFaceIndex].faceIndex]);
			FaceNormals.Add(voxelNormals[voxelMeshFaces[meshFaceIndex].faceIndex]);
			FaceNormals.Add(voxelNormals[voxelMeshFaces[meshFaceIndex].faceIndex]);
			FaceNormals.Add(voxelNormals[voxelMeshFaces[meshFaceIndex].faceIndex]);

			vertexIndex += 4;
		}

	}
}
