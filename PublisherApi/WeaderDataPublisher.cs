using Confluent.Kafka;

using Newtonsoft.Json;

using ShartedLib;

namespace PublisherApi {
    public class WeaderDataPublisher : IWeaderDataPublisher {
        private readonly IProducer<Null, string> producer;

        public WeaderDataPublisher(IProducer<Null, string> producer) {
            this.producer = producer;
        }

        public async Task<DeliveryResult<Null, String>> PublishAsync(Weather weather) =>

            await producer.ProduceAsync(ConfigValues.Topic, new Message<Null, string> { Value = JsonConvert.SerializeObject(weather) });
    }
}
