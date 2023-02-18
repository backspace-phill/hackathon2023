using Error418_GartenMassaker;
using SocketIOClient;
using System.Text.Json;
using System.Drawing;
using System.Diagnostics;

SocketIO socket = new("https://games.uhno.de", new SocketIOOptions
{
	Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
});
string secret = "32a58ff6-ca53-4a39-8568-69d201c576e4";

List<Game> games = new();
int winCount = 0;
int gamesCount = 0;

socket.OnConnected += async (sender, e) =>
{
	await socket.EmitAsync("authenticate", secret);
	Console.WriteLine("Bot connected to Socket\n");
};

await socket.ConnectAsync();
socket.OnDisconnected += (sender, e) =>
{
	Console.WriteLine("Disconnected" + sender.ToString() + e.ToString());
};

socket.On("disconnect", data =>
{
	Console.WriteLine(data);
});

socket.On("data", async response =>
{
	Root test = response.GetValue<Root>();
	switch (test.type)
	{
		case "INIT":
			Initialize(test);
			break;
		case "RESULT":
			OnResulting(test);
			break;
		case "SET":
			await SetBoard(test, response);
			break;
		case "ROUND":
			await OnRound(test, response);
			break;
	}

});

void Initialize(Root data)
{
	Console.WriteLine(data.type);
	int limit = 50;
	if (games.Count < limit)
	{
		Game newGame = new(data);
		games.Add(newGame);
	}
}
void OnResulting(Root data)
{
	Console.WriteLine(data.type);
	if (!games.Any(game => game.GameId == data.id)) return;

	var finishedGame = games.Find(game => game.GameId == data.id);

	if (finishedGame.HasWon(data)) winCount++;
	gamesCount++;

	games.Remove(finishedGame);

	Console.WriteLine("Winpercentage: " + (winCount / gamesCount * 100) + "%");
	Console.WriteLine("Games: " + gamesCount);
}
async Task SetBoard(Root data, SocketIOResponse response)
{
	Console.WriteLine(data.type);
	if (!games.Any(game => game.GameId == data.id)) return;

	await games.Find(game => game.GameId == data.id).SetBoard(response);
}
async Task OnRound(Root data, SocketIOResponse response)
{
	Console.WriteLine(data.type + " : " + data.id);
	if (!games.Any(game => game.GameId == data.id)) return;

	var currentGame = games.Find(game => game.GameId == data.id);
	currentGame.Boards = data.boards;
	currentGame.WriteBoard();
	await currentGame.Attack(response);
}

Console.ReadLine();
