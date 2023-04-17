using CMUI.Pages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMUI {
	public class Menu : MonoBehaviour {

		[SerializeField] private Page[] pageObjects;
		[SerializeField] private string rootPage;

		Dictionary<string, Page> pagesCache = new Dictionary<string, Page>();

		private Stack<string> pagesOrder = new Stack<string>();

		public bool IsMenuActive => pagesOrder.Count > 0;

		// Start is called before the first frame update
		void Awake() {
			foreach (Page pageObject in pageObjects) {
				Debug.Log($"Registering page with name {pageObject.name} in menu {name}");
				if (pagesCache.ContainsKey(pageObject.name)) {
					Debug.LogWarning($"Multiple pages with name {pageObject.name} in menu {name}!");
				} else {
					pagesCache.Add(pageObject.name, pageObject);
				}

				pageObject.SetMenu(this);
			}

			MenuManager.RegisterMenu(this);
		}

		private void OnDisable() {
			MenuManager.UnregisterMenu(this);
		}

		public void OpenPage(string pageName) {
			Page page = pagesCache[pageName];

			page.ShowPage();

			if (pagesOrder.Count > 0) {
				Page activePage = pagesCache[pagesOrder.Peek()];
				activePage.IsActivePage = false;
			}
			page.IsActivePage = true;

			pagesOrder.Push(pageName);
		}

		public void ClosePage() {
			ForceClosePage();

			if (pagesOrder.Count > 0) {
				Page activePage = pagesCache[pagesOrder.Peek()];

				activePage.IsActivePage = true;
			} else {
				MenuManager.CloseMenu();
			}
		}

		public void CloseMenu() {
			while(IsMenuActive) {
				ForceClosePage();
			}
		}

		private void ForceClosePage() {
			Page activePage = pagesCache[pagesOrder.Pop()];

			activePage.HidePage();
		}

		public string GetRootPage() => rootPage;

		public Page GetActivePage() {
			return pagesCache[pagesOrder.Peek()];
		}

	}
}
