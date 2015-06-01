//
// UI Stuff mostly taken from:
// https://github.com/lxteo/Cities-Skylines-Mapper
// https://github.com/AlexanderDzhoganov/Skylines-FPSCamera
//
using System;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace TerrainGen {
	public class TerrainPanel : UIPanel {
		UILabel title;
		UIButton okButton;
		UIButton cancelButton;
		
		float smoothness = 8;
		float scale = 0.75f;
		float offset = 0.0f;
		float blur = 1.0f;
		
		public override void Awake() {
			title = AddUIComponent<UILabel>();
			okButton = AddUIComponent<UIButton>();
			cancelButton = AddUIComponent<UIButton>();
		}
		
		private void SetButton(UIButton okButton, string p1,int x, int y){
			okButton.text = p1;
			okButton.normalBgSprite = "ButtonMenu";
			okButton.hoveredBgSprite = "ButtonMenuHovered";
			okButton.disabledBgSprite = "ButtonMenuDisabled";
			okButton.focusedBgSprite = "ButtonMenuFocused";
			okButton.pressedBgSprite = "ButtonMenuPressed";
			okButton.size = new Vector2(90, 32);
			okButton.relativePosition = new Vector3(x, y - 3);
			okButton.textScale = 0.9f;
			okButton.hoveredTextColor = new Color(1,1,.45f,1);
		}
		
		public override void Start () {
			this.backgroundSprite = "MenuPanel";
			this.width = 240;
			this.height = 290;
			
			title.text = "Terrain Generator";
			title.relativePosition = new Vector3(15, 15);
			title.textScale = 0.9f;
			title.size = new Vector2(200, 30);
			
			SetButton(okButton, "Generate", 20, 250);
			okButton.eventClick += okButton_eventClick;
			
			SetButton(cancelButton, "Cancel", 130, 250);
			cancelButton.eventClick += cancelButton_eventClick;
			
			MakeSlider ("SmoothSlider", "Smoothness", 50, 9.0f, 0.0f, 10.0f, 1.0f,
			            value => {
				smoothness = value;
			});
			
			MakeSlider ("ScaleSlider", "Scale", 100, scale, 0.0f, 1.0f, 0.05f,
			            value => {
				scale = value;
			});

			MakeSlider ("OffsetSlider", "Offset", 150, offset, -1.0f, 1.0f, 0.05f,
			            value => {
				offset = value;
			});
			
			MakeSlider ("BlurSlider", "Blur", 200, blur, 0.0f, 2.0f, 1.00f,
			            value => {
				blur = value;
			});
		}
		
		private void okButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
			TerrainGen tg = new TerrainGen();
			tg.Do ((int)Mathf.Pow (2.0f, smoothness), scale, offset, (int)Mathf.Pow (2.0f, blur) + 1);
		}
		
		private void cancelButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
			this.Hide ();
		}
		
		private delegate void SliderSetValue(float value);
		
		private UISlider MakeSlider(string name, string text, float y, float value, float min, float max, float step, SliderSetValue setValue) {
			var label = AddUIComponent<UILabel>();
			label.name = name + "Label";
			label.text = text;
			label.relativePosition = new Vector3(15.0f, y);
			label.textScale = 0.8f;
			
			var slider = AddUIComponent<UISlider>();
			slider.name = name + "Slider";
			slider.minValue = min;
			slider.maxValue = max;
			slider.stepSize = step;
			slider.value = value;
			slider.relativePosition = new Vector3(15.0f, y+16);
			slider.size = new Vector2(170.0f, 16.0f);
			
			var thumbSprite = slider.AddUIComponent<UISprite>();
			thumbSprite.name = "ScrollbarThumb";
			thumbSprite.spriteName = "ScrollbarThumb";
			thumbSprite.Show();
			
			slider.backgroundSprite = "ScrollbarTrack";
			slider.thumbObject = thumbSprite;
			slider.orientation = UIOrientation.Horizontal;
			slider.isVisible = true;
			slider.enabled = true;
			slider.canFocus = true;
			slider.isInteractive = true;
			
			var valueLabel = AddUIComponent<UILabel>();
			valueLabel.name = name + "ValueLabel";
			valueLabel.text = slider.value.ToString("0.00");
			valueLabel.relativePosition = new Vector3(200.0f, y+16);
			valueLabel.textScale = 0.8f;
			
			slider.eventValueChanged += (component, f) =>
			{
				setValue(f);
				valueLabel.text = slider.value.ToString("0.00");
			};
			
			return slider;
		}
		
		public override void Update() {
			if (Input.GetKeyDown (KeyCode.Escape) && this.isVisible) {
				this.Hide ();
			}
		}	
	}
} 