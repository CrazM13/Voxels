using CMUI.Pages;
using CMUI.Tabs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPage : Page {

	[Header("Tabs")]
	[SerializeField] private TabGroup tabGroup;

	[Header("Inventory")]
	[SerializeField] private ItemStackDisplay itemPrefab;
	[SerializeField] private Transform container;

	private List<ItemStack> displayableItems = new List<ItemStack>();

	private List<ItemStackDisplay> itemUIs = new List<ItemStackDisplay>();

	public void LimitItems(System.Predicate<ItemStack> condition) {
		displayableItems = ((InventoryMenu) menu).TargetInventory.Search(condition);
	}

	public void RefreshUI() {
		foreach (ItemStackDisplay itemStackUI in itemUIs) Destroy(itemStackUI.gameObject);
		itemUIs.Clear();

		for (int i = 0; i < displayableItems.Count; i++) {
			ItemStackDisplay newDisplay = Instantiate(itemPrefab.gameObject, container).GetComponent<ItemStackDisplay>();
			newDisplay.SetItemStack(displayableItems[i]);
			itemUIs.Add(newDisplay);
		}
	}

}
