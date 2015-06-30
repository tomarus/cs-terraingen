using UnityEngine;
namespace TerrainGen {
public class Algorithm  {
	protected int width;
	protected int height;
	protected double[] values;

	public Algorithm(int size) {
		width = size;
		height = size;
	}

	~Algorithm() {
		if (values != null) {
			values = null;
		}
	}

	protected double getPoint(int x, int y) {
		return values[ (x&(width-1)) + ( (y&(height-1)) * width ) ];
	}

	public double Point(int x, int y) {
		return getPoint (x, y);
	}
	public double Point(Vector2 pos) {
		return getPoint ((int)pos.x, (int)pos.y);
	}

	protected void setPoint(int x, int y, double val) {
		values[(x&(width-1)) + ((y&(height-1)) * width)] = val;
	}

	public void Set(Vector2 pos, double val) {
		setPoint((int)pos.x, (int)pos.y, val);
	}
}
}
