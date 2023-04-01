using CMUI.Pages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMUI {
	public class Menu : MonoBehaviour {

		[SerializeField] private CanvasGroup[] pageObjects;
		[SerializeField] private string rootPage;

		Dictionary<string, Page> pagesCache = new Dictionary<string, Page>();

		private Stack<string> pagesOrder = new Stack<string>();

		// Start is called before the first frame update
		void Awake() {
			foreach (CanvasGroup pageObject in pageObjects) {
				if (pagesCache.ContainsKey(pageObject.name)) {
					Debug.LogWarning($"Multiple pages with name {pageObject.name} in menu {name}!");
				} else {
					pagesCache.Add(pageObject.name, new Page(pageObject));
				}
			}

			MenuManager.RegisterMenu(this);
		}

		private void OnDisable() {
			MenuManager.UnregisterMenu(this);
		}

		public void OpenPage(string pageName) {
			Page page = pagesCache[pageName];

			page.OpenPage();

			if (pagesOrder.Count > 0) {
				Page activePage = pagesCache[pagesOrder.Peek()];
				activePage.IsActivePage = false;
			}
			page.IsActivePage = true;
		}

		public void ClosePage() {
			Page activePage = pagesCache[pagesOrder.Pop()];

			activePage.ClosePage();

			if (pagesOrder.Count > 0) {
				activePage = pagesCache[pagesOrder.Peek()];

				activePage.IsActivePage = true;
			}
		}

		public string GetRootPage() => rootPage;


	}
}
