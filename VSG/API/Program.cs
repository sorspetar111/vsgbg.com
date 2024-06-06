using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Xml.Serialization;

var builder = WebApplication.CreateBuilder(args);

 
builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true; 
})
.AddXmlSerializerFormatters();  


builder.Services.AddSingleton<BinanceWebSocketClient>();


// Do you want automatic parse without calling API and sending any request? 
// builder.Services.AddSingleton<BinanceService>();
builder.Services.AddHostedService<BinanceService>();

// EF
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
services.AddMemoryCache();

var app = builder.Build();

// DEBUG features
//if (app.Environment.IsDevelopment())
//{
    //app.UseDeveloperExceptionPage();
//}

// SECURITY features
//app.UseHttpsRedirection();
//app.UseAuthorization();

app.MapControllers();
// app.UseSqlServerAvailabilityMiddleware();

app.Run();