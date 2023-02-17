using Error418_GartenMassaker;
using SocketIOClient;
using System.Text.Json;

SocketIO socket = new("https://games.uhno.de", new SocketIOOptions {
    Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
});
string secret = "b88f0722-18c7-4a92-9d61-4f1228a7bc39";

int playerIndex = 0;

socket.OnConnected += async (sender, e) => {
    await socket.EmitAsync("authenticate", secret);
    Console.WriteLine("Bot connected to Socket\n");
};

await socket.ConnectAsync();
socket.OnDisconnected += (sender, e) => {
    Console.WriteLine("Disconnected" + sender.ToString() + e.ToString());
};

socket.On("disconnect", data => {
    Console.WriteLine(data);
});

socket.On("data", async response => {
    Root test = response.GetValue<Root>();
    Console.WriteLine(response);
    switch (test.type) {
        case "INIT":
            Initialize(test);
            break;
        case "RESULT":
            OnResulting(test);
            break;
        case "SET":
            await SetBoard(test);
            break;
        case "ROUND":
            await OnRound(test);
            break;
    }

});

void Initialize(Root data) {
    Console.WriteLine(data.type);
    playerIndex = data.players[0].id == data.self ? 0 : 1;
}
void OnResulting(Root data) {
    Console.WriteLine(data.type);
}
async Task SetBoard(Root data) {
    Console.WriteLine(data.type);
    Furniture[] allFurnitures = new Furniture[5] {
        new Furniture { start = new int[] { 4, 3 }, direction = 'h', size = 5 },
        new Furniture { start = new int[] { 8, 6 }, direction = 'v', size = 4 },
        new Furniture { start = new int[] { 1, 5 }, direction = 'v', size = 3 },
        new Furniture { start = new int[] { 3, 5 }, direction = 'v', size = 3 },
        new Furniture { start = new int[] { 5,5 }, direction = 'v', size = 2 }
    };
    var packet = JsonSerializer.Serialize(allFurnitures);
    await socket.EmitAsync("", packet);
}
async Task OnRound(Root data) {
    Console.WriteLine(data.type);
}

Console.ReadLine();

