using SocketIOClient;

SocketIO socket = new("https://games.uhno.de", new SocketIOOptions
{
	Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
});
string secret = "89186412-f0c5-47bb-8064-9ab384474ed6";

socket.OnConnected += async (sender, e) =>
{
	await socket.EmitAsync("authenticate", secret);
	Console.WriteLine("Bot connected to Socket");
};

await socket.ConnectAsync();

socket.On("data", async response =>
{
	Console.WriteLine(response);
});

Console.ReadLine();