using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMVoxels;

public class Chunk {

	private GameObject chunkObject;

	private VoxelState[,,] voxelMap;

	public World World { get; private set; }
	public bool IsInitialized { get; private set; }
	public bool IsPopulated { get; set; }

	public bool IsDirty { get; set; }

	public Vector2Int ChunkPosition { get; private set; }

	public bool IsActive { get; set; }

	public Vector3 Position => chunkObject.transform.position;

	public int ChunkWidth => voxelMap.GetLength(0);
	public int ChunkHeight => voxelMap.GetLength(1);

	public Chunk(World world, Vector2Int chunkPosition) {
		this.World = world;
		this.ChunkPosition = chunkPosition;

		WorldGenerator worldGenerator = world.GetWorldGenerator();

		voxelMap = new VoxelState[worldGenerator.ChunkWidth, worldGenerator.ChunkHeight, worldGenerator.ChunkWidth];

		Initialize();
	}

	public void Initialize() {
		if (IsInitialized) return;
		IsInitialized = true;
		PopulateVoxelMap();
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

		//if (forceRerender) ReloadRenderer();
		IsDirty = true;

		return true;
	}

	public void CalculateLight() {
		// Emmision Pass
		for (int x = 0; x < ChunkWidth; x++) {
			for (int z = 0; z < ChunkWidth; z++) {
				float skyLight = 1f; // Skylight from skybox

				for (int y = ChunkHeight - 1; y >= 0; y--) {
					VoxelState currentVoxel = voxelMap[x, y, z];
					skyLight *= currentVoxel.GetVoxelType().GetTransparency();
					
					// Apply light from light source
					if (currentVoxel.GetVoxelType().IsLightSource()) {
						VoxelLightColour emmision = currentVoxel.GetVoxelType().GetEmmision();
						currentVoxel.voxelLighting.Increase(emmision.R, emmision.G, emmision.B);
					}

					currentVoxel.voxelLighting.SkyInfluence = skyLight;
				}

			}
		}

		// Diffuse Pass
		// Top level loop is number of passes. More passes = better lighting
		for (int _ = 0; _ < VoxelLightingData.LightPasses; _++) {
			for (int x = 0; x < ChunkWidth; x++) {
				for (int z = 0; z < ChunkWidth; z++) {
		
					for (int y = ChunkHeight - 1; y >= 0; y--) {
						VoxelState currentVoxel = voxelMap[x, y, z];
		
						float transparency = currentVoxel.GetVoxelType().GetTransparency();
						if (transparency != 0) {
		
							float r;
							float g;
							float b;
							float sky;
		
							VoxelLightColour right = GetDiffuseLight(new Vector3Int(x + 1, y, z));
							VoxelLightColour forward = GetDiffuseLight(new Vector3Int(x, y, z + 1));
							VoxelLightColour left = GetDiffuseLight(new Vector3Int(x - 1, y, z));
							VoxelLightColour back = GetDiffuseLight(new Vector3Int(x, y, z - 1));
							VoxelLightColour up = GetDiffuseLight(new Vector3Int(x, y + 1, z));
							VoxelLightColour down = GetDiffuseLight(new Vector3Int(x, y - 1, z));
		
							r = Mathf.Max(right.R, left.R, forward.R, back.R, up.R, down.R, currentVoxel.voxelLighting.R);
							g = Mathf.Max(right.G, left.G, forward.G, back.G, up.G, down.G, currentVoxel.voxelLighting.G);
							b = Mathf.Max(right.B, left.B, forward.B, back.B, up.B, down.B, currentVoxel.voxelLighting.B);
							sky = Mathf.Max(right.SkyInfluence, left.SkyInfluence, forward.SkyInfluence, back.SkyInfluence, up.SkyInfluence, down.SkyInfluence, currentVoxel.voxelLighting.SkyInfluence);
		
							VoxelLightColour newLighting = new VoxelLightColour(r, g, b, sky);
		
							currentVoxel.voxelLighting = newLighting;
						}
					}
		
				}
			}
		}
	}

	private VoxelLightColour GetDiffuseLight(Vector3Int position) {
		VoxelState neighbor;
		Vector3Int neighborPosition = position;
	
		if (IsVoxelInChunk(neighborPosition)) neighbor = GetVoxelAt(neighborPosition);
		else {
			Vector3Int neighborWorldPosition = LocalToWorld(neighborPosition);
			neighbor = World.GetVoxelAt(neighborWorldPosition);
		}

		VoxelLightColour diffusedLight = neighbor.voxelLighting.Copy();

		diffusedLight.ApplyDiffusion();
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
