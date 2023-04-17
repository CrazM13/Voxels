using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CMUI.Tabs {
	public class Tab : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

		[SerializeField] private TabGroup tabGroup;

		public UnityEvent<Tab> OnStateChanged = new UnityEvent<Tab>();

		public bool IsHovering { get; set; }
		public bool IsSelected { get; set; }

		public void OnPointerClick(PointerEventData eventData) {
			IsHovering = false;

			tabGroup.SelectTab(this);
		}

		public void OnPointerEnter(PointerEventData eventData) {
			IsHovering = true;
			SendStateChange();
		}

		public void OnPointerExit(PointerEventData eventData) {
			IsHovering = false;
			SendStateChange();
		}

		public void SendStateChange() {
			OnStateChanged.Invoke(this);
		}

		private void Awake() {
			tabGroup.Subscribe(this);
		}

	}
}
