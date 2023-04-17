using CMUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenu : Menu {
	
	public Inventory TargetInventory { get; set; }

	public void RefreshUI() {
		InventoryPage inventoryPage = (GetActivePage() as InventoryPage);

		inventoryPage.LimitItems((ItemStack _) => true);
		inventoryPage.RefreshUI();

	}

}
