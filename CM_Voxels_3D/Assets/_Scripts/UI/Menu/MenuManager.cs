using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMUI {
	public static class MenuManager {

		private static Dictionary<string, Menu> menusCache = new Dictionary<string, Menu>();

		public static void RegisterMenu(Menu menu) {
			if (menusCache.ContainsKey(menu.name)) {
				Debug.LogWarning($"Multiple menus with name {menu.name}!");
			} else {
				menusCache.Add(menu.name, menu);
			}
		}

		public static void UnregisterMenu(Menu menu) {
			if (menusCache.ContainsKey(menu.name)) {
				menusCache.Remove(menu.name);
			} else {
				Debug.LogWarning($"No menus with name {menu.name}!");
			}
		}

		public static void OpenMenu(string menuName) {
			Menu menu = menusCache[menuName];
			menu.OpenPage(menu.GetRootPage());
		}

	}
}
