using SocketIOClient;
using System.Text.Json;

SocketIO socket = new("https://games.uhno.de", new SocketIOOptions
{
	Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
});
string secret = "01bcf9dc-292c-4554-9ed2-00a65bd75553";

socket.OnConnected += async (sender, e) =>
{
	await socket.EmitAsync("authenticate", secret);
	Console.WriteLine("Bot connected to Socket");
};

await socket.ConnectAsync();

socket.On("data", async response =>
{
	var test = response.GetValue<Error418_GartenMassaker.Root>();
	Console.WriteLine(test.type + "\n" + test.boards);

});

Console.ReadLine();