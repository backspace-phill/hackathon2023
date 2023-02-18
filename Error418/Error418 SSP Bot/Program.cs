using Error418_SSP_Bot;
using SocketIOClient;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text.Json;

SocketIO socket = new("https://games.uhno.de", new SocketIOOptions
{
	Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
});
string secret = "89186412-f0c5-47bb-8064-9ab384474ed6";

socket.OnConnected += async (sender, e) =>
{
	await socket.EmitAsync("authenticate", secret);
	Console.WriteLine("Bot connected");
};

await socket.ConnectAsync();

socket.On("data", async response =>
{
	Data.SSP sspDATA = JsonSerializer.Deserialize<Data.SSP>(response.GetValue());

	switch (sspDATA.type)
	{
		case "INIT":
			Initialize(sspDATA);
			break;
		case "ROUND":
			await Play(sspDATA);
			break;
		case "RESULT":
			OnResult(sspDATA);
			break;
		default:
			Console.WriteLine("No Type specified");
			break;/*throw new Exception("No type specified");*/
	}
});

void Initialize(Data.SSP data)
{
	Console.WriteLine("INIT");
}

async Task Play(Data.SSP data)
{
	string[] sspOptions = new string[3] { "STEIN", "SCHERE", "PAPIER" };
	Random random = new Random();
	int index = random.Next(0, 2);
	await socket.EmitAsync(sspOptions[index]);
	Console.WriteLine("Played " + sspOptions[index] + " in " + data.round);
}

void OnResult(Data.SSP data)
{
	int indexOfPlayer = (data.players[0].id.Equals(data.self)) ? 0 : 1;
	foreach (Data.Log current in data.log)
	{
		if (current.rating[indexOfPlayer] == 1)
			Console.WriteLine("Bot has won a round");
		else
			Console.WriteLine("Bot has lost a round");
	}
}

Console.ReadLine();