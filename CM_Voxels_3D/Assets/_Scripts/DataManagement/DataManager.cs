using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxels;
using Voxels.VoxelModels;

public class DataManager {

	public static VoxelManager @VoxelManager { get; private set; } = new VoxelManager();
	public static VoxelModelManager @VoxelModelManager { get; private set; } = new VoxelModelManager();

}
