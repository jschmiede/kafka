namespace ShartedLib {
    public sealed class ConfigValues {
        public const string BootstrapServers = "localhost:9092";
        public const string Topic = "weather-topic";
        public const string GroupId = "weather-consumer-group";
        public const string StreamTopic = "recent_changes";
        public const string StreamGroupId = "wiki-edit-stream-group-1";
        public const string StreamUrl = "https://stream.wikimedia.org/v2/stream/recentchange";
    }
}