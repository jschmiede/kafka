using Confluent.Kafka;

using Newtonsoft.Json;

using ShartedLib;
Console.WriteLine("Consumer");

var config = new ConsumerConfig {
    GroupId = ConfigValues.GroupId,
    BootstrapServers = ConfigValues.BootstrapServers,
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using var consumer = new ConsumerBuilder<Null, string>(config).Build();

consumer.Subscribe(ConfigValues.Topic);


CancellationTokenSource cts = new();

try {
    while (true) {
        var response = consumer.Consume(cts.Token);
        if (response != null) {
            var weather = JsonConvert.DeserializeObject<Weather>(response.Message.Value);
            Console.WriteLine($"State: {weather?.State}, Temp: {weather?.Temperature} F°,  LeaderEpoch:  {response.LeaderEpoch}, IsPartitionEOF:  {response.IsPartitionEOF}, TopicPartitionOffset: {response.TopicPartitionOffset}");
        }
    }
} catch (Exception) {

    throw;
}

