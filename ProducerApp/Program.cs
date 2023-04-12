// See https://aka.ms/new-console-template for more information

using Confluent.Kafka;

using Newtonsoft.Json;

using ShartedLib;

Console.WriteLine("Producer");

var config = new ProducerConfig { BootstrapServers = ConfigValues.BootstrapServers };
using var producer = new ProducerBuilder<Null, string>(config).Build();

try {
    string? state;
    while ((state = Console.ReadLine()) != null) {
        var response = await producer.ProduceAsync(ConfigValues.Topic, new Message<Null, string> {
            Value = JsonConvert.SerializeObject(new Weather(state, 70))
        });
        Console.WriteLine($"Value: {response.Value}, TopicPartitionOffset: {response.TopicPartitionOffset}, Timestamp: {response.Timestamp.UtcDateTime:HH:mm:ss.fff}, Status: {response.Status}");

    }
} catch (ProduceException<Null, string> ex) {

    Console.WriteLine(ex.Message);
}

