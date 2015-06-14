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
			Destroy(flatButton);
		}

		public override void Start() {
			okButton = TerrainUI.MakeButton(this, "Generate Terrain", 10, 210, 135);
			okButton.eventClick += okButton_eventClick;

			flatButton = TerrainUI.MakeButton(this, "Flatten", 155, 210, 70);
			flatButton.eventClick += flattenButton_eventClick;

			int y = 8;
			int sh = 40;
			smoothSlider = TerrainUI.MakeSlider (this, "SmoothSlider", "Smoothness", y, 9.0f, 0.0f, 10.0f, 1.0f, value => { smoothness = value; }); y+=sh;
			scaleSlider = TerrainUI.MakeSlider (this, "ScaleSlider", "Scale", y, scale, 0.0f, 1.0f, 0.05f, value => { scale = value; }); y+=sh;
			offsetSlider = TerrainUI.MakeSlider (this, "OffsetSlider", "Offset", y, offset, -1.0f, 1.0f, 0.05f, value => { offset = value; }); y+=sh;
			blurSlider = TerrainUI.MakeSlider (this, "BlurSlider", "Blur", y, blur, 0.0f, 2.0f, 1.00f, value => { blur = value; }); y+=sh;
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
		private UISlider oreSlider;
		private UICustomCheckbox deltreesCheckbox;

		private float smoothness = 6f;
		private float scale = 0.75f;
		private float forest = 0.5f;
		private float ore = 0.33f;

		public override void OnDestroy() {
			Destroy(smoothSlider);
			Destroy(scaleSlider);
			Destroy(forestSlider);
			Destroy(oreSlider);
			Destroy(okButton);
		}

		public override void Start() {
			float y = 8.0f;
			int sh = 40;

			smoothSlider = TerrainUI.MakeSlider (this, "SmoothSlider", "Smoothness", y, smoothness, 0.0f, 9.0f, 1.0f, value => { smoothness = value; });
			y += sh;
			scaleSlider = TerrainUI.MakeSlider (this, "ScaleSlider", "Scale", y, scale, 0.0f, 1.0f, 0.05f, value => { scale = value; });
			y += sh;
			forestSlider = TerrainUI.MakeSlider (this, "ForestSlider", "Forest Level", y, forest, 0.0f, 1.0f, 0.05f, value => { forest = value; }, "Small values create thin lines around areas. Larger values generate big forest areas.");
			y += sh;
			oreSlider = TerrainUI.MakeSlider (this, "OreSlider", "Ore Level", y, ore, 0.0f, 1.0f, 0.05f, value => { ore = value; });

			y = 180;
			deltreesCheckbox = TerrainUI.MakeCheckBox (this, "Delete trees before generating.", y, deltree_eventClick);

			okButton = TerrainUI.MakeButton(this, "Generate Resources", 10, 210, 160);
			okButton.eventClick += okButton_eventClick;
		}

		private void okButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
			if ( deltreesCheckbox.isChecked )
				TerraGen.tg.DeleteAllTrees();
			TerraGen.tg.DoResources ((int)Mathf.Pow (2.0f, smoothness), scale, 0.0f, 3, forest, ore);
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

	public delegate void UITerrainSetValue(SquareDiamond.InitMode value);

	public class UITerrainInitConfig  {
		private UICustomCheckbox hi;
		private UICustomCheckbox lo;
		private UICustomCheckbox zero;
		private UICustomCheckbox random;
		private UITerrainSetValue setValue;

		public UITerrainInitConfig(UIPanel panel, string txt, float y, UITerrainSetValue valFunc) {
			UILabel label = panel.AddUIComponent<UILabel>();
			label.name = txt + "Label";
			label.text = txt;
			label.relativePosition = new Vector3(10.0f, y);
			label.textScale = 0.8f;

			hi = TerrainUI.MakeCheckBox(panel, "", y, hi_eventClick, "Set to max height.", 120);
			zero = TerrainUI.MakeCheckBox(panel, "", y, zero_eventClick, "Set to medium height.", 145);
			lo = TerrainUI.MakeCheckBox(panel, "", y, lo_eventClick, "Set to lowest height.", 170);
			random = TerrainUI.MakeCheckBox(panel, "", y, random_eventClick, "Set to random height.",195);
			none ();
			random.isChecked= true;
			setValue = valFunc;
			setValue(SquareDiamond.InitMode.INIT_RANDOM);
		}

		// cheap radiobutton simulation
		private void none() {
			hi.isChecked = false;
			lo.isChecked = false;
			zero.isChecked = false;
			random.isChecked = false;
		}
		private void hi_eventClick(UIComponent c, UIMouseEventParameter e) {
			none();
			hi.isChecked =! hi.isChecked;
			setValue(SquareDiamond.InitMode.INIT_HI);
		}
		private void lo_eventClick(UIComponent c, UIMouseEventParameter e) {
			none();
			lo.isChecked =! lo.isChecked;
			setValue(SquareDiamond.InitMode.INIT_LO);
		}
		private void zero_eventClick(UIComponent c, UIMouseEventParameter e) {
			none();
			zero.isChecked =! zero.isChecked;
			setValue(SquareDiamond.InitMode.INIT_ZERO);
		}
		private void random_eventClick(UIComponent c, UIMouseEventParameter e) {
			none();
			random.isChecked =! random.isChecked;
			setValue(SquareDiamond.InitMode.INIT_RANDOM);
		}
	}

	public class TerrainUIInitPanel : UIPanel {
		private UICustomCheckbox useforResources;

		public override void Start() {
			float y = 0.0f;
			float sh = 28.0f;

			UILabel lab1 = this.AddUIComponent<UILabel>();
			lab1.relativePosition = new Vector3(105, y);
			lab1.text = "Mntn";
			lab1.textScale = 0.6f;

			UILabel lab2 = this.AddUIComponent<UILabel>();
			lab2.relativePosition = new Vector3(135, y);
			lab2.text = "Land";
			lab2.textScale = 0.6f;

			UILabel lab3 = this.AddUIComponent<UILabel>();
			lab3.relativePosition = new Vector3(165, y);
			lab3.text = "Sea";
			lab3.textScale = 0.6f;

			UILabel lab4 = this.AddUIComponent<UILabel>();
			lab4.relativePosition = new Vector3(185, y);
			lab4.text = "Random";
			lab4.textScale = 0.6f;

			y+=sh;

			new UITerrainInitConfig(this, "Corners", y, value => { TerraGen.tg.InitNorthWest = value; }); y+=sh;
			new UITerrainInitConfig(this, "Top", y, value => { TerraGen.tg.InitNorth = value; }); y+=sh;
			new UITerrainInitConfig(this, "Side", y, value => { TerraGen.tg.InitWest = value; }); y+=sh;
			new UITerrainInitConfig(this, "Center", y, value => { TerraGen.tg.InitCenter = value; }); y+=sh;

			useforResources = TerrainUI.MakeCheckBox(this, "Use for resources too.", y, useforResources_eventClick, "Use this settings for resources too. Otherwise resources are always randomized.");
			useforResources.isChecked = false;
		}

		private void useforResources_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
			useforResources.isChecked =! useforResources.isChecked;
			TerraGen.tg.RandomResourcesInit = !useforResources.isChecked;
		}
	}

	public class TerrainUIMainPanel : UIPanel {
		private UILabel title;
		private UIDragHandle dragHandle;
		private UITabContainer tabContainer;
		private TerrainUITerrainPanel terrainPanel;
		private TerrainUIResourcePanel resPanel;
		private TerrainUITreePanel treePanel;
		private TerrainUIInitPanel initPanel;
		private UITabstrip tabStrip;
		private UIButton closeButton;

		private List<UIButton> tabs;

		public override void Awake() {
			title = AddUIComponent<UILabel>();
			dragHandle = AddUIComponent<UIDragHandle>();
			tabStrip = AddUIComponent<UITabstrip>();
			tabContainer = AddUIComponent<UITabContainer>();
			closeButton = AddUIComponent<UIButton>();

			initPanel = tabContainer.AddUIComponent<TerrainUIInitPanel>();
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
			Destroy (initPanel);
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
			tabStrip.startSelectedIndex = 1;
			tabStrip.selectedIndex = -1;

			tabs = new List<UIButton>();
			tabs.Add (TerrainUI.MakeTab (tabStrip, "Init", 34, initPanel, baseTabButton_eventClick));
			tabs.Add (TerrainUI.MakeTab (tabStrip, "Terrain", 54, terrainPanel, baseTabButton_eventClick));
			tabs.Add (TerrainUI.MakeTab (tabStrip, "Resources", 74, resPanel, baseTabButton_eventClick));
			tabs.Add (TerrainUI.MakeTab (tabStrip, "Trees", 54, treePanel, baseTabButton_eventClick));

			//terrainPanel.Hide ();
			resPanel.Hide ();
			treePanel.Hide ();
			initPanel.Hide ();
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