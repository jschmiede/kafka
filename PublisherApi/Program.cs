using Confluent.Kafka;

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

//Kafka
var config = new ProducerConfig { BootstrapServers = ConfigValues.BootstrapServers };
builder.Services.AddSingleton(x =>
    new ProducerBuilder<Null, string>(config).Build());
builder.Services.AddSingleton<IWeaderDataPublisher, WeaderDataPublisher>();





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
