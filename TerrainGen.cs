#if !(UNITY_4 || UNITY_5)
using System;
using System.Collections;
using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace TerrainGen {
	public class TerraGen {
		private static TerrainGen i;

		private TerraGen() {}

		public static TerrainGen tg {	get {
			if (i==null)
				i = new TerrainGen();
			return i;
		}}
	}

	public class TerrainGen : ThreadingExtensionBase {
		private System.Random r;
		private SquareDiamond sdTerrain;
		private SquareDiamond sdResources;

		public SquareDiamond.InitMode InitNorth = SquareDiamond.InitMode.INIT_RANDOM;
		public SquareDiamond.InitMode InitWest = SquareDiamond.InitMode.INIT_RANDOM;
		public SquareDiamond.InitMode InitNorthWest = SquareDiamond.InitMode.INIT_RANDOM;
		public SquareDiamond.InitMode InitCenter = SquareDiamond.InitMode.INIT_RANDOM;
		public bool RandomResourcesInit = true;

		private double frand() {
			if (r==null)
				r = new System.Random();
			return (r.NextDouble () * 2.0) - 1.0;
		}

		public void FlattenTerrain() {
			byte[] map = new byte[1081*1081*2];
			for (int i=0; i<1081*1081*2; i+=2) {
				map[i] = 0;
				map[i+1] = 15;
			}
			SimulationManager.instance.AddAction(LoadHeightMap(map));
		}

		public void DoTerrain(int smoothness, float scale, float offset, int blur) {
			if ( sdTerrain == null )
				sdTerrain = new SquareDiamond(1024, new System.Random());

			sdTerrain.Generate(smoothness, (double)scale, InitNorthWest, InitNorth, InitWest, InitCenter);
			if ( blur >= 3 )
				sdTerrain.Blur(blur);

			int[] hmap = new int[1081*1081];
			for (int y=0; y<1081; y++) {
				for(int x=0; x<1081; x++) {
					// Offset 28 px 1081-1024/2
					float pt = (float)sdTerrain.Point(x-28,y-28);

					int pos = x+y*1081;
					int val = (int)((pt+1.0+(double)offset)*32768);
					val = Mathf.Clamp (val, 0, 65535);
					hmap[pos] = val;
				}
			}

			// doRiver();

			byte[] map = new byte[1081*1081*2];
			for (int y=0; y<1081; y++) {
				for(int x=0; x<1081; x++) {
					int pos = x+y*1081;
					byte[] bytes = BitConverter.GetBytes(hmap[pos]);
					if (!BitConverter.IsLittleEndian)
						Array.Reverse(bytes);
					map[pos*2] = bytes[0];
					map[1+pos*2] = bytes[1];
				}
			}

			SimulationManager.instance.AddAction(LoadHeightMap(map));
		}


		public void DoResources(int smoothness, float scale, float offset, int blur, float forestlvl, float orelvl) {
			if ( sdResources == null)
				sdResources = new SquareDiamond(512, new System.Random());

			if ( RandomResourcesInit == true ) {
				sdResources.Generate(smoothness, (double)scale,
					SquareDiamond.InitMode.INIT_RANDOM, SquareDiamond.InitMode.INIT_RANDOM,
					SquareDiamond.InitMode.INIT_RANDOM, SquareDiamond.InitMode.INIT_RANDOM);
			} else {
				sdResources.Generate(smoothness, (double)scale, InitNorthWest, InitNorth, InitWest, InitCenter);
			}

			NaturalResourceManager m = NaturalResourceManager.instance;

			// forest level from 0.0 to 1.0
			// in reality max 0.7 (and 3 * 0.1 for other three natural resources makes 1).
			orelvl *= 3;
			forestlvl *= 0.7f;
			float gap = (1.0f - forestlvl ) / 3;
			float oregap = gap * orelvl;
			forestlvl /= 2;

			for (int y=0; y<512; y++) {
				for (int x=0; x<512; x++) {
					int pos = x * 512 + y;
					double pt = sdResources.Point(x, y) + 1.0 / 2.0;

					m.m_naturalResources[pos].m_oil = 0;
					m.m_naturalResources[pos].m_ore = 0;
					m.m_naturalResources[pos].m_fertility = 0;
					m.m_naturalResources[pos].m_forest = 0;
					m.m_naturalResources[pos].m_tree = 0;

					if ( pt < gap ) {
						m.m_naturalResources[pos].m_fertility = 255;
					} else if ( pt > gap+forestlvl && pt < gap+forestlvl+oregap ) {
						m.m_naturalResources[pos].m_ore = 255;
					} else if ( pt > gap+forestlvl+oregap+forestlvl  ) {
						m.m_naturalResources[pos].m_oil = 255;
					} else {
						m.m_naturalResources[pos].m_forest = 255;
						m.m_naturalResources[pos].m_tree = 120; // ??
					}
					m.m_naturalResources[pos].m_modified = true;
				}
			}
		}

		public void DoTrees(bool follow, int treeNum, bool inside25, int density) {
			SquareDiamond sd;

			if (follow) {
				sd = sdResources;
			} else {
				sd = new SquareDiamond(512, new System.Random());
				sd.Generate(9, 1.0, SquareDiamond.InitMode.INIT_RANDOM, SquareDiamond.InitMode.INIT_RANDOM, SquareDiamond.InitMode.INIT_RANDOM, SquareDiamond.InitMode.INIT_RANDOM);
				sd.Blur(3);
			}

			TreeInfo tree;
			NaturalResourceManager m = NaturalResourceManager.instance;
			TreeCollection tc = GameObject.FindObjectOfType<TreeCollection>();
			int numTrees = tc.m_prefabs.Length;

			// TODO: Do something with TerrainManager.instance.GetShorePos() and HasWater()

			int start = 0;
			int end = 512;
			if (inside25 == true) {
				// Create trees only inside center 25 tiles.
				start = (512/9)*2;
				end = 512-(512/9)*2;
			}

			int matches = 0;
			for (int y=start; y<end; y++) {
				for (int x=start; x<end; x++) {
					int pos = x * 512 + y;
					double pt = sd.Point(x, y);
					if ( follow ? m.m_naturalResources[pos].m_forest == 255 : (pt > 0.0 && pt < 0.5) ) {
						matches++;
					}
				}
			}

			int max = TreeManager.MAX_MAP_TREES/matches > density ? density : TreeManager.MAX_MAP_TREES/matches;

			for (int y=start; y<end; y++) {
				for (int x=start; x<end; x++) {
					int pos = x * 512 + y;
					double pt = sd.Point(x, y);
					if ( follow ? m.m_naturalResources[pos].m_forest == 255 : (pt > 0.0 && pt < 0.5) ) {
						for (int n=0; n<max; n++ ) {
							float xscale = 33.75f; // 540/16=33.75
							float center = 270*32;
							int newx = (int)(((float)x*xscale) - center + (frand()*xscale));
							int newy = (int)(((float)y*xscale) - center + (frand()*xscale));

							if (treeNum == 0) {
								tree = tc.m_prefabs[r.Next()&numTrees-1];
							} else {
								tree = tc.m_prefabs[treeNum-1];
							}
							SimulationManager.instance.AddAction( AddTree(newy, newx, SimulationManager.instance.m_randomizer, tree) );
						}
					}
				}
			}
		}

		public void DeleteAllTrees() {
			int r = TreeManager.TREEGRID_RESOLUTION; // 540
			TreeManager tm = TreeManager.instance;

			if ( tm.m_treeCount == 0 )
				return;

			uint tot = 0;
			for (int i=0; i<r*r; i++) {
				uint id = tm.m_treeGrid[i];
				if (id != 0) {
					while (id != 0 && tot++ < TreeManager.MAX_MAP_TREES) {
						uint next = tm.m_trees.m_buffer[id].m_nextGridTree;
						SimulationManager.instance.AddAction( DelTree(id) );
						id = next;
					};
				}
			}
		}

		private IEnumerator DelTree(uint id) {
			TreeManager.instance.ReleaseTree(id);
			yield return null;
		}
		private IEnumerator AddTree(int x, int y, ColossalFramework.Math.Randomizer rr, TreeInfo tree) {
			uint treeNum;
			TreeManager.instance.CreateTree(out treeNum, ref rr, tree, new Vector3(x, 0, y), false);
			yield return null;
		}

		private IEnumerator LoadHeightMap(byte[] map) {
			Singleton<TerrainManager>.instance.SetRawHeightMap(map);
			yield return null;
		}
	}
}
#endif
