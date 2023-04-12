using Confluent.Kafka;

using Polly;
using Polly.Contrib.WaitAndRetry;

using PublisherApi;

using ShartedLib;

var builder = WebApplication.CreateBuilder(args);


//Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Polly over http client 
builder.Services.AddHttpClient("ApiClient", c => {
    c.BaseAddress = new Uri("https://localhost:7299");
}).AddTransientHttpErrorPolicy(p =>
    p.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5))
).AddTransientHttpErrorPolicy(p =>
    p.CircuitBreakerAsync(6, TimeSpan.FromSeconds(5))
);

//Kafka
var config = new ProducerConfig { BootstrapServers = ConfigValues.BootstrapServers };
builder.Services.AddSingleton(x =>
    new ProducerBuilder<Null, string>(config).Build());
builder.Services.AddSingleton<IWeaderDataPublisher, WeaderDataPublisher>();

builder.Services.AddSingleton<IWeatherForecastService, WeatherForecastService>();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
