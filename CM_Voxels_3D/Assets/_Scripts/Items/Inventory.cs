using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

	public List<ItemStack> Items { get; private set; }

	public Inventory() {
		Items = new List<ItemStack>();
	}

	public List<ItemStack> Search(System.Predicate<ItemStack> condition) {
		return Items.FindAll(condition);
	}

	public void AddItemStack(ItemStack itemStack) {
		foreach (ItemStack stack in Items) {
			if (stack.Voxel == itemStack.Voxel) {
				stack.Count += itemStack.Count;
				return;
			}
		}

		Items.Add(itemStack);
	}

}
