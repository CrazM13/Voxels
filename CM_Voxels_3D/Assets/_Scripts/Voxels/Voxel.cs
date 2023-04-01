using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMVoxels.VoxelModels;

namespace CMVoxels {
	public class Voxel {

		private string voxelID;
		private bool isSolid;
		private bool renderNeighborFaces;
		private Color transparency;
		private Color emmision;

		private VoxelModel model;



		public Voxel(string voxelID, string modelID) {
			this.voxelID = voxelID;
			this.model = DataManager.Models.LoadResource(modelID);

			isSolid = false;
			renderNeighborFaces = false;
			transparency = Color.clear;
			emmision = Color.clear;
		}

		#region Events
		public virtual void OnRandomTick(World world, Vector3Int position) { /*MT*/ }
		public virtual void OnInteract(World world, Vector3Int position, VoxelEntity source) { /*MT*/ }
		#endregion

		#region Getters
		public VoxelModel Model => model;

		public bool IsLightSource() => emmision.maxColorComponent > 0;

		public Color GetEmmision() => emmision;

		public float GetTransparency() => transparency.maxColorComponent;

		public Color GetTint() => transparency;

		public bool IsSolid() => isSolid;

		public bool ShouldRenderNeighborFaces() => renderNeighborFaces;

		public string GetVoxelID() => voxelID;
		#endregion

		#region Setters
		public void SetSolid(bool value) {
			isSolid = value;
		}

		public void SetRenderNeighborFaces(bool value) {
			renderNeighborFaces = value;
		}

		public void SetTransparency(Color value) {
			transparency = value;
		}

		public void SetEmmision(Color value) {
			emmision = value;
		}
		#endregion

	}
}
