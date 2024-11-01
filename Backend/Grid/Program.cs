using Grid.Data;
using Grid.Hubs;
using Grid.Loaders;
using Grid.Services;
using Grid.Stores;
// using Hangfire;
// using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLConnectionString")));
builder.Services.AddScoped<IBitcoinStore, BitcoinStore>();

builder.Services.AddMemoryCache();
// builder.Services.AddHangfire(config =>
// {
//     config.UseMemoryStorage();
// });
// builder.Services.AddHangfireServer();
builder.Services.AddScoped<IBitcoinService, BitcoinService>();
builder.Services.AddHttpClient<CnbLoader>();
builder.Services.AddHttpClient<IBitcoinPriceLoader, GeckoLoader>();
builder.Services.AddScoped<ICurrencyLoader, CurrencyCacheLoader>();

builder.Services.AddControllers();

builder.Services.AddSignalR();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(
            allowedOrigins
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors();

app.UseAuthorization();

// app.UseHangfireDashboard();

app.MapHub<BitcoinHub>("/bitcoin");
app.MapControllers();

// Původně jsem používal Hangfire, ale říkal jsem si, že by bylo zbytečné se dotazovat, když není nikdo připojený
// RecurringJob.AddOrUpdate<BitcoinService>("broadcast-bitcoin",job => job.BroadcastBitcoinResponseAsync(), builder.Configuration.GetValue<string>("BroadcastInterval"));

app.Run();
