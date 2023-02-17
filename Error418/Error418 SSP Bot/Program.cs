using Error418_SSP_Bot;
using SocketIOClient;
using System.Text.Json;

SocketIO socket = new("https://games.uhno.de", new SocketIOOptions
{
	Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
});
string secret = "177a7257-e8d9-4f9a-a5d6-1e6cc43743a4";

socket.OnConnected += async (sender, e) =>
{
	await socket.EmitAsync("authenticate", secret);
};

await socket.ConnectAsync();

socket.On("data", response =>
{
	Console.WriteLine(response);
	Data.SSP test = JsonSerializer.Deserialize<Data.SSP>(response.GetValue());
});


Console.ReadLine();