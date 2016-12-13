#if !(UNITY_4 || UNITY_5)
using System;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace TerrainGen {
	public class Loader : LoadingExtensionBase {
		private TerrainUIMainPanel panel;
		private GameObject window;
		private GameObject button1;
		private GameObject button2;
		private UIButton menuButton;
		private UITabstrip strip = null;
		private bool initialized;

		public override void OnLevelLoaded(LoadMode m)
		{
			if ( m != LoadMode.NewMap ) {
				return;
			}

			window = new GameObject("Terrain Panel");

			UIView v = UIView.GetAView();

			strip = UIView.Find<UITabstrip>("MainToolstrip");

			button1 = UITemplateManager.GetAsGameObject("MainToolbarButtonTemplate");
			button2 = UITemplateManager.GetAsGameObject("ScrollablePanelTemplate");
			menuButton = strip.AddTab("TerrainGenerator", button1, button2, new Type[] { }) as UIButton;
			menuButton.eventClick += uiButton_eventClick;

			menuButton.normalFgSprite = "InfoIconTerrainHeight";
			menuButton.hoveredFgSprite = "InfoIconTerrainHeightHovered";
			menuButton.focusedFgSprite = "InfoIconTerrainHeightFocused";
			menuButton.pressedFgSprite = "InfoIconTerrainHeightPressed";
			menuButton.tooltip = "Generate Terrain";

			panel = window.AddComponent<TerrainUIMainPanel>();
			panel.transform.parent = v.transform;
			panel.position = new Vector3(menuButton.position.x-240, menuButton.position.y - 105);
			panel.Hide ();

			initialized = true;
		}		

		private void uiButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
			if (panel.isVisible) {
				panel.isVisible = false;
				panel.Hide();
			} else {
				panel.isVisible = true;
				panel.BringToFront();
				panel.Show();
			}
		}

		public override void OnLevelUnloading() {
			if (initialized && menuButton != null) {
				menuButton.eventClick -= uiButton_eventClick;
				strip.RemoveUIComponent(menuButton);
				UIView.Destroy(menuButton);
				UIView.Destroy(panel);
				UIView.Destroy(window);
				initialized = false;
			}
		}
	}
}
#endif
