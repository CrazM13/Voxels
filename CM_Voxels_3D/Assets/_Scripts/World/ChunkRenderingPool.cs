using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRenderingPool : MonoBehaviour {

	[SerializeField] private int maxChunks;

	private Queue<ChunkRenderer> unassignedChunks = new Queue<ChunkRenderer>();
	private Dictionary<Vector2Int, ChunkRenderer> assignedChunks = new Dictionary<Vector2Int, ChunkRenderer>();

	public void CreateChunks() {
		for (int i = 0; i < maxChunks; i++) {
			GameObject chunkObject = new GameObject($"Chunk (Pool # {i})");
			chunkObject.transform.SetParent(this.transform);

			ChunkRenderer newRenderer = chunkObject.AddComponent<ChunkRenderer>();
			chunkObject.SetActive(false);

			unassignedChunks.Enqueue(newRenderer);
		}
	}

	public void AssignChunkForRendering(Chunk chunk) {
		if (unassignedChunks.Count <= 0) return;

		Vector2Int chunkPosition = chunk.ChunkPosition;
		ChunkRenderer chunkRenderer = unassignedChunks.Dequeue();

		chunkRenderer.ChunkData = chunk;
		chunkRenderer.transform.position = new Vector3(chunkPosition.x * chunk.ChunkWidth, 0, chunkPosition.y * chunk.ChunkWidth);

		chunkRenderer.gameObject.SetActive(true);

		assignedChunks.Add(chunkPosition, chunkRenderer);
	}

	public void UnassignChunkForRendering(Chunk chunk) {
		Vector2Int chunkPosition = chunk.ChunkPosition;

		if (!assignedChunks.ContainsKey(chunkPosition)) return;
		
		ChunkRenderer chunkRenderer = assignedChunks[chunkPosition];
		assignedChunks.Remove(chunkPosition);

		chunkRenderer.ChunkData = null;
		chunkRenderer.transform.position = Vector3.zero;

		chunkRenderer.gameObject.SetActive(false);

		unassignedChunks.Enqueue(chunkRenderer);
	}

}
