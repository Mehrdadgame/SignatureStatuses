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

        /// <summary>
        // The GetInfoSignature() method retrieves the latest message from the Kafka database and retrieves the last signature index.// Then, it loops through each signature in the list of signature status information and creates a new list of signature models for each signature, which are then saved to the database.
        // Next, it retrieves the transaction corresponding to the current signature and processes the log messages to determine the type of event and event data. Finally, it saves the last signature and updates the database. This method appears to be part of a larger system for processing blockchain events and logging them in a Kafka database.
        /// </summary>
        // public void GetInfoSignature()
        // {
        //     // Get the latest message from database
        //    // KafkaConsumer.GetLatestMessage();
        //     // Initialize a variable for storing the last signature index
        //     var signaturesLast = new RequestResult<List<SignatureStatusInfo>>();
        //     // Call the LastSignatureIndex method to get the last signature index
        //     LastSignatureIndex(_dbContext, transactionCount, out signaturesLast);
        //     // Loop through each signature in the list of signature status info
        //     foreach (var signature in signaturesLast.Result)
        //     {
        //         // Create a new list of signature models and add the current signature to it
        //         var signatureModels = new List<SignatureModel>
        //             {new SignatureModel {SignatureDataBase = signature.Signature}};
        //         // Save the signature models to the database
        //         MyRepository.SaveModelsToDatabase(signatureModels, _dbContext);
        //         //  Console.WriteLine($"Sig: {signature.Signature}");
        //         // Get the transaction corresponding to the current signature
        //         var transaction = RpcClient.GetTransaction(signature.Signature);
        //         // If the transaction is null or the message or instructions are null, continue to the next signature
        //         if (transaction?.Result.Transaction?.Message?.Instructions == null) continue;
        //         // Get the log messages from the transaction result metadata
        //         var logMessages = transaction.Result.Meta?.LogMessages;
        //         // If the log messages are null, continue to the next signature
        //         if (logMessages == null) continue;
        //         // Process the events
        //         foreach (var data in logMessages)
        //         {
        //             // Check if the log message starts with "Program log: Instruction: "
        //             var instruction = data.StartsWith("Program log: Instruction: ");
        //
        //             if (instruction)
        //             {
        //                 // Replace the prefix "Program log: Instruction: " with an empty string to get the instruction
        //                 var replaceInstruction = data.Replace("Program log: Instruction: ", "");
        //                 // Determine the type of event based on the instruction
        //                 switch (replaceInstruction)
        //                 {
        //                     case "HatchNonGenesisBee":
        //                     case "HatchNonGenesisQueen":
        //                         typeEvent = EventsModel.Events.HatchEvent;
        //                         break;
        //                     case "SaveBee":
        //                     case "SaveQueen":
        //                         typeEvent = EventsModel.Events.SaveEvent;
        //                         break;
        //                     case "CreateBee":
        //                     case "CreateQueen":
        //                         typeEvent = EventsModel.Events.CreateEggEvent;
        //                         break;
        //                     case "CreateItem":
        //                         typeEvent = EventsModel.Events.CreateItemEvent;
        //                         break;
        //                     case "LoginRegister":
        //                         typeEvent = EventsModel.Events.LoginEvent;
        //                         break;
        //                     case "UpgradeBee":
        //                     case "UpgradeQueen":
        //                         typeEvent = EventsModel.Events.UpgradeEvent;
        //                         break;
        //                 }
        //             }
        //
        //             // Check if the log message starts with "Program data: "
        //             if (data.StartsWith("Program data: "))
        //             {
        //                 // Get the event data by removing the prefix "Program data: "
        //                 var eventData = data;
        //                 var replaceProgramData = eventData.Replace("Program data: ", "");
        //                 // Decode the base64-encoded event data
        //                 byte[] decodedBytes = Convert.FromBase64String(replaceProgramData);
        //                 // Deserialize the event based on the type of event
        //                 switch (typeEvent)
        //                 {
        //                     case EventsModel.Events.RerollEvent:
        //                         var rerollEvent = RerollEvent.DeserializeRerollEvent(decodedBytes);
        //                         KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(rerollEvent));
        //                         break;
        //                     case EventsModel.Events.HatchEvent:
        //                         var hatch = HatchEvent.DeserializeHatchEvent(decodedBytes);
        //                         KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(hatch));
        //                         break;
        //                     case EventsModel.Events.BreedEvent:
        //                         var breed = BreedEvent.DeserializeBreedEvent(decodedBytes);
        //                         KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(breed));
        //                         break;
        //                     case EventsModel.Events.CreateItemEvent:
        //                         var createdItem = CreateItemEvent.DeserializeCreateItemEventEvent(decodedBytes);
        //                         KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(createdItem));
        //                         break;
        //                     case EventsModel.Events.CreateEggEvent:
        //                         var create = CreateEggEvent.DesrelizeCreateEgg(decodedBytes);
        //                         KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(create));
        //                         break;
        //                     case EventsModel.Events.UpgradeEvent:
        //                         var upgrade = UpgradeEvent.DesrelizeUpgrade(decodedBytes);
        //                         KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(upgrade));
        //                         break;
        //                     case EventsModel.Events.LoginEvent:
        //                         var loginEvent = LoginEvent.DesrelizeLoginEvent(decodedBytes);
        //                         KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(loginEvent));
        //
        //                         break;
        //                     case EventsModel.Events.WithdrawEvent:
        //                         var withdrawEvent = WithdrawEvent.DesrelizeLWithdrawEvent(decodedBytes);
        //                         KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(withdrawEvent));
        //
        //                         break;
        //                     case EventsModel.Events.SaveEvent:
        //                         var save = SaveEvent.DeserializSaveEvent(decodedBytes);
        //                         KafkaConsumer.SaveSignatureInBroker(KafkaConsumer.SerializeToJson(save));
        //                         break;
        //                 }
        //             }
        //
        //             saveLastSignture = signature.Signature;
        //         }
        //     }
        //
        //     SaveLastSinature(saveLastSignture, _dbContext);
        // }

        public void GetInfoSignature()
        {
            var signaturesLast = new RequestResult<List<SignatureStatusInfo>>();
            LastSignatureIndex(_dbContext, transactionCount, out signaturesLast);

            var events = new Dictionary<string, EventsModel.Events>()
            {
                {"HatchNonGenesisBee", EventsModel.Events.HatchEvent},
                {"HatchNonGenesisQueen", EventsModel.Events.HatchEvent},
                {"SaveBee", EventsModel.Events.SaveEvent},
                {"SaveQueen", EventsModel.Events.SaveEvent},
                {"CreateBee", EventsModel.Events.CreateEggEvent},
                {"CreateQueen", EventsModel.Events.CreateEggEvent},
                {"CreateItem", EventsModel.Events.CreateItemEvent},
                {"LoginRegister", EventsModel.Events.LoginEvent},
                {"UpgradeBee", EventsModel.Events.UpgradeEvent},
                {"UpgradeQueen", EventsModel.Events.UpgradeEvent}
            };

            foreach (var signature in signaturesLast.Result)
            {
                var signatureModel = new SignatureModel
                {
                    SignatureDataBase = signature.Signature
                };
                MyRepository.SaveModelsToDatabase(new List<SignatureModel> {signatureModel}, _dbContext);

                var transaction = RpcClient.GetTransaction(signature.Signature)?.Result;
                if (transaction == null || transaction.Transaction == null ||
                    transaction.Transaction.Message?.Instructions == null)
                {
                    continue;
                }

                var logMessages = transaction.Meta?.LogMessages;
                if (logMessages == null)
                {
                    continue;
                }

                foreach (var data in logMessages)
                {
                    var instruction = events.FirstOrDefault(x => data.StartsWith($"Program log: Instruction: {x.Key}"));

                    if (!instruction.Equals(default(KeyValuePair<string, EventsModel.Events>)))
                    {
                        typeEvent = instruction.Value;
                    }

                    if (data.StartsWith("Program data: "))
                    {
                        var eventData = data.Replace("Program data: ", "");
                        byte[] decodedBytes = Convert.FromBase64String(eventData);

                        switch (typeEvent)
                        {
                            case EventsModel.Events.RerollEvent:
                                KafkaConsumer.SaveSignatureInBroker(
                                    KafkaConsumer.SerializeToJson(RerollEvent.DeserializeRerollEvent(decodedBytes)));
                                break;
                            case EventsModel.Events.HatchEvent:
                                KafkaConsumer.SaveSignatureInBroker(
                                    KafkaConsumer.SerializeToJson(HatchEvent.DeserializeHatchEvent(decodedBytes)));
                                break;
                            case EventsModel.Events.BreedEvent:
                                KafkaConsumer.SaveSignatureInBroker(
                                    KafkaConsumer.SerializeToJson(BreedEvent.DeserializeBreedEvent(decodedBytes)));
                                break;
                            case EventsModel.Events.CreateItemEvent:
                                KafkaConsumer.SaveSignatureInBroker(
                                    KafkaConsumer.SerializeToJson(
                                        CreateItemEvent.DeserializeCreateItemEventEvent(decodedBytes)));
                                break;
                            case EventsModel.Events.CreateEggEvent:
                                KafkaConsumer.SaveSignatureInBroker(
                                    KafkaConsumer.SerializeToJson(CreateEggEvent.DesrelizeCreateEgg(decodedBytes)));
                                break;
                            case EventsModel.Events.UpgradeEvent:
                                KafkaConsumer.SaveSignatureInBroker(
                                    KafkaConsumer.SerializeToJson(UpgradeEvent.DesrelizeUpgrade(decodedBytes)));
                                break;
                            case EventsModel.Events.LoginEvent:
                                KafkaConsumer.SaveSignatureInBroker(
                                    KafkaConsumer.SerializeToJson(LoginEvent.DesrelizeLoginEvent(decodedBytes)));
                                break;
                            case EventsModel.Events.WithdrawEvent:
                                KafkaConsumer.SaveSignatureInBroker(
                                    KafkaConsumer.SerializeToJson(WithdrawEvent.DesrelizeLWithdrawEvent(decodedBytes)));
                                break;
                            case EventsModel.Events.SaveEvent:
                                KafkaConsumer.SaveSignatureInBroker(
                                    KafkaConsumer.SerializeToJson(SaveEvent.DeserializSaveEvent(decodedBytes)));
                                break;
                        }
                    }

                    saveLastSignture = signature.Signature;
                }
            }

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