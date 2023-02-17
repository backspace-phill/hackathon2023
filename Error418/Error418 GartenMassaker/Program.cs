using Error418_GartenMassaker;
using SocketIOClient;
using System.Text.Json;
using System.Text.Json.Nodes;
using SocketIOClient.JsonSerializer;

SocketIO socket = new("https://games.uhno.de", new SocketIOOptions
{
	Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
});
string secret = "01bcf9dc-292c-4554-9ed2-00a65bd75553";

int playerIndex = 0;

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
	Console.WriteLine(response);
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
			await OnRound(test);
			break;
	}

});

void Initialize(Root data)
{
	Console.WriteLine(data.type);
	playerIndex = data.players[0].id == data.self ? 0 : 1;
}
void OnResulting(Root data)
{
	Console.WriteLine(data.type);
}
async Task SetBoard(Root data, SocketIOResponse response)
{
	Console.WriteLine(data.type);
	Furniture[] allFurnitures = new Furniture[5] {
		new Furniture { start = new int[] { 4, 3 }, Direction = 'h', Size = 5 },
		new Furniture { start = new int[] { 8, 6 }, Direction = 'v', Size = 4 },
		new Furniture { start = new int[] { 1, 5 }, Direction = 'v', Size = 3 },
		new Furniture { start = new int[] { 3, 5 }, Direction = 'v', Size = 3 },
		new Furniture { start = new int[] { 5,5 }, Direction = 'v', Size = 2 }
	};

	await response.CallbackAsync(allFurnitures.ToList());
}
async Task OnRound(Root data)
{
	Console.WriteLine(data.type);
}

Console.ReadLine();
