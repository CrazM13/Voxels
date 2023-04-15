using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CMVoxels;

public class World : MonoBehaviour {

	public TextAsset worldGeneratorSettings;
	public Material voxelMaterial;

	public ChunkRenderingPool chunkRendererPool;

	private Vector3 worldSpawnPosition;

	private Chunk[,] chunks;
	private List<Vector2Int> activeChunks = new List<Vector2Int>();

	private WorldGenerator worldGenerator;

	private List<Vector2Int> chunksToCreate = new List<Vector2Int>();

	private Thread generationThread;

	private List<Vector2Int> chunksToUnload = new List<Vector2Int>();
	private List<Vector2Int> chunksToLoad = new List<Vector2Int>();

	private void Awake() {
		DataManager.LoadData();

		chunkRendererPool.CreateChunks();

		worldGenerator = new OverworldGenerator(this, JsonUtility.FromJson<WorldGeneratorSettings>(worldGeneratorSettings.text));

		chunks = new Chunk[worldGenerator.WorldSizeInChunks, worldGenerator.WorldSizeInChunks];
		worldSpawnPosition = new Vector3((worldGenerator.ChunkWidth * worldGenerator.WorldSizeInChunks) / 2f, worldGenerator.ChunkHeight, (worldGenerator.ChunkWidth * worldGenerator.WorldSizeInChunks) / 2f);

		generationThread = new Thread(new ThreadStart(UpdateGeneration));
		generationThread.Start();
	}

	private void Update() {
		if (chunksToLoad.Count > 0 || chunksToUnload.Count > 0) {
			UpdateChunkLoading();
		}

		foreach (Vector2Int chunk in activeChunks) {
			if (chunks[chunk.x, chunk.y] != null) {
				chunks[chunk.x, chunk.y].RandomlyTick();
			}
		}
	}

	private void UpdateGeneration() {
		while(true) {
			if (chunksToCreate.Count > 0) {
				worldGenerator.PopulateChunkTerrain(chunksToCreate[0]);
				chunks[chunksToCreate[0].x, chunksToCreate[0].y].IsPopulated = true;

				worldGenerator.PopulateChunkStructures(chunksToCreate[0]);

				chunksToCreate.RemoveAt(0);
			}
		}
	}

	private void OnDisable() {
		Debug.Log("Ending threads");
		generationThread.Abort();
	}

	public void UnloadChunks(Vector2Int currentChunk) {

		for (int x = currentChunk.x - worldGenerator.ViewDistanceInChunks; x < currentChunk.x + worldGenerator.ViewDistanceInChunks; x++) {
			for (int z = currentChunk.y - worldGenerator.ViewDistanceInChunks; z < currentChunk.y + worldGenerator.ViewDistanceInChunks; z++) {
				Vector2Int targetChunkPosition = new Vector2Int(x, z);

				if (IsChunkInWorld(targetChunkPosition)) {
					float distance = Vector2Int.Distance(currentChunk, targetChunkPosition);

					if (distance <= worldGenerator.ViewDistanceInChunks) {
						//if (chunksToLoad.Contains(targetChunkPosition)) chunksToLoad.Remove(targetChunkPosition);
						if (!chunksToUnload.Contains(targetChunkPosition)) chunksToUnload.Add(targetChunkPosition);
					}
				}
			}
		}
	}

	public void LoadChunks(Vector2Int currentChunk) {
		for (int x = currentChunk.x - worldGenerator.ViewDistanceInChunks; x < currentChunk.x + worldGenerator.ViewDistanceInChunks; x++) {
			for (int z = currentChunk.y - worldGenerator.ViewDistanceInChunks; z < currentChunk.y + worldGenerator.ViewDistanceInChunks; z++) {
				Vector2Int targetChunkPosition = new Vector2Int(x, z);
		
				if (IsChunkInWorld(targetChunkPosition)) {
					float distance = Vector2Int.Distance(currentChunk, targetChunkPosition);
		
					if (distance <= worldGenerator.ViewDistanceInChunks) {
						if (chunksToUnload.Contains(targetChunkPosition)) chunksToUnload.Remove(targetChunkPosition);
						if (!chunksToLoad.Contains(targetChunkPosition)) chunksToLoad.Add(targetChunkPosition);
					}
				}
			}
		}
	}

	private void LoadSingleChunk(Vector2Int chunk) {
		if (chunks[chunk.x, chunk.y] == null) {
			CreateChunk(chunk.x, chunk.y);
			ScheduleGenerateChunk(chunk);
			chunks[chunk.x, chunk.y].IsActive = true;
			chunkRendererPool.AssignChunkForRendering(chunks[chunk.x, chunk.y]);
		} else {
			if (!chunks[chunk.x, chunk.y].IsActive) {
				chunks[chunk.x, chunk.y].IsActive = true;
				chunkRendererPool.AssignChunkForRendering(chunks[chunk.x, chunk.y]);
			}

			if (!chunks[chunk.x, chunk.y].IsPopulated) ScheduleGenerateChunk(chunk);
		}

		activeChunks.Add(chunk);
	}

	private void UnloadSingleChunk(Vector2Int chunk) {
		if (chunks[chunk.x, chunk.y] != null) {
			if (chunks[chunk.x, chunk.y].IsActive) chunks[chunk.x, chunk.y].IsActive = false;
		}

		activeChunks.Remove(chunk);
		chunkRendererPool.UnassignChunkForRendering(chunks[chunk.x, chunk.y]);
	}

	public void UpdateChunkLoading() {
		while (chunksToUnload.Count > 0) {
			UnloadSingleChunk(chunksToUnload[0]);
			chunksToUnload.RemoveAt(0);
		}

		while (chunksToLoad.Count > 0) {
			LoadSingleChunk(chunksToLoad[0]);
			chunksToLoad.RemoveAt(0);
		}
	}

	public void ScheduleGenerateChunk(Vector2Int chunk) {
		if (!chunksToCreate.Contains(chunk)) chunksToCreate.Add(chunk);
	}

	public Vector2Int ConvertPositionToChunk(Vector3 position) {
		return ConvertPositionToChunk(position.x, position.z);
	}

	public Vector2Int ConvertPositionToChunk(float x, float z) {
		int chunkX = Mathf.FloorToInt(x / worldGenerator.ChunkWidth);
		int chunkZ = Mathf.FloorToInt(z / worldGenerator.ChunkWidth);
	
		return new Vector2Int(chunkX, chunkZ);
	}

	private void CreateChunk(int x, int z) {
		if (chunks[x, z] != null) return;
	
		Vector2Int chunkPosition = new Vector2Int(x, z);
		chunks[x, z] = new Chunk(this, chunkPosition);
	}

	public bool IsChunkInWorld(Vector2Int chunkPosition) {
		return chunkPosition.x >= 0 && chunkPosition.x < worldGenerator.WorldSizeInChunks - 1
			&& chunkPosition.y >= 0 && chunkPosition.y < worldGenerator.WorldSizeInChunks - 1;
	}

	public bool IsVoxelInWorld(Vector3Int voxelPosition) {
		return IsVoxelInWorld(voxelPosition.x, voxelPosition.y, voxelPosition.z);
	}

	public bool IsVoxelInWorld(int x, int y, int z) {
		return x >= 0 && x < worldGenerator.WorldSizeInVoxels
			&& y >= 0 && y < worldGenerator.ChunkHeight
			&& z >= 0 && z < worldGenerator.WorldSizeInVoxels;
	}

	public WorldGenerator GetWorldGenerator() => worldGenerator;

	public bool SetVoxel(Vector3Int position, Voxel voxel, bool rerenderChunk = true) {
		return SetVoxel(position.x, position.y, position.z, voxel, rerenderChunk);
	}

	public bool SetVoxel(int x, int y, int z, Voxel voxel, bool forceRerenderChunk = true) {
		if (!IsVoxelInWorld(x, y, z)) {
			return false;
		}

		Vector2Int chunk = ConvertPositionToChunk(x, z);


		int voxelX = x - (chunk.x * worldGenerator.ChunkWidth);
		int voxelZ = z - (chunk.y * worldGenerator.ChunkWidth);

		if (chunks[chunk.x, chunk.y] == null) {
			CreateChunk(chunk.x, chunk.y);
		}

		bool result = chunks[chunk.x, chunk.y].SetVoxelAt(new Vector3Int(voxelX, y, voxelZ), voxel, forceRerenderChunk);
		
		// Rerender adjacent chunks if needed
		if (result) {
			if (voxelX == 0) SetChunkDirty(chunk + Vector2Int.left);
			if (voxelX == worldGenerator.ChunkWidth - 1) SetChunkDirty(chunk + Vector2Int.right);
			if (voxelZ == worldGenerator.ChunkWidth - 1) SetChunkDirty(chunk + Vector2Int.up);
			if (voxelZ == 0) SetChunkDirty(chunk + Vector2Int.down);
		}
		
		return result;
	}

	public VoxelState GetVoxelAt(Vector3 position) {
		return GetVoxelAt(position.x, position.y, position.z);
	}

	public VoxelState GetVoxelAt(float x, float y, float z) {
		return GetVoxelAt(Mathf.RoundToInt(x), Mathf.RoundToInt(y), Mathf.RoundToInt(z));
	}

	public VoxelState GetVoxelAt(Vector3Int position) {
		return GetVoxelAt(position.x, position.y, position.z);
	}

	public VoxelState GetVoxelAt(int x, int y, int z) {
		if (!IsVoxelInWorld(x, y, z)) return VoxelState.EMPTY;

		Vector2Int chunk = ConvertPositionToChunk(x, z);


		int voxelX = x - (chunk.x * worldGenerator.ChunkWidth);
		int voxelZ = z - (chunk.y * worldGenerator.ChunkWidth);

		if (chunks[chunk.x, chunk.y] == null) return VoxelState.EMPTY;
		return chunks[chunk.x, chunk.y].GetVoxelAt(new Vector3Int(voxelX, y, voxelZ));
	}

	public Vector3 GetWorldSpawn() => worldSpawnPosition;

	public void SetChunkDirty(Vector2Int targetChunkPosition) {
		if (IsChunkInWorld(targetChunkPosition) && chunks[targetChunkPosition.x, targetChunkPosition.y] != null) chunks[targetChunkPosition.x, targetChunkPosition.y].IsDirty = true;
	}

}
