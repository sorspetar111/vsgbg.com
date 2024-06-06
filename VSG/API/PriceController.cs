using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MyApi.Data;
using Newtonsoft.Json;  
using System.Collections.Generic;

[Route("api")]
[ApiController]
[Consumes("application/xml", "application/json")] 
[Produces("application/xml", "application/json")] 
public class PriceController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private readonly BinanceWebSocketClient _binanceWebSocketClient;

    public PriceController(ApplicationDbContext dbContext, IMemoryCache cache, BinanceWebSocketClient binanceWebSocketClient)
    {
        _dbContext = dbContext;
        _cache = cache;
        _binanceWebSocketClient = binanceWebSocketClient;

        Connect();
    }

    [HttpGet("{symbol}/24hAvgPrice")]
    // [ProducesResponseType(typeof(decimal), 200)]
    public async Task<IActionResult> Get24hAvgPrice(string symbol)
    {
        // Check cache first
        if (_cache.TryGetValue($"{symbol}_24hAvgPrice", out decimal averagePrice))
        {
            return Ok(averagePrice);
        }

        // If not in cache, query the database
        var yesterday = DateTime.UtcNow.AddDays(-1);
        var prices = await _dbContext.PriceData
            .Where(p => p.Symbol == symbol && p.Timestamp >= yesterday)
            .Select(p => p.Price)
            .ToListAsync();

        if (prices.Any())
        {
            averagePrice = prices.Average();
            // Cache the result for 24 hours
            _cache.Set($"{symbol}_24hAvgPrice", averagePrice, TimeSpan.FromHours(24));
            return Ok(averagePrice);
        }
        else
        {
            return NotFound();
        }
    }

   
    /*
    Formula:
    SMA= A1 + A2 + A3 + A(n)... / n
    A(n) = the price of an asset at period n
    n = the number of total periods
 
    Example:
    Week One (5 days): 20, 22, 24, 25, 23
    Week Two (5 days): 26, 28, 26, 29, 27
    Week Three (5 days): 28, 30, 27, 29, 28
    
    A 10-day moving average would average out the closing prices for the first 10 days as the first data point. The next data point would drop the earliest price, add the price on day 11, then take the average, and so on. Likewise, a 50-day moving average would accumulate enough data to average 50 consecutive days of data on a rolling basis.
    A simple moving average is customizable because it can be calculated for different numbers of time periods. This is done by adding the closing price of the security for a number of time periods and then dividing this total by the number of time periods, which gives the average price of the security over the time period.
    A simple moving average smooths out volatility and makes it easier to view the price trend of a security. If the simple moving average points up, this means that the security's price is increasing. If it is pointing down, it means that the security's price is decreasing. The longer the time frame for the moving average, the smoother the simple moving average. A shorter-term moving average is more volatile, but its reading is closer to the source data.
    */

    [HttpGet("{symbol}/SimpleMovingAverage")]    
    // [ProducesResponseType(typeof(decimal), 200)]
    public async Task<IActionResult> GetSimpleMovingAverage(string symbol, int n, [FromQuery] TimePeriod p, DateTime? s = null)
    {
        // Validate time period (p)
        if (!IsValidTimePeriod(p))
        {
            return BadRequest("Invalid time period. Valid values: 1w, 1d, 30m, 5m, 1m");
        }

        // Get the start date
        var startDate = s ?? DateTime.UtcNow;

        // Get the end date (current date)
        var endDate = DateTime.UtcNow;

        // Calculate the interval between data points based on the time period
        var interval = p switch
        {
            TimePeriod.OneWeek => TimeSpan.FromDays(7),
            TimePeriod.OneDay => TimeSpan.FromDays(1),
            TimePeriod.ThirtyMinutes => TimeSpan.FromMinutes(30),
            TimePeriod.FiveMinutes => TimeSpan.FromMinutes(5),
            TimePeriod.OneMinute => TimeSpan.FromMinutes(1),
            _ => throw new ArgumentException("Invalid time period"),
        };

        // Query the database for price data within the specified time range
        var dataPoints = await _dbContext.Prices
            .Where(p => p.Symbol == symbol && p.Timestamp >= startDate && p.Timestamp <= endDate)
            .OrderBy(p => p.Timestamp)
            .ToListAsync();

        // Calculate the SMA
        decimal sma = 0;
        int count = 0;

        // Calculate SMA for the specified number of data points (n)
        for (int i = dataPoints.Count - 1; i >= 0 && count < n; i--)
        {
            sma += dataPoints[i].PriceValue;
            count++;
        }

        // Calculate the final SMA
        if (count > 0)
        {
            sma /= count;
        }

        return Ok(sma);
    }

    [HttpGet("fetchPrices")]
    public async Task<IActionResult> FetchPrices()
    {
        List<decimal> prices = new List<decimal>();

        // Subscribe to WebSocket messages
        _binanceWebSocketClient.SubscribeToTickerUpdates(symbols: new string[] { "btcusdt", "adausdt", "ethusdt" }, callback: async (symbol, data) =>
        {
            var tickerData = JsonConvert.DeserializeObject<BinanceTickerData>(data);
            prices.Add(tickerData.LastPrice);

            // Save price data to database
            var priceData = new PriceData
            {
                Symbol = symbol.ToUpper(),
                Price = tickerData.LastPrice,
                Timestamp = DateTime.UtcNow
            };

            // Do we need to save subscribed prices into our DB?
            await _dbContext.PriceData.AddAsync(priceData);
            await _dbContext.SaveChangesAsync();
        });

        return Ok(prices);
    }

    private bool IsValidTimePeriod(string period, bool string = true)
    {
        switch (period)
        {
            case "1w":
            case "1d":
            case "30m":
            case "5m":
            case "1m":
                return true;
            default:
                return false;
        }
    }

    private bool IsValidTimePeriod(TimePeriod period)
    {
        switch (period)
        {
            case TimePeriod.OneWeek:
            case TimePeriod.OneDay:
            case TimePeriod.ThirtyMinutes:
            case TimePeriod.FiveMinutes:
            case TimePeriod.OneMinute:
                return true;
            default:
                return false;
        }
    }

    private void Connect()
    {
        _binanceWebSocketClient.Start();
    }
    
    private void Disconnect()
    {
        _binanceWebSocketClient.Stop();
    }


}
