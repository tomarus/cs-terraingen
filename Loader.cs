using System;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace TerrainGen {
	public class Loader : LoadingExtensionBase {
		private TerrainPanel panel;
		private GameObject window;
		private GameObject button1;
		private GameObject button2;
		private UIButton menuButton;
		
		public override void OnLevelLoaded(LoadMode m)
		{
			if ( m != LoadMode.NewMap ) {
				return;
			}
			
			window = new GameObject("Terrain Panel");	
			UIView v = UIView.GetAView();
			
			UITabstrip strip = null;
			strip = UIView.Find<UITabstrip>("MainToolstrip");
			
			button1 = UITemplateManager.GetAsGameObject("MainToolbarButtonTemplate");
			button2 = UITemplateManager.GetAsGameObject("ScrollablePanelTemplate");
			menuButton = strip.AddTab("TerrainGenerator", button1, button2, new Type[] { }) as UIButton;
			menuButton.eventClick += uiButton_eventClick;
			
			menuButton.normalFgSprite = "ToolbarIconBeautification";
			menuButton.hoveredFgSprite = "ToolbarIconBeautificationHovered";
			menuButton.focusedFgSprite = "ToolbarIconBeautificationFocused";
			menuButton.pressedFgSprite = "ToolbarIconBeautificationPressed";
			menuButton.tooltip = "Generate Terrain";
			
			panel = window.AddComponent<TerrainPanel>();
			panel.transform.parent = v.transform;
			panel.position = new Vector3(menuButton.position.x-240, menuButton.position.y - 160);
			panel.Hide ();

		}		
		
		private void uiButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
			if (!panel.isVisible)	{
				panel.isVisible = true;
				panel.BringToFront();
				panel.Show();
			} else {
				panel.isVisible = false;
				panel.Hide();
			}            
		}
	}
} 