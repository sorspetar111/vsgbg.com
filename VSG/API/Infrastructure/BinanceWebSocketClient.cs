// BinanceWebSocketClient.cs
using System;
using WebSocketSharp;

public class BinanceWebSocketClient
{
    private readonly string[] _symbols = { "btcusdt", "adausdt", "ethusdt" };
    private readonly Dictionary<string, Action<string, Price>> _subscribers = new Dictionary<string, Action<string, Price>>();
   
    private WebSocket _webSocket;

    public BinanceWebSocketClient()
    {
        var streams = string.Join("/", _symbols) + "@ticker";
        var url = $"wss://stream.binance.com:9443/stream?streams={streams}";

        _webSocket = new WebSocket(url);
        _webSocket.OnMessage += (sender, e) =>
        {
            OnMessage(e.Data);
        };
        _webSocket.OnError += (sender, e) =>
        {
            Console.WriteLine($"Error: {e.Message}");
        };
        _webSocket.OnClose += (sender, e) =>
        {
            Console.WriteLine("Connection closed");
        };
    }

    public void Start()
    {
        _webSocket.Connect();
    }

    public void Stop()
    {
        _webSocket.Close();
    }

    public void SubscribeToAvgPriceUpdates(string symbol, Action<string, Price> callback)
    {
        _subscribers[symbol.ToLower()] = callback;
    }

    private void OnMessage(string data)
    {
        // Console.WriteLine(data);

        var priceData = JsonConvert.DeserializeObject<PriceData>(data);

        if (_subscribers.TryGetValue(priceData.Symbol.ToLower(), out var callback))
        {
            callback?.Invoke(priceData.Symbol, priceData);
        }


    }

    public void Disconnect()
    {
        _webSocket.Dispose();
    }
}
