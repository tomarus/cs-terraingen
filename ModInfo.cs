using ICities;

namespace TerrainGen {
	
	public class ModInfo : IUserMod {
		
		public string Name {
			get { return "Terrain Generator"; }
		}
		
		public string Description {
			get {  return "Generate random terrains using the square diamond algorithm.";}
		}
	}
}
