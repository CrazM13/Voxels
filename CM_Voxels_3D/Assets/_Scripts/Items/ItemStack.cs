using CMVoxels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStack {

	public Voxel Voxel { get; set; }
	public int Count { get; set; }

	public ItemStack(Voxel voxel, int count = 1) {
		this.Voxel = voxel;
		this.Count = count;
	}

}
