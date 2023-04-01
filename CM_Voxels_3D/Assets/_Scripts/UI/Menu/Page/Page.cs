using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMUI.Pages {

	public class Page {

		private CanvasGroup pageObject;


		private bool isActivePage = false;
		public bool IsActivePage {
			get => isActivePage;
			set {
				if (pageObject) {
					pageObject.interactable = value;
					pageObject.blocksRaycasts = value;
				}

				isActivePage = value;
			}
		}

		public Page(CanvasGroup pageObject) {
			this.pageObject = pageObject;

			ClosePage();
			IsActivePage = false;
		}

		public void OpenPage() {
			pageObject.gameObject.SetActive(true);
		}

		public void ClosePage() {
			pageObject.gameObject.SetActive(false);
		}

	}

}
