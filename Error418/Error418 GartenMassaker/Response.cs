using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Error418_GartenMassaker {
    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class Player {
        public string id { get; set; }
        public int score { get; set; }
    }

    public class Root {
        public string id { get; set; }
        public List<Player> players { get; set; }
        public List<object> boards { get; set; }
        public List<object> log { get; set; }
        public string type { get; set; }
        public string self { get; set; }
    }
    public class Furniture {
        public int[] start { get; set; } = new int[2];
        public char direction { get; set; }
        public int size { get; set; }
    }
}
