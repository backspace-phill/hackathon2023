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
		private readonly List<Furniture[]> availableFurnature = new List<Furniture[]> {  new Furniture[5] {
#region LongASSFurnature
			new Furniture { start = new int[] { 1, 3 }, Direction = 'v', Size = 5 },
                new Furniture { start = new int[] { 8, 6 }, Direction = 'v', Size = 4 },
                new Furniture { start = new int[] { 6, 1 }, Direction = 'h', Size = 3 },
                new Furniture { start = new int[] { 5, 7 }, Direction = 'v', Size = 3 },
                new Furniture { start = new int[] { 1, 0 }, Direction = 'h', Size = 2 }
            },
				new Furniture[5] {
                new Furniture { start = new int[] { 7, 5 }, Direction = 'v', Size = 5 },
                new Furniture { start = new int[] { 0, 1 }, Direction = 'h', Size = 4 },
                new Furniture { start = new int[] { 5, 0 }, Direction = 'v', Size = 3 },
                new Furniture { start = new int[] { 9, 7 }, Direction = 'v', Size = 3 },
                new Furniture { start = new int[] { 8, 2 }, Direction = 'h', Size = 2 }
            },
                new Furniture[5] {
                new Furniture { start = new int[] { 1, 2 }, Direction = 'h', Size = 5 },
                new Furniture { start = new int[] { 4, 5 }, Direction = 'v', Size = 4 },
                new Furniture { start = new int[] { 1, 0 }, Direction = 'h', Size = 3 },
                new Furniture { start = new int[] { 8, 3 }, Direction = 'v', Size = 3 },
                new Furniture { start = new int[] { 2, 4 }, Direction = 'v', Size = 2 }
            },
                new Furniture[5] {
                new Furniture { start = new int[] { 2, 1 }, Direction = 'h', Size = 5 },
                new Furniture { start = new int[] { 1, 3 }, Direction = 'v', Size = 4 },
                new Furniture { start = new int[] { 6, 4 }, Direction = 'h', Size = 3 },
                new Furniture { start = new int[] { 4, 5 }, Direction = 'v', Size = 3 },
                new Furniture { start = new int[] { 7, 7 }, Direction = 'v', Size = 2 }
            },
                new Furniture[5] {
                new Furniture { start = new int[] { 9, 0 }, Direction = 'v', Size = 5 },
                new Furniture { start = new int[] { 0, 9 }, Direction = 'h', Size = 4 },
                new Furniture { start = new int[] { 4, 5 }, Direction = 'h', Size = 3 },
                new Furniture { start = new int[] { 2, 0 }, Direction = 'v', Size = 3 },
                new Furniture { start = new int[] { 1, 5 }, Direction = 'h', Size = 2 }
            }
    };
        #endregion
        public GameLogicV2 GameLogic { get; set; } = new GameLogicV2();
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
			Random rnd = new Random();
			Furniture[] selectedFurnature = availableFurnature[rnd.Next(0, 4)];

			await response.CallbackAsync(selectedFurnature.ToList());

		}
		public async Task Attack(SocketIOResponse response)
		{
			var board = BoardToCharArray(this.Boards[this.EnemyIndex]);
			Point test = this.GameLogic.nextMove(board);
			await response.CallbackAsync(new int[2] { test.X, test.Y });
		}
		public bool HasWon(Root data)
		{
			if (data.players[this.PlayerIndex].score <= data.players[this.EnemyIndex].score)
			{
				Console.WriteLine("Bot has lost " + this.GameId);
				return false;
			}
			else
			{
				Console.WriteLine("Bot has won " + this.GameId);
				return true;
			}
		}
		public void WriteBoard()
		{
			var enemy = BoardToCharArray(this.Boards[this.EnemyIndex]);
			var player = BoardToCharArray(this.Boards[this.PlayerIndex]);
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					if (enemy[i, j].Equals('X'))
					{
						Console.BackgroundColor = ConsoleColor.Red;
						Console.Write(enemy[i, j] + " ");
						Console.ResetColor();
					}
					else if (enemy[i, j].Equals('x'))
					{
						Console.BackgroundColor = ConsoleColor.Magenta;
						Console.Write(enemy[i, j] + " ");
						Console.ResetColor();
					}
					else if (enemy[i, j].Equals('.'))
					{
						Console.BackgroundColor = ConsoleColor.Blue;
						Console.Write(enemy[i, j] + " ");
						Console.ResetColor();
					}
					else
					{
						Console.BackgroundColor = ConsoleColor.White;
						Console.Write(enemy[i, j] + " ");
						Console.ResetColor();
					}
				}
				Console.Write(" | ");
				for (int j = 0; j < 10; j++)
				{
					Console.Write(player[i, j] + " ");
				}
				Console.WriteLine();
			}
		}
		char[,] BoardToCharArray(object board)
		{
			JsonElement matrix = (JsonElement)board;
			char[,] currentboard = new char[10, 10];
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					string current = "";
					try {
						current = matrix[i][j].ToString();
					} catch(Exception e) {
						Console.WriteLine(e.ToString());
						current = "";
					}
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
