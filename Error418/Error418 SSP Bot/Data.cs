using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Error418_SSP_Bot
{
	internal class Data
	{
		public class Player
		{
			public string id { get; set; }
			public int score { get; set; }
		}

		public class Log
		{
			public int round { get; set; }
			public List<string> results { get; set; }
			public List<int> rating { get; set; }
		}

		public class SSP
		{
			public string id { get; set; }
			public List<Player> players { get; set; }
			public int? round { get; set; }
			public List<Log> log { get; set; }
			public string type { get; set; }
			public string self { get; set; }
		}
	}
}
