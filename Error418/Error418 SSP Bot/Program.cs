using SocketIOClient;
using System.Net.Sockets;

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
});


Console.ReadLine();