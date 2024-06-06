public class BinanceService : BackgroundService
{
    private readonly BinanceWebSocketClient _client;
    private readonly PriceController _priceController;  
    private Timer _timer;

    public BinanceService(BinanceWebSocketClient client, PriceController priceController)
    {
        _client = client;
        _priceController = priceController;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ScheduleTask();
        await Task.CompletedTask;
    }

    private void ScheduleTask()
    {
        var now = DateTime.Now;
        var nextRunTime = now.Date.AddDays(1).AddHours(10);
        if (now.Hour < 10)
        {
            nextRunTime = now.Date.AddHours(10);
        }
        var timeToGo = nextRunTime - now;

        if (timeToGo <= TimeSpan.Zero)
        {
            timeToGo = TimeSpan.Zero;
        }

        _timer = new Timer(StartBinanceService, null, timeToGo, TimeSpan.FromHours(24));
    }

    private void StartBinanceService(object state)
    {
        _client.Start();
        
        // Call FetchPrices method of PriceController
        _priceController.FetchPrices().GetAwaiter().GetResult();
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        _client.Stop();
        await Task.CompletedTask;
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}
