using UnityEngine;

namespace CMUI.Pages {

	public class Page : MonoBehaviour {

		[SerializeField] private CanvasGroup pageObject;
		protected Menu menu;

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

		public void ShowPage() {
			pageObject.gameObject.SetActive(true);
		}

		public void HidePage() {
			pageObject.gameObject.SetActive(false);
		}

		public void SetMenu(Menu menu) {
			this.menu = menu;
		}

	}

}
