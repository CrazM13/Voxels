using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMVoxels;

public class Chunk {

	private GameObject chunkObject;

	private VoxelState[,,] voxelMap;

	private ChunkRenderer renderer;
	public World World { get; private set; }
	public bool IsInitialized { get; private set; }
	public bool IsRendererInitialized { get; private set; }
	public bool IsPopulated { get; set; }

	public Vector2Int ChunkPosition { get; private set; }

	private bool isActive;
	public bool IsActive {
		get => isActive;
		set {
			isActive = value;
			if (chunkObject) chunkObject.SetActive(value);
		}
	}

	public Vector3 Position => chunkObject.transform.position;

	public int ChunkWidth => voxelMap.GetLength(0);
	public int ChunkHeight => voxelMap.GetLength(1);

	public Chunk(World world, Vector2Int chunkPosition) {
		this.World = world;
		this.ChunkPosition = chunkPosition;

		WorldGenerator worldGenerator = world.GetWorldGenerator();

		voxelMap = new VoxelState[worldGenerator.ChunkWidth, worldGenerator.ChunkHeight, worldGenerator.ChunkWidth];
	}

	public void Initialize() {
		if (IsInitialized) return;
		IsInitialized = true;
		PopulateVoxelMap();
	}

	public void InitializeRenderer() {
		WorldGenerator worldGenerator = World.GetWorldGenerator();

		// Create the chunk object for rendering
		chunkObject = new GameObject($"Chunk ({this.ChunkPosition.x}, {this.ChunkPosition.y})");
		chunkObject.transform.SetParent(this.World.transform);
		chunkObject.transform.position = new Vector3(this.ChunkPosition.x * worldGenerator.ChunkWidth, 0, this.ChunkPosition.y * worldGenerator.ChunkWidth);
		chunkObject.SetActive(isActive);

		renderer = new ChunkRenderer(this, chunkObject);

		IsRendererInitialized = true;
	}

	private void PopulateVoxelMap() {
		for (int y = 0; y < ChunkHeight; y++) {
			for (int x = 0; x < ChunkWidth; x++) {
				for (int z = 0; z < ChunkWidth; z++) {
					voxelMap[x, y, z] = new VoxelState();
				}
			}
		}

	}

	public bool IsVoxelInChunk(Vector3Int position) {
		return IsVoxelInChunk(position.x, position.y, position.z);
	}

	public bool IsVoxelInChunk(int x, int y, int z) {
		if (x < 0 || x >= ChunkWidth) return false;
		if (y < 0 || y >= ChunkHeight) return false;
		if (z < 0 || z >= ChunkWidth) return false;

		return true;
	}

	public VoxelState GetVoxelAt(Vector3Int position) {
		if (position.x < 0 || position.x >= ChunkWidth) return VoxelState.EMPTY;
		if (position.y < 0 || position.y >= ChunkHeight) return VoxelState.EMPTY;
		if (position.z < 0 || position.z >= ChunkWidth) return VoxelState.EMPTY;
	
		return voxelMap[position.x, position.y, position.z];
	}

	public bool SetVoxelAt(Vector3Int position, Voxel voxel, bool forceRerender = true) {
		if (position.x < 0 || position.x >= ChunkWidth) return false;
		if (position.y < 0 || position.y >= ChunkHeight) return false;
		if (position.z < 0 || position.z >= ChunkWidth) return false;
	
		if (voxelMap[position.x, position.y, position.z].VoxelType == voxel) return false;
	
		voxelMap[position.x, position.y, position.z].VoxelType = voxel;
	
		if (forceRerender) ReloadRenderer();
	
		return true;
	}

	public void ReloadRenderer() {
		//CalculateLight();

		if (renderer != null) renderer.ReRenderChunk();
	}

	private void CalculateLight() {
		// Sky Pass
		for (int x = 0; x < ChunkWidth; x++) {
			for (int z = 0; z < ChunkWidth; z++) {
				float skyLight = 1f;

				for (int y = ChunkHeight - 1; y >= 0; y--) {
					VoxelState currentVoxel = voxelMap[x, y, z];
					skyLight *= currentVoxel.GetVoxelType().GetTransparency();
					
					currentVoxel.voxelLighting.a = skyLight;
				}

			}
		}

		// Diffuse Pass
		// Top level loop is number of passes. More passes = better lighting
		//for (int _ = 0; _ < VoxelLightingData.LightPasses; _++) {
		//	for (int x = 0; x < ChunkWidth; x++) {
		//		for (int z = 0; z < ChunkWidth; z++) {
		//
		//			for (int y = ChunkHeight - 1; y >= 0; y--) {
		//				VoxelState currentVoxel = voxelMap[x, y, z];
		//
		//				float transparency = currentVoxel.GetVoxelType().GetTransparency();
		//				if (transparency != 0) {
		//
		//					float r;
		//					float g;
		//					float b;
		//					float a;
		//
		//					Color right = GetDiffuseLight(new Vector3Int(x + 1, y, z));
		//					Color forward = GetDiffuseLight(new Vector3Int(x, y, z + 1));
		//					Color left = GetDiffuseLight(new Vector3Int(x - 1, y, z));
		//					Color back = GetDiffuseLight(new Vector3Int(x, y, z - 1));
		//					Color up = GetDiffuseLight(new Vector3Int(x, y + 1, z));
		//					Color down = GetDiffuseLight(new Vector3Int(x, y - 1, z));
		//
		//					r = (right.r + left.r + forward.r + back.r + up.r + down.r + currentVoxel.voxelLighting.r) / 7f;
		//					g = (right.g + left.g + forward.g + back.g + up.g + down.g + currentVoxel.voxelLighting.g) / 7f;
		//					b = (right.b + left.b + forward.b + back.b + up.b + down.b + currentVoxel.voxelLighting.b) / 7f;
		//					a = Mathf.Max(right.a, left.a, forward.a, back.a, up.a, down.a, currentVoxel.voxelLighting.a);
		//
		//					Color newLighting = new Color(r, g, b, a);
		//
		//					currentVoxel.voxelLighting = newLighting;
		//				}
		//			}
		//
		//		}
		//	}
		//}
	}

	private Color GetDiffuseLight(Vector3Int position) {
		VoxelState neighbor;
		Vector3Int neighborPosition = position;
	
		if (IsVoxelInChunk(neighborPosition)) neighbor = GetVoxelAt(neighborPosition);
		else {
			Vector3Int neighborWorldPosition = LocalToWorld(neighborPosition);
			neighbor = World.GetVoxelAt(neighborWorldPosition);
		}
	
		Color diffusedLight = neighbor.voxelLighting;
	
		// Give off light
		Color emmision = neighbor.GetVoxelType().GetEmmision();
		if (emmision.a > 0) {
			float alpha = diffusedLight.a;
			diffusedLight = emmision;//Color.Lerp(diffusedLight, emmision, emmision.a);
			alpha = Mathf.Max(alpha, emmision.a);
			diffusedLight.a = alpha;
		}
	
		//diffusedLight.a -= VoxelLightingData.LightFalloff;
		return diffusedLight;
	}

	public Vector3Int LocalToWorld(Vector3Int localPosition) {
		int worldX = localPosition.x + (ChunkPosition.x * ChunkWidth);
		int worldZ = localPosition.z + (ChunkPosition.y * ChunkWidth);

		return new Vector3Int(worldX, localPosition.y, worldZ);
	}

	public Vector3Int LocalToWorld(int x, int y, int z) {
		int worldX = x + (ChunkPosition.x * ChunkWidth);
		int worldZ = z + (ChunkPosition.y * ChunkWidth);

		return new Vector3Int(worldX, y, worldZ);
	}

	public void RandomlyTick() {
		int x = Random.Range(0, ChunkWidth);
		int y = Random.Range(0, ChunkHeight);
		int z = Random.Range(0, ChunkWidth);

		voxelMap[x, y, z].GetVoxelType().OnRandomTick(World, LocalToWorld(x, y, z));

	}

}
