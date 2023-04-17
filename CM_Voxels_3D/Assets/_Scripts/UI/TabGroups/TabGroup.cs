using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CMUI.Tabs {
	public class TabGroup : MonoBehaviour {

		private List<Tab> tabs;
		public UnityEvent<TabGroup> OnTabChanged = new UnityEvent<TabGroup>();

		public void Subscribe(Tab tab) {
			tabs.Add(tab);
		}

		public void SelectTab(Tab tab) {
			foreach (Tab t in tabs) {
				if (t != tab) {
					t.IsSelected = false;
					t.SendStateChange();
				}
			}

			tab.IsSelected = true;
			tab.SendStateChange();

			OnTabChanged.Invoke(this);
		}

	}
}
