using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Error418_GartenMassaker
{
	// Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
	public class Player
	{
		public string id { get; set; }
		public int score { get; set; }
	}

	public class Root
	{
		public string id { get; set; }
		public List<Player> players { get; set; }
		public List<object> boards { get; set; }
		public List<object> log { get; set; }
		public string type { get; set; }
		public string self { get; set; }
	}
	public class Furniture
	{
		[JsonPropertyName("start")]
		public int[] start { get; set; } = new int[2];
		[JsonPropertyName("direction")]
		public char Direction { get; set; }
		[JsonPropertyName("size")]
		public int Size { get; set; }
	}
}
