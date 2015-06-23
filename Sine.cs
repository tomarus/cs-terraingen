using UnityEngine; // for vector2
using System;
using System.Collections;

namespace TerrainGen {
	public class Sine : Algorithm {
		public Sine(int n) : base(n) {}

		public void Generate(double frequency, double scale) {
			if ( values != null ) {
				values = null;
			}
			values = new double[width*height];

			for (int x=0; x<width; x++) {
				for (int y=0; y<height; y++) {
					float nx = (Mathf.PI/(width/2))*((width/2)-x);
					float ny = (Mathf.PI/(height/2))*((height/2)-y);
					float sin = Mathf.Sin( Mathf.Sqrt( (nx*nx) + (ny*ny) ) * (float)frequency );
					setPoint(x, y, ((sin / 2) + 0.5) * scale);
				}
			}
		}
	}
}
