using Error418_GartenMassaker;
using SocketIOClient;
using System.Text.Json;
using System.Drawing;

SocketIO socket = new("https://games.uhno.de", new SocketIOOptions
{
	Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
});
string secret = "01bcf9dc-292c-4554-9ed2-00a65bd75553";

List<Game> games = new();

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
	Game newGame = new(data);
	games.Add(newGame);
}
void OnResulting(Root data)
{
	Console.WriteLine(data.type);
	var finishedGame = games.Find(game => game.GameId == data.id);
	finishedGame.HasWon(data);
	games.Remove(finishedGame);
}
async Task SetBoard(Root data, SocketIOResponse response)
{
	Console.WriteLine(data.type);
	await games.Find(game => game.GameId == data.id).SetBoard(response);
}
async Task OnRound(Root data, SocketIOResponse response)
{
	Console.WriteLine(data.type);
	var currentGame = games.Find(game => game.GameId == data.id);
	currentGame.Boards = data.boards;
	await currentGame.Attack(response);
}

Console.ReadLine();
