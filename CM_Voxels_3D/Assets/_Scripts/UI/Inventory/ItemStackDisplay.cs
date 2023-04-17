using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStackDisplay : MonoBehaviour {

	[SerializeField] private TMPro.TMP_Text itemName;
	[SerializeField] private TMPro.TMP_Text itemCount;

	public void SetItemStack(ItemStack itemStack) {
		itemName.text = itemStack.Voxel.GetVoxelID();
		itemCount.text = itemStack.Count.ToString();
	}

}
