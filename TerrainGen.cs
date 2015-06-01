//
// Square Diamond Algorithm
// C# Implementation: http://www.bluh.org/code-the-diamond-square-algorithm/
// And excellent explanation of the algorithm in js:
// http://www.playfuljs.com/realistic-terrain-in-130-lines/
//
using System;
using System.Collections;
using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace TerrainGen {

	public class TerrainGen : ThreadingExtensionBase {
	
		private int width = 1024;
		private int height = 1024;
		private double[] values;
		private System.Random r;
		
		private double getPoint(int x, int y) {
			return values[ (x&(width-1)) + ( (y&(height-1)) * width ) ];
		}
		
		private void setPoint(int x, int y, double val) {
			values[(x&(width-1)) + ((y&(height-1)) * width)] = val;
		}
		
		private void generate(int fs, double scale) {
			values = new double[width*height];
			r = new System.Random();
			
			// Initialize with some random points.
			for( int y = 0; y < height; y += 8) {
				for (int x = 0; x < width; x += 8) {
					setPoint(x, y, frand());
				}
			}
			
			int samples = fs;
			while(samples > 0) {
				divide (samples, scale);
				samples /= 2;
				scale /= 2.0;
			}
			
			r = null;
		}
		
		private double frand() {
			return (r.NextDouble () * 2.0) - 1.0;
		}
		
		private void square(int x, int y, int size, double val) {
			int half = size / 2;
			double a = getPoint(x-half, y-half);
			double b = getPoint(x+half, y-half);
			double c = getPoint(x-half, y+half);
			double d = getPoint(x+half, y+half);
			setPoint(x, y, ((a+b+c+d)/4.0) + val);
		}
		
		private void diamond(int x, int y, int size, double val) {
			int half = size / 2;
			double a = getPoint(x - half, y);
			double b = getPoint(x + half, y);
			double c = getPoint(x, y-half);
			double d = getPoint(x, y+half);
			setPoint(x, y, ((a+b+c+d)/4.0) + val);
		}
		
		private void divide(int step, double scale) {
			int half = step / 2;
			for (int y = half; y < height + half; y += step) {
				for (int x = half; x < width + half; x += step) {
					square (x, y, step, frand () * scale);
				}
			}
			for ( int y = 0; y < height; y += step ) {
				for ( int x = 0; x < width; x += step) {
					diamond (x + half, y, step, frand () * scale);
					diamond (x, y + half, step, frand () * scale);
				}
			}
		}
		
		private void boxSmooth(int size) {
			int count = 0;
			double total = 0;
			
			for (int x=0; x<width; x++) {
				for (int y=0; y<height; y++) {
					count = 0;
					total = 0.0;
					
					for (int x0 = x-size; x0 <= x+size; x0++) {
						//if ( x0 < 0 || x0 > width-1 )
						//	continue;
						for ( int y0 = y-size; y0 <= y+size; y0++) {
						//	if ( y0 < 0 || y0 > height -1 )
						//		continue;
							
							total += getPoint(x0, y0);
							count++;
						}
					}
					
					if ( count > 0 )
						setPoint(x, y, total / (double)count);
				}
			}
			
		}
		
		public void Do(int smoothness, float scale, float offset, int blur) {
			generate (smoothness, (double)scale);
			if ( blur >= 3 )
				boxSmooth (blur);

			byte[] map = new byte[1081*1081*2];
			for (int y=0; y<1081; y++) {
				for(int x=0; x<1081; x++) {
					// Offset 28 px 1081-1024/2
					float pt = (float)getPoint (x-28,y-28);
					//pt = Mathf.Clamp (pt, -1.0f, 1.0f);
					
					int pos = (x+y*1081) * 2;
					int val = (int)((pt+1.0+(double)offset)*32768) ;
					val = Mathf.Clamp (val, 0, 65535);
					
					byte[] bytes = BitConverter.GetBytes(val);
					if (!BitConverter.IsLittleEndian)
						Array.Reverse(bytes);
					map[pos] = bytes[0];
					map[pos+1] = bytes[1];
				}
			}
				
			values = null;
			SimulationManager.instance.AddAction(LoadHeightMap(map));
		}
		
		private IEnumerator LoadHeightMap(byte[] map) {
			Singleton<TerrainManager>.instance.SetRawHeightMap(map);
			yield return null;
		}
	}
}
