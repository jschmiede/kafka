using Confluent.Kafka;

using ShartedLib;

namespace PublisherApi {
    public interface IWeaderDataPublisher {
        Task<DeliveryResult<Null, string>> PublishAsync(Weather weather);
    }
}