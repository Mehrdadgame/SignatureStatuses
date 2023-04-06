using Confluent.Kafka;
using Newtonsoft.Json;
using Solnet.Rpc;
using Solnet.Rpc.Models;
using Solnet.Rpc.Core.Http;

namespace SignatureStatuses
{
    public class SigatureInfo
    {
        private static readonly IRpcClient RpcClient = ClientFactory.GetClient(Cluster.DevNet);
        private readonly MyDbContext _dbContext = new();
        private readonly int transactionCount = 10;
        private string saveLastSignture;
        private object? typeEvent;
        private object? eventObject;

      public void GetInfoSignature()
        {
            KafkaConsumer.GetLatestMessage();
            var signaturesLast = new RequestResult<List<SignatureStatusInfo>>();
            LastSignatureIndex(_dbContext, transactionCount, out signaturesLast);
            foreach (var signature in signaturesLast.Result)
            {
                var sigModel = new List<SignatureModel> {new SignatureModel {SignatureDataBase = signature.Signature}};
                MyRepository.SaveModelsToDatabase(sigModel, _dbContext.SignatureModels, _dbContext);
                //  Console.WriteLine($"Sig: {signature.Signature}");

                var transaction = RpcClient.GetTransaction(signature.Signature);
                if (transaction?.Result.Transaction?.Message?.Instructions == null) continue;
                var logMessages = transaction.Result.Meta?.LogMessages;
                if (logMessages == null) continue;
                // Process the events
                foreach (var t in logMessages)
                {
                    var instruction = t.StartsWith("Program log: Instruction: ");

                    if (instruction)
                    {
                        var replaseInstruction = t.Replace("Program log: Instruction: ", "");
                        switch (replaseInstruction)
                        {
                            case "HatchNonGenesisBee":
                            case "HatchNonGenesisQueen":
                                typeEvent = EventsModel.Events.HatchEvent;
                                break;
                            case "SaveBee":
                            case "SaveQueen":
                                typeEvent = EventsModel.Events.SaveEvent;
                                break;
                            case "CreateBee":
                            case "CreateQueen":
                                typeEvent = EventsModel.Events.CreateEggEvent;
                                break;
                            case "CreateItem":
                                typeEvent = EventsModel.Events.CreateItemEvent;
                                break;
                            case "LoginRegister":
                                typeEvent = EventsModel.Events.LoginEvent;
                                break;
                            case "UpgradeBee":
                            case "UpgradeQueen":
                                typeEvent = EventsModel.Events.UpgradeEvent;
                                break;
                        }
                    }


                    if (t.StartsWith("Program data: "))
                    {
                        var eventData = t;
                        var replase = eventData.Replace("Program data: ", "");
                        byte[] decodedBytes = Convert.FromBase64String(replase);

                        switch (typeEvent)
                        {
                            case EventsModel.Events.RerollEvent:
                                var reoll = RerollEvent.DesrelizeLRerollEvent(decodedBytes);
                                KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(reoll));
                                break;
                            case EventsModel.Events.HatchEvent:
                                var hatch = HatchEvent.DeserializeHatchEvent(decodedBytes);
                                KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(hatch));
                                break;
                            case EventsModel.Events.BreedEvent:
                                var breed = BreedEvent.DeserializeBreedEvent(decodedBytes);
                                KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(breed));
                                break;
                            case EventsModel.Events.CreateItemEvent:
                                var createdItem = CreateItemEvent.DeserializeCreateItemEventEvent(decodedBytes);
                                KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(createdItem));
                                break;
                            case EventsModel.Events.CreateEggEvent:
                                var create = CreateEggEvent.DesrelizeCreateEgg(decodedBytes);
                                KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(create));
                                break;
                            case EventsModel.Events.UpgradeEvent:
                                var upgradet = UpgradeEvent.DesrelizeUpgrade(decodedBytes);
                                KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(upgradet));
                                break;
                            case EventsModel.Events.LoginEvent:
                                var loginEvent = LoginEvent.DesrelizeLoginEvent(decodedBytes);
                                KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(loginEvent));

                                break;
                            case EventsModel.Events.WithdrawEvent:
                                var wtihdrow = WithdrawEvent.DesrelizeLWithdrawEvent(decodedBytes);
                                KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(wtihdrow));

                                break;
                            case EventsModel.Events.SaveEvent:
                                var save = SaveEvent.DeserializSaveEvent(decodedBytes);
                                KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(save));

                                break;
                        }
                    }

                    saveLastSignture = signature.Signature;
                }
            }

            SaveLastSinature(saveLastSignture, _dbContext);
        }

        public async Task GetInfoSignatureAsync()
        {
            var signaturesLast = new RequestResult<List<SignatureStatusInfo>>();
               LastSignatureIndex(_dbContext, transactionCount, out signaturesLast);
            foreach (var signature in signaturesLast.Result)
            {
                var sigModel = new List<SignatureModel> {new SignatureModel {SignatureDataBase = signature.Signature}};
                 MyRepository.SaveModelsToDatabase(sigModel, _dbContext.SignatureModels, _dbContext);

                var transaction = await RpcClient.GetTransactionAsync(signature.Signature);
                if (transaction?.Result.Transaction?.Message?.Instructions == null) continue;

                var logMessages = transaction.Result.Meta?.LogMessages;
                if (logMessages == null) continue;

                foreach (var t in logMessages)
                {
                    var instruction = t.StartsWith("Program log: Instruction: ");

                    if (instruction)
                    {
                         typeEvent = t.Replace("Program log: Instruction: ", "") switch
                        {
                            "HatchNonGenesisBee" or "HatchNonGenesisQueen" => EventsModel.Events.HatchEvent,
                            "SaveBee" or "SaveQueen" => EventsModel.Events.SaveEvent,
                            "CreateBee" or "CreateQueen" => EventsModel.Events.CreateEggEvent,
                            "CreateItem" => EventsModel.Events.CreateItemEvent,
                            "LoginRegister" => EventsModel.Events.LoginEvent,
                            "UpgradeBee" or "UpgradeQueen" => EventsModel.Events.UpgradeEvent,
                            _ => null
                        };

                        if (typeEvent == null) continue;

                        if (t.StartsWith("Program data: "))
                        {
                            var eventData = t.Replace("Program data: ", "");
                            byte[] decodedBytes = Convert.FromBase64String(eventData);

                             eventObject = typeEvent switch
                            {
                                EventsModel.Events.RerollEvent => RerollEvent.DesrelizeLRerollEvent(decodedBytes),
                                EventsModel.Events.HatchEvent => HatchEvent.DeserializeHatchEvent(decodedBytes),
                                EventsModel.Events.BreedEvent => BreedEvent.DeserializeBreedEvent(decodedBytes),
                                EventsModel.Events.CreateItemEvent => CreateItemEvent.DeserializeCreateItemEventEvent(decodedBytes),
                                EventsModel.Events.CreateEggEvent => CreateEggEvent.DesrelizeCreateEgg(decodedBytes),
                                EventsModel.Events.UpgradeEvent => UpgradeEvent.DesrelizeUpgrade(decodedBytes),
                                EventsModel.Events.LoginEvent => LoginEvent.DesrelizeLoginEvent(decodedBytes),
                                EventsModel.Events.WithdrawEvent => WithdrawEvent.DesrelizeLWithdrawEvent(decodedBytes),
                                EventsModel.Events.SaveEvent => SaveEvent.DeserializSaveEvent(decodedBytes),
                                _ => null
                            };


                            if (eventObject == null) continue;
                            Console.WriteLine(eventObject);
                             KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(eventObject));
                             saveLastSignture = signature.Signature;
                        }
                    }
                }
            }

            ;
            SaveLastSinature(saveLastSignture, _dbContext);

        }


        private void LastSignatureIndex(MyDbContext db, int transactionCount,
            out RequestResult<List<SignatureStatusInfo>> signatureStatus)
        {
            if (db.SignatureModels.Any())
            {
                var lastIndex = db.lastSignatures.FirstOrDefault();
                signatureStatus = RpcClient.GetSignaturesForAddress("2Vez3DvZ2rdCQazovZpx5iLBNJwz5K84NXMXWyfcum7g",
                    limit: (ulong) transactionCount, lastIndex.Signature);
            }
            else
            {
                signatureStatus = RpcClient.GetSignaturesForAddress("2Vez3DvZ2rdCQazovZpx5iLBNJwz5K84NXMXWyfcum7g",
                    limit: (ulong) transactionCount);
            }
        }

        private void SaveLastSinature(string sign, MyDbContext db)
        {
            var firstEntity = db.lastSignatures.FirstOrDefault();
            if (firstEntity != null)
            {
                db.lastSignatures.Remove(firstEntity);
                var last = new LastSignature()
                {
                    Signature = sign
                };

                db.lastSignatures.Add(last);
                db.SaveChanges();
            }
        }
    }
}

public class TransactionRequest
{
    public string Method { get; set; }
    public List<object> Params { get; set; }
    public string Jsonrpc { get; set; }
    public int Id { get; set; }
}

public class KafkaConsumer
{
    private readonly ConsumerConfig config;
    private readonly string topic;
    private readonly IConsumer<Ignore, string> consumer;

    public KafkaConsumer(string brokerList, string groupId, string topic)
    {
        this.config = new ConsumerConfig
        {
            BootstrapServers = brokerList,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Latest
        };
        this.topic = topic;
        this.consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        this.consumer.Subscribe(topic);
    }

    public static void GetLatestMessage()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "my_group",
            AutoOffsetReset = AutoOffsetReset.Latest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe("my-backup-topic");

        TopicPartition topicPartition = new TopicPartition("my-backup-topic", new Partition(0));
        var watermarkOffsets = consumer.QueryWatermarkOffsets(topicPartition, TimeSpan.FromSeconds(10));
        var lastOffset = watermarkOffsets.High;

        consumer.Assign(new TopicPartitionOffset[] {new TopicPartitionOffset(topicPartition, lastOffset - 1)});

        try
        {
            var consumeResult = consumer.Consume(TimeSpan.FromSeconds(10));
            if (consumeResult != null)
            {
                Console.WriteLine($"Received last message: {consumeResult.Message.Value}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error consuming last message: {e.Message}");
        }
    }

    public static void SaveSignatureInBroker(string data)
    {
        var config = new ProducerConfig {BootstrapServers = "localhost:9092"};
        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            var message = new Message<Null, string> {Value = data};
            var deliveryResult = producer.ProduceAsync("EventData", message).GetAwaiter().GetResult();
            Console.WriteLine($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
        }
    }
    public static string SerializeToJson<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
}

public class KafkaProducer
{
    private readonly string _topic;
    private readonly IProducer<Null, string> _producer;

    public KafkaProducer(string topic)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };
        this._topic = topic;
        this._producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public void Produce(string message)
    {
        var deliveryReport = this._producer.ProduceAsync(this._topic, new Message<Null, string> {Value = message})
            .GetAwaiter().GetResult();
        Console.WriteLine($"Delivered '{deliveryReport.Value}' to '{deliveryReport.TopicPartitionOffset}'");
    }

  
}