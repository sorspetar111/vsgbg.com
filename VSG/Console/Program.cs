


// You have service!!!  Executed each day at 10:00 AM
// Also with Fetch you can subscribe to socket wss and fetch prices and save it into db context. I Implemeted Price model.
// Then with controller you have two endpoints functions to get all the information.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a command.");
            return;
        }

        string command = args[0];
        string symbol = args.Length > 1 ? args[1] : "";
        string n = args.Length > 2 ? args[2] : "";
        string p = args.Length > 3 ? args[3] : "";
        string s = args.Length > 4 ? args[4] : "";

        var serviceProvider = new ServiceCollection()
            .AddHttpClient()
            .BuildServiceProvider();

        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        var client = httpClientFactory.CreateClient();

        if (command == "24h")
        {
            await Get24hAvgPrice(client, symbol);
        }
        else if (command == "sma")
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Invalid number of arguments for 'sma' command.");
                return;
            }
            await GetSimpleMovingAverage(client, symbol, n, p, s);
        }
        else
        {
            Console.WriteLine("Invalid command.");
        }
    }

    static async Task Get24hAvgPrice(HttpClient client, string symbol)
    {
        string url = $"https://localhost:5001/api/{symbol}/24hAvgPrice";
        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"24h average price for {symbol}: {result}");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
    }

    static async Task GetSimpleMovingAverage(HttpClient client, string symbol, string n, string p, string s)
    {
        string url = $"https://localhost:5001/api/{symbol}/SimpleMovingAverage?n={n}&p={p}&s={s}";
        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Simple moving average for {symbol} (n={n}, p={p}, s={s}): {result}");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
    }
}

