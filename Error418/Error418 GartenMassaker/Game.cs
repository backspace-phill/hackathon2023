using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Error418_GartenMassaker
{
	internal class Game
	{
		public GameLogic GameLogic { get; set; } = new GameLogic();
		public string GameId { get; set; } = string.Empty;
		public int PlayerIndex { get; set; } = 0;
		public int EnemyIndex { get; set; } = 0;
		public List<object> Boards { get; set; }
		public Game(Root data)
		{
			GameId = data.id;
			PlayerIndex = data.players[0].id == data.self ? 0 : 1;
			EnemyIndex = data.players[0].id != data.self ? 0 : 1;
			Boards = data.boards;
		}
		public async Task SetBoard(SocketIOResponse response)
		{
			Furniture[] allFurnitures = new Furniture[5] {
				new Furniture { start = new int[] { 4, 3 }, Direction = 'h', Size = 5 },
				new Furniture { start = new int[] { 8, 6 }, Direction = 'v', Size = 4 },
				new Furniture { start = new int[] { 1, 5 }, Direction = 'v', Size = 3 },
				new Furniture { start = new int[] { 3, 5 }, Direction = 'v', Size = 3 },
				new Furniture { start = new int[] { 5,5 }, Direction = 'v', Size = 2 }
			};

			await response.CallbackAsync(allFurnitures.ToList());

		}
		public async Task Attack(SocketIOResponse response)
		{
			var board = BoardToCharArray(this.Boards[this.EnemyIndex]);
			Point test = this.GameLogic.nextMove(board);
			await response.CallbackAsync(new int[2] { test.X, test.Y });
		}
		public void HasWon(Root data)
		{
			if (data.players[this.PlayerIndex].score <= data.players[this.EnemyIndex].score)
				Console.WriteLine("Bot has lost " + this.GameId);
			else
				Console.WriteLine("Bot has won " + this.GameId);
		}
		char[,] BoardToCharArray(object board)
		{
			JsonElement matrix = (JsonElement)board;
			char[,] currentboard = new char[10, 10];
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					string current = matrix[i][j].ToString();
					if (current == string.Empty)
						currentboard[i, j] = ' ';
					else
						currentboard[i, j] = current[0];
				}
			}
			return currentboard;
		}
	}
}
