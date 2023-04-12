using Confluent.Kafka;

using ShartedLib;

using System.Text.Json;

var clientConfig = new ClientConfig { BootstrapServers = ConfigValues.BootstrapServers };

var consumerConfig = new ConsumerConfig(clientConfig) {
    GroupId = ConfigValues.StreamGroupId,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false
};

var producerConfig = new ProducerConfig(clientConfig);

Consume(ConfigValues.StreamTopic, consumerConfig);
await Produce(ConfigValues.StreamTopic, producerConfig);

static async Task Produce(string topicName, ClientConfig config) {
    Console.WriteLine($"{nameof(Produce)} starting");

    // The URL of the EventStreams service.
    string eventStreamsUrl = "https://stream.wikimedia.org/v2/stream/recentchange";

    // Declare the producer reference here to enable calling the Flush
    // method in the finally block, when the app shuts down.
    IProducer<string, string> producer = null;

    try {
        // Build a producer based on the provided configuration.
        // It will be disposed in the finally block.
        producer = new ProducerBuilder<string, string>(config).Build();

        using var httpClient = new HttpClient();

        using var stream = await httpClient.GetStreamAsync(eventStreamsUrl);

        using var reader = new StreamReader(stream);
        // Read continuously until interrupted by Ctrl+C.
        while (!reader.EndOfStream) {
            var line = reader.ReadLine();

            // The Wikimedia service sends a few lines, but the lines
            // of interest for this demo start with the "data:" prefix. 
            if (!line.StartsWith("data:")) {
                continue;
            }

            var (key, jsonData) = ProcessLine(line);

            // For higher throughput, use the non-blocking Produce call
            // and handle delivery reports out-of-band, instead of awaiting
            // the result of a ProduceAsync call.
            producer.Produce(topicName, new Message<string, string> { Key = key, Value = jsonData },
                (deliveryReport) => {
                    if (deliveryReport.Error.Code != ErrorCode.NoError) {
                        Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                    } else {
                        Console.WriteLine($"Produced message to: {deliveryReport.TopicPartitionOffset}");
                    }
                });
        }
    } catch (Exception ex) {
        Console.WriteLine(ex);
    } finally {
        var queueSize = producer.Flush(TimeSpan.FromSeconds(5));
        if (queueSize > 0) {
            Console.WriteLine("WARNING: Producer event queue has " + queueSize + " pending events on exit.");
        }
        producer.Dispose();
    }
}

static void Consume(string topicName, ConsumerConfig config) {
    Console.WriteLine($"{nameof(Consume)} starting");

    // Enable canceling the consumer loop with Ctrl+C.
    CancellationTokenSource cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) => {
        e.Cancel = true; // prevent the process from terminating.
        cts.Cancel();
    };

    // Build a consumer that uses the provided configuration.
    using var consumer = new ConsumerBuilder<string, string>(config).Build();
    // Subscribe to events from the topic.
    consumer.Subscribe(topicName);

    try {
        // Run until the terminal receives Ctrl+C. 
        while (true) {
            // Consume and deserialize the next message.
            var cr = consumer.Consume(cts.Token);

            // Parse the JSON to extract the URI of the edited page.
            var jsonDoc = JsonDocument.Parse(cr.Message.Value);

            // For consuming from the recent_changes topic. 
            var metaElement = jsonDoc.RootElement.GetProperty("meta");
            var uriElement = metaElement.GetProperty("uri");
            var uri = uriElement.GetString();

            // For consuming from the ksqlDB sink topic.
            // var editsElement = jsonDoc.RootElement.GetProperty("NUM_EDITS");
            // var edits = editsElement.GetInt32();
            // var uri = $"{cr.Message.Key}, edits = {edits}";

            Console.WriteLine($"Consumed record with URI {uri}");
        }
    } catch (OperationCanceledException) {
        // Ctrl+C was pressed.
        Console.WriteLine($"Ctrl+C pressed, consumer exiting");
    } finally {
        consumer.Close();
    }
}

static (string key, string jsonData) ProcessLine(string line) {
    // Extract and deserialize the JSON payload.
    int openBraceIndex = line.IndexOf('{');
    var jsonData = line.Substring(openBraceIndex);
    Console.WriteLine($"Data string: {jsonData}");

    // Parse the JSON to extract the URI of the edited page.
    var jsonDoc = JsonDocument.Parse(jsonData);
    var metaElement = jsonDoc.RootElement.GetProperty("meta");
    var uriElement = metaElement.GetProperty("uri");
    string key = uriElement.GetString() ?? "";
    // Use the URI as the message key.
    return (key, jsonData);
}