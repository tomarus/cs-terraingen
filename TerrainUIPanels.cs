using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;

namespace TerrainGen {

	public class TerrainUITerrainPanel : UIPanel {
		private UIButton okButton;
		private UIButton flatButton;
		private UISlider smoothSlider;
		private UISlider scaleSlider;
		private UISlider offsetSlider;
		private UISlider blurSlider;

		private float smoothness = 8;
		private float scale = 0.75f;
		private float offset = 0.0f;
		private float blur = 1.0f;

		public override void OnDestroy() {
			Destroy(smoothSlider);
			Destroy(scaleSlider);
			Destroy(offsetSlider);
			Destroy(blurSlider);
			Destroy(okButton);
		}

		public override void Start() {
			okButton = TerrainUI.MakeButton(this, "Generate Terrain", 10, 210, 135);
			okButton.eventClick += okButton_eventClick;

			flatButton = TerrainUI.MakeButton(this, "Flatten", 155, 210, 70);
			flatButton.eventClick += flattenButton_eventClick;

			int y = 8;
			smoothSlider = TerrainUI.MakeSlider (this, "SmoothSlider", "Smoothness", y, 9.0f, 0.0f, 10.0f, 1.0f, value => { smoothness = value; }); y+=50;
			scaleSlider = TerrainUI.MakeSlider (this, "ScaleSlider", "Scale", y, scale, 0.0f, 1.0f, 0.05f, value => { scale = value; }); y+=50;
			offsetSlider = TerrainUI.MakeSlider (this, "OffsetSlider", "Offset", y, offset, -1.0f, 1.0f, 0.05f, value => { offset = value; }); y+=50;
			blurSlider = TerrainUI.MakeSlider (this, "BlurSlider", "Blur", y, blur, 0.0f, 2.0f, 1.00f, value => { blur = value; }); y+=50;
		}

		private void okButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
			TerraGen.tg.DoTerrain ((int)Mathf.Pow (2.0f, smoothness), scale, offset, (int)Mathf.Pow (2.0f, blur) + 1);
		}

		private void flattenButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
			TerraGen.tg.FlattenTerrain();
		}
	}


	public class TerrainUIResourcePanel : UIPanel {
		private UIButton okButton;
		private UISlider smoothSlider;
		private UISlider scaleSlider;
		private UISlider forestSlider;
		private UICustomCheckbox deltreesCheckbox;

		private float smoothness = 6f;
		private float scale = 0.75f;
		private float forest = 0.5f;

		public override void OnDestroy() {
			Destroy(smoothSlider);
			Destroy(scaleSlider);
			Destroy(forestSlider);
			Destroy(okButton);
		}

		public override void Start() {
			float y = 8.0f;
			smoothSlider = TerrainUI.MakeSlider (this, "SmoothSlider", "Smoothness", y, smoothness, 0.0f, 9.0f, 1.0f, value => { smoothness = value; });
			y += 50;
			scaleSlider = TerrainUI.MakeSlider (this, "ScaleSlider", "Scale", y, scale, 0.0f, 1.0f, 0.05f, value => { scale = value; });
			y += 50;
			forestSlider = TerrainUI.MakeSlider (this, "ForestSlider", "Forest Level", y, forest, 0.0f, 1.0f, 0.05f, value => { forest = value; }, "Small values create thin lines around areas. Larger values generate big forest areas.");

			y = 180;
			deltreesCheckbox = TerrainUI.MakeCheckBox (this, "Delete trees before generating.", y, deltree_eventClick);

			okButton = TerrainUI.MakeButton(this, "Generate Resources", 10, 210, 160);
			okButton.eventClick += okButton_eventClick;
		}

		private void okButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
			if ( deltreesCheckbox.isChecked )
				TerraGen.tg.DeleteAllTrees();
			TerraGen.tg.DoResources ((int)Mathf.Pow (2.0f, smoothness), scale, 0.0f, 3, forest);
		}

		private void deltree_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
			deltreesCheckbox.isChecked =! deltreesCheckbox.isChecked;
		}
	}


	public class TerrainUITreePanel : UIPanel {
		private UIButton delallButton;
		private UIButton okButton;
		private UICustomCheckbox followCheckbox;
		private UICustomCheckbox insideCheckbox;
		private TreeCollection treeCol;

		private int treeNum = 0;
		private float density = 16.0f;

		public override void OnDestroy() {
			Destroy(delallButton);
			Destroy(okButton);
			Destroy(followCheckbox);
			Destroy(insideCheckbox);
		}

		public override void Start() {
			float y = 0.0f;

			treeCol = GameObject.FindObjectOfType<TreeCollection>();
			string[] items = new string[treeCol.m_prefabs.Length + 1];
			items[0] = "Use Random Trees";
			for (int n=0; n<treeCol.m_prefabs.Length; n++) {
				items[n+1] = treeCol.m_prefabs[n].name;
			}

			TerrainUI.MakeDropDown(this, y, "Tree Type", items, listbox_eventClick);
			y += 50;

			followCheckbox = TerrainUI.MakeCheckBox(this, "Follow Resources", y, followcb_eventClick, "Use forest resources to place trees. If unchecked trees are placed randomly.");
			y+=30;

			insideCheckbox = TerrainUI.MakeCheckBox(this, "Only inside game area.", y, insidecb_eventClick, "Do not place trees outside the center 25 tiles.");
			y+=30;

			TerrainUI.MakeSlider (this, "DensitySlider", "Max Density", y, density, 1.0f, 64.0f, 1.0f, value => { density = value; });
			y += 45;

			UILabel txt = AddUIComponent<UILabel>();
			txt.text = "Please Note: It can take a couple\nof minutes for the trees\nto show on the map.";
			txt.name = "TreePlacementInfoText";
			txt.relativePosition = new Vector3(15.0f, y);
			txt.textScale = 0.8f;
			txt.textColor = new Color(.7f,.7f,.7f,1f);
			txt.size = new Vector2(200, 86);
			txt.autoSize = false;
			txt.autoHeight = false;

			okButton = TerrainUI.MakeButton(this, "Create Trees", 10, 210, 110);
			okButton.eventClick += okButton_eventClick;

			delallButton = TerrainUI.MakeButton(this, "Delete All", 140, 210, 90);
			delallButton.eventClick += delallButton_eventClick;
		}

		private void delallButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
			TerraGen.tg.DeleteAllTrees();
		}

		private void okButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
			//TerraGen.tg.DeleteAllTrees();
			TerraGen.tg.DoTrees(followCheckbox.isChecked, treeNum, insideCheckbox.isChecked, (int)density);
		}

		private void followcb_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
			followCheckbox.isChecked =! followCheckbox.isChecked;
		}

		private void insidecb_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
			insideCheckbox.isChecked =! insideCheckbox.isChecked;
		}

		private void listbox_eventClick(UIComponent component, int value) {
			treeNum = value;
		}
	}


	public class TerrainUIMainPanel : UIPanel {
		private UILabel title;
		private UIDragHandle dragHandle;
		private UITabContainer tabContainer;
		private TerrainUITerrainPanel terrainPanel;
		private TerrainUIResourcePanel resPanel;
		private TerrainUITreePanel treePanel;
		private UITabstrip tabStrip;
		private UIButton closeButton;

		private List<UIButton> tabs;

		public override void Awake() {
			title = AddUIComponent<UILabel>();
			dragHandle = AddUIComponent<UIDragHandle>();
			tabStrip = AddUIComponent<UITabstrip>();
			tabContainer = AddUIComponent<UITabContainer>();
			closeButton = AddUIComponent<UIButton>();

			terrainPanel = tabContainer.AddUIComponent<TerrainUITerrainPanel>();
			resPanel = tabContainer.AddUIComponent<TerrainUIResourcePanel>();
			treePanel = tabContainer.AddUIComponent<TerrainUITreePanel>();
		}

		public override void OnDestroy() {
			foreach (UIButton tab in tabs) {
				Destroy (tab);
			}

			Destroy(treePanel);
			Destroy(resPanel);
			Destroy(terrainPanel);
			Destroy(closeButton);
			Destroy(tabContainer);
			Destroy(tabStrip);
			Destroy(dragHandle);
			Destroy(title);
		}

		public override void Start () {
			this.backgroundSprite = "MenuPanel";
			this.width = 240;  //240
			this.height = 330; //290

			title.text = "Terrain Generator";
			title.relativePosition = new Vector3(15, 15);
			title.textScale = 0.9f;
			title.size = new Vector2(200, 30);

			// Thanks, https://github.com/SamsamTS/CS-MeshInfo :)
			dragHandle.width = this.width;
			dragHandle.height = 50;
			dragHandle.target = this;
			dragHandle.relativePosition = Vector3.zero;

			closeButton.relativePosition = new Vector2(this.width - 34, 6);
			closeButton.normalBgSprite = "buttonclose";
			closeButton.hoveredBgSprite = "buttonclosehover";
			closeButton.focusedBgSprite = "buttonclosepressed";
			closeButton.pressedBgSprite = "buttonclosepressed";
			closeButton.color = new Color(1, 1, 1, .15f);
			closeButton.hoveredColor = new Color(1,1,1,.75f);
			closeButton.eventClick += closeButton_eventClick;

			tabContainer.relativePosition = new Vector3(0,84);
			tabContainer.width = this.width;
			tabContainer.backgroundSprite = "GenericPanel";
			tabContainer.color = new Color(.4f, .4f, .4f, 1.0f);

			tabStrip.width = this.width - 20;
			tabStrip.height = 18;
			tabStrip.relativePosition = new Vector3(10,50);
			tabStrip.startSelectedIndex = 0;
			tabStrip.selectedIndex = -1;

			tabs = new List<UIButton>();
			tabs.Add (TerrainUI.MakeTab (tabStrip, "Terrain", terrainPanel, baseTabButton_eventClick));
			tabs.Add (TerrainUI.MakeTab (tabStrip, "Resources", resPanel, baseTabButton_eventClick));
			tabs.Add (TerrainUI.MakeTab (tabStrip, "Trees", treePanel, baseTabButton_eventClick));

			//terrainPanel.Hide ();
			resPanel.Hide ();
			treePanel.Hide ();
		}

		private void checkTabs() {
			if ( tabs == null )
				return;
			foreach (UIButton tab in tabs) {
				if ( ((UIPanel)tab.objectUserData).isVisible == true && tab.name == "Resources" ) {
					InfoManager.instance.SetCurrentMode( InfoManager.InfoMode.NaturalResources, InfoManager.SubInfoMode.Default );
					return;
				}
			}
			InfoManager.instance.SetCurrentMode( InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default );
		}

		protected override void OnVisibilityChanged () {
			base.OnVisibilityChanged ();
			checkTabs ();
		}

		private void closeButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
			this.Hide ();
		}

		private void baseTabButton_eventClick(UIComponent c, UIMouseEventParameter e) {
			foreach (UIButton tab in tabs) {
				UIPanel p = (UIPanel)tab.objectUserData;
				if ( tab.name == c.name ) {
					p.Show ();
				} else {
					p.Hide ();
				}
			}
			checkTabs ();
		}

		public override void Update() {
			if (Input.GetKeyDown (KeyCode.Escape) && this.isVisible) {
				this.Hide ();
			}
		}
	}
}