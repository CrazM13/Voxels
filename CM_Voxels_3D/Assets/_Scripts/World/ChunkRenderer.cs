using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMVoxels;
using CMVoxels.Rendering;
using CMVoxels.VoxelModels;

public class ChunkRenderer : MonoBehaviour {

	private new VoxelModelRenderer renderer;

	private static readonly Vector3Int[] voxelNormals = new Vector3Int[6] {
		new Vector3Int(0, 0, -1),
		new Vector3Int(0, 0, 1),
		new Vector3Int(0, 1, 0),
		new Vector3Int(0, -1, 0),
		new Vector3Int(-1, 0, 0),
		new Vector3Int(1, 0, 0)
	};

	private Chunk chunkData;
	public Chunk ChunkData {
		get => chunkData;
		set {
			chunkData = value;
			if (renderer == null) this.renderer = new VoxelModelRenderer(gameObject, ChunkData.World.voxelMaterial);
		}
	}

	private void CreateMeshData() {
		for (int y = ChunkData.ChunkHeight - 1; y >= 0; y--) {
			for (int x = 0; x < ChunkData.ChunkWidth; x++) {
				for (int z = 0; z < ChunkData.ChunkWidth; z++) {

					AddVoxelData(new Vector3Int(x, y, z));

				}
			}
		}
	}

	public void ClearChunkModel() {
		renderer.ClearMesh();
		renderer.CreateMesh();
	}

	public void ReRenderChunk() {
		renderer.ClearMesh();
		CreateMeshData();
		renderer.CreateMesh();
	}

	private void Update() {
		if (ChunkData == null) return;

		if (ChunkData.IsDirty) {
			//ChunkData.CalculateLight();
			ReRenderChunk();
			ChunkData.IsDirty = false;
		}
	}

	private void AddVoxelData(Vector3Int position) {
		Voxel voxel = ChunkData.GetVoxelAt(position).GetVoxelType();
		RenderToChunk(voxel.Model, position);
	}

	public void RenderToChunk(VoxelModel model, Vector3Int position) {
		// No Model Loaded. Stop
		if (!model.IsLoaded) return;

		// Add each model's mesh to the chunk mesh
		RenderVoxelMeshToChunk(model, position);
	}

	private void RenderVoxelMeshToChunk(VoxelModel model, Vector3Int position) {
		VoxelMesh voxelMesh = model.GetMesh();
		// Check culling
		bool[] culledFaces = new bool[6];
		for (int face = 0; face < 6; face++) {
			
			VoxelState voxelState = ChunkData.World.GetVoxelAt(ChunkData.LocalToWorld(position + voxelNormals[face]));
			culledFaces[face] = !voxelState.GetVoxelType().ShouldRenderNeighborFaces();
		}
		voxelMesh.RebuildMesh(new VoxelMesh.VoxelMeshCulling(culledFaces[0], culledFaces[1], culledFaces[2], culledFaces[3], culledFaces[4], culledFaces[5]));

		VoxelBoneTransform transform = new VoxelBoneTransform() {
			Position = position,
			Scale = Vector3.one,
			Rotation = Vector3.zero
		};

		// Build the rendering mesh
		for (int v = 0; v < voxelMesh.Vertices.Count; v++) {

			Vector3 newVertex = voxelMesh.Vertices[v];
			newVertex = transform.TransformVertex(newVertex);

			// Lighting
			VoxelState neighbor;
			Vector3Int neighborPosition = position + voxelMesh.FaceNormals[v];
			if (ChunkData.IsVoxelInChunk(neighborPosition)) neighbor = ChunkData.GetVoxelAt(neighborPosition);
			else {
				Vector3Int neighborWorldPosition = ChunkData.LocalToWorld(neighborPosition);
				neighbor = ChunkData.World.GetVoxelAt(neighborWorldPosition);
			}

			renderer.Colours.Add(neighbor.voxelLighting.ToColour());

			renderer.Vertices.Add(newVertex);
		}

		for (int t = 0; t < voxelMesh.Triangles.Count; t++) {
			renderer.Triangles.Add(renderer.VertexIndex + voxelMesh.Triangles[t]);
		}

		for (int uv = 0; uv < voxelMesh.UVs.Count; uv++) {
			renderer.UVs.Add(voxelMesh.UVs[uv]);
		}

		renderer.VertexIndex += voxelMesh.Vertices.Count;
	}

}
