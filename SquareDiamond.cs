//
// Square Diamond Algorithm
// C# Implementation: http://www.bluh.org/code-the-diamond-square-algorithm/
// And excellent explanation of the algorithm in js:
// http://www.playfuljs.com/realistic-terrain-in-130-lines/
//
using System;
using System.Collections;

namespace TerrainGen {
	public class SquareDiamond {
		private int width;
		private int height;
		private double[] values;
		private System.Random r;

		public enum InitMode { INIT_LO = 1, INIT_ZERO, INIT_HI, INIT_RANDOM };

		public SquareDiamond(int size) {
			width = size;
			height = size;
			r = new System.Random();
		}

		~SquareDiamond() {
			if (values != null) {
				values = null;
			}
		}

		private double getPoint(int x, int y) {
			return values[ (x&(width-1)) + ( (y&(height-1)) * width ) ];
		}

		public double Point(int x, int y) {
			return getPoint (x, y);
		}

		private void setPoint(int x, int y, double val) {
			values[(x&(width-1)) + ((y&(height-1)) * width)] = val;
		}

		public void Generate(int fs, double scale, InitMode iNorthWest, InitMode iNorth, InitMode iWest, InitMode iCenter) {
			if ( values != null ) {
				values = null;
			}
			values = new double[width*height];

			if (iNorthWest == InitMode.INIT_RANDOM && iNorth == InitMode.INIT_RANDOM && iWest == InitMode.INIT_RANDOM && iCenter == InitMode.INIT_RANDOM ) {
				// Thoroughly initialize random grid.
				for( int y = 0; y < height; y += 8) {
					for (int x = 0; x < width; x += 8) {
						setPoint(x, y, frand());
					}
				}
			} else {
				// Initialize with user config.
				double n;
				n = iNorthWest == InitMode.INIT_RANDOM ? frand() : (float)iNorthWest - 2.0;
				setPoint (0, 0, n);
				n = iNorth == InitMode.INIT_RANDOM ? frand() : (float)iNorth - 2.0;
				setPoint (width/2, 0, n);
				n = iWest == InitMode.INIT_RANDOM ? frand() : (float)iWest - 2.0;
				setPoint (0, height/2, n);
				n = iCenter == InitMode.INIT_RANDOM ? frand() : (float)iCenter - 2.0;
				setPoint (width/2, height/2, n);
			}

			int samples = fs;
			while(samples > 0) {
				divide (samples, scale);
				samples /= 2;
				scale /= 2.0;
			}
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
						for ( int y0 = y-size; y0 <= y+size; y0++) {
							total += getPoint(x0, y0);
							count++;
						}
					}

					if ( count > 0 )
						setPoint(x, y, total / (double)count);
				}
			}
		}

		public void Blur(int size) {
			boxSmooth(size);
		}
	}
}
