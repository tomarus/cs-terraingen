using UnityEngine;

public class HeightmapPass {
	public int Type; // 0 = off, 1 = sd, 2 = FIXME to algo class or enum

	private float weight;
	public float Weight { get { return Type == 0 ? 0.0f : weight; } set { weight = value; } }

	// Square Diamond
	private TerrainGen.SquareDiamond sd;
	public float Smoothness;
	public float Scale;
	public float Offset;
	public float Blur;
	// Sine
	private TerrainGen.Sine si;
	public float SIScale;
	public float SIFrequency;
	// PerlinNoise
	private TerrainGen.PerlinNoise pn;
	public float PNScale;
	public float PNFreq;
	public float PNOct;

	public void Generate(System.Random r) {
		switch(Type) {
			case 0: // OFF
				break;
			case 1: // Square Diamond
				sd = new TerrainGen.SquareDiamond(512, r);
				int blur = (int)Mathf.Pow(2.0f, Blur) + 1;
				int smooth = (int)Mathf.Pow(2.0f, Smoothness);
				sd.GenerateRandom(smooth, (double)Scale);
				//(256, 1.0f, SquareDiamond.InitMode.INIT_RANDOM, SquareDiamond.InitMode.INIT_RANDOM, SquareDiamond.InitMode.INIT_RANDOM, SquareDiamond.InitMode.INIT_RANDOM);
				if (Blur>0)
					sd.Blur(blur);
				sd.Normalize();
				Debug.Log("Generated Square Diamond Heightmap. Smoothness: " + smooth + " Scale: " + Scale + " Blur: " + blur);
				break;
			case 2: // Perlin Noise
				pn = new TerrainGen.PerlinNoise(512);
				pn.Generate(PNFreq, PNScale, (int)PNOct);
				break;
			case 3: // Sine
				si = new TerrainGen.Sine(512);
				si.Generate(SIFrequency, SIScale);
				break;
		}
	}

	public double Point(int x, int y) {
		double p = 0.0f;
		switch(Type) {
			case 0:
				p = 0.0f;
				break;
			case 1:
				p = sd.Point(x, y);
				break;
			case 2:
				p = pn.Point(x, y);
				break;
			case 3:
				p = si.Point(x, y);
				break;
		}
		return p;
	}
}

public class Heightmap {
	public HeightmapPass[] pass;
	public System.Random random;
	public int seed;
	public int size = 512;

	public Heightmap() {
		pass = new HeightmapPass[4];
		for (int i=0; i<4; i++ ) {
			pass[i] = new HeightmapPass();
		}
		random = new System.Random(seed);
	}

	public void Seed(bool useRandom, int seed) {
		if (useRandom)
			random = new System.Random((int)System.DateTime.Now.Ticks);
		else
			random = new System.Random(seed);
	}

	public void Generate() {
		for (int i=0; i<4; i++) {
			if ( pass[i] != null ) {
				pass[i].Generate(random);
			}
		}
	}

	public double Point(int x, int y) {
		return (
			(pass[0].Point(x, y) * pass[0].Weight) +
			(pass[1].Point(x, y) * pass[1].Weight) +
			(pass[2].Point(x, y) * pass[2].Weight) +
			(pass[3].Point(x, y) * pass[3].Weight)
		       ) / (pass[0].Weight + pass[1].Weight + pass[2].Weight + pass[3].Weight);
	}

	private Texture2D texture;
	public Texture2D CreateTexture() {
		texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
		return texture;
	}

	public void UpdateTexture() {
		for (int x=0; x<size; x++) {
			for (int y=0; y<size; y++) {
				float p = (float)Point(x,y);
				texture.SetPixel(x, y, new Color(p,p,p,1.0f));
			}
		}
		texture.Apply();
	}
}
