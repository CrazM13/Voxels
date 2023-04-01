using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeData {

	[System.Serializable]
	public class VoxelLayer {

		[System.Serializable]
		public class VoxelLayerComponent {

			public string voxelID;
			public float weight;

			public VoxelLayerComponent() {
				weight = 1;
			}

		}

		public int size;
		public VoxelLayerComponent[] voxels;

		public VoxelLayer() {
			size = int.MaxValue;
		}

	}

	[System.Serializable]
	public class TerrainSettings {

		public float smoothness;
		public float hillSize;
		public float height;

	}

	// Values to determine placement
	public float rainfall;
	public float temperature;

	// Settings
	public TerrainSettings terrainSettings;

	// Voxels
	public VoxelLayer[] layers;

}
