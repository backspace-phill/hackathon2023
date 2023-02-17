// See https://aka.ms/new-console-template for more information
using Websocket.Client;
var exitEvent = new ManualResetEvent(false);
var secret = "177a7257-e8d9-4f9a-a5d6-1e6cc43743a4";
var url = new Uri("wss://games.uhno.de");

using (var client = new WebsocketClient(url)) {
    Console.WriteLine(client.IsRunning);

    client.ReconnectTimeout = TimeSpan.FromSeconds(2);
    client.ReconnectionHappened.Subscribe(info =>
        Console.WriteLine($"Reconnection happened, type: {info.Type}"));

    client.MessageReceived.Subscribe(msg => Console.WriteLine($"Message received: {msg}"));
    client.Start();

    Task.Run(() => client.Send("{ message }"));

    exitEvent.WaitOne();
}