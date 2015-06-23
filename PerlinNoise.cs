using UnityEngine;

namespace TerrainGen {
	public class PerlinNoise : Algorithm {
		public PerlinNoise(int n) : base(n) {}

		// perlinNoise normalized between -1 and 1	
		private float perlinNoise(float x, float y) {
			return 1 - Mathf.PerlinNoise(x, y) * 2;
		}
			
		private float perlin(int x, int y, float freq, float persist, int octaves) {
			float max = 0.0f;
			float tot = 0.0f;
			float amp = 1.0f;
			
			float xx = (float)x / (float)width;
			float yy = (float)y / (float)height;
			
			for (int i=0; i<octaves; i++) {
				tot += perlinNoise(xx * freq, yy * freq) * amp;
				max += amp;
				amp *= persist;
				freq *= 2;
			}
			return tot/max;
		}
		
		public void Generate(double freq, double scale, int octaves) {
			if ( values != null ) {
				values = null;
			}
			values = new double[width*height];
			
			for (int x=0; x<width; x++) {
				for (int y=0; y<height; y++) {
					float v = perlin(x, y, (float)freq, (float)scale, octaves);
					setPoint(x, y, v);
				}
			}
		}
	}
}
