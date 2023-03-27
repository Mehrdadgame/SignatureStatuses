using System;
using System.Text;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.Encoders;
using Solnet.Rpc;
using Solnet.Wallet.Utilities;
using Hexarc.Borsh;
using SQLitePCL;
using System.Drawing;
using Org.BouncyCastle.Utilities;
using Hexarc.Borsh.Serialization;
using Hexarc.Borsh.Serialization.Metadata;
using Microsoft.EntityFrameworkCore;

namespace SignatureStatuses
{
    public class SigatureInfo
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.DevNet);

        public  void GetInfoSignature()
        {


            var db = new MyDbContext();

           

            var transactionCount = 10;
            var signaturesAll = rpcClient.GetSignaturesForAddress("2Vez3DvZ2rdCQazovZpx5iLBNJwz5K84NXMXWyfcum7g", limit: (ulong)transactionCount);
            foreach (var signature in signaturesAll.Result)
            {
                //  Console.WriteLine($"Sig: {signature.Signature}");
                var transaction = rpcClient.GetTransaction(signature.Signature);
                var typeEvent = new object();
                if (transaction == null || transaction.Result.Transaction?.Message?.Instructions == null) continue;

                var sigcheck =  MyRepository.CheckSignature(signature.Signature, db);
                if (sigcheck) continue;
                var sigModel = new List<SignatureModel> { new SignatureModel { SignatureDataBase = signature.Signature } };
                MyRepository.SaveModelsToDatabase(sigModel, db.SignatureModels, db);


                var logMessages = transaction.Result.Meta?.LogMessages;
                if (logMessages == null) continue;
                // Process the events
                for (int i = 0; i < logMessages.Length; i++)
                {

                    var instruction = logMessages[i].StartsWith("Program log: Instruction: ");

                    if (instruction)
                    {
                        var replaseInstruction = logMessages[i].Replace("Program log: Instruction: ", "");
                        //  Console.WriteLine($"instruction:{replaseInstruction}");
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


                    if (logMessages[i].StartsWith("Program data: "))
                    {
                        var eventData = logMessages[i];
                        var replase = eventData.Replace("Program data: ", "");
                        byte[] decodedBytes = Convert.FromBase64String(replase);

                        switch (typeEvent)
                        {
                            case EventsModel.Events.RerollEvent:
                                var reoll = RerollEvent.DesrelizeLRerollEvent(decodedBytes);
                                var reollmModel = new List<RerollEvent> { reoll };
                                MyRepository.SaveModelsToDatabase(reollmModel, db.RerollEvents, db);
                                break;
                            case EventsModel.Events.HatchEvent:
                                var hatch = HatchEvent.DeserializeHatchEvent(decodedBytes);
                                var hatchModel = new List<HatchEvent> { hatch };
                                MyRepository.SaveModelsToDatabase(hatchModel, db.Hatch, db);
                                break;
                            case EventsModel.Events.BreedEvent:
                                var breed = BreedEvent.DeserializeBreedEvent(decodedBytes);
                                var breedModel = new List<BreedEvent> { breed };
                                MyRepository.SaveModelsToDatabase(breedModel, db.BreedEvents, db);
                                break;
                            case EventsModel.Events.CreateItemEvent:
                                var createdItem = CreateItemEvent.DeserializeCreateItemEventEvent(decodedBytes);
                                var createItemModelList = new List<CreateItemEvent> { createdItem };
                                MyRepository.SaveModelsToDatabase(createItemModelList, db.CreateItemEvents, db);
                                break;
                            case EventsModel.Events.CreateEggEvent:
                                var create = CreateEggEvent.DesrelizeCreateEgg(decodedBytes);
                                var createModel = new List<CreateEggEvent> { create };
                                MyRepository.SaveModelsToDatabase(createModel, db.CreateEggEvents, db);
                                break;
                            case EventsModel.Events.UpgradeEvent:
                                var upgradet = UpgradeEvent.DesrelizeUpgrade(decodedBytes);
                                var upgradeModel = new List<UpgradeEvent> { upgradet };
                                MyRepository.SaveModelsToDatabase(upgradeModel, db.UpgradeEvents, db);
                                break;
                            case EventsModel.Events.LoginEvent:
                                var loginEvent = LoginEvent.DesrelizeLoginEvent(decodedBytes);
                                var loginModel = new List<LoginEvent> { loginEvent };
                                MyRepository.SaveModelsToDatabase(loginModel, db.LoginEvents, db);
                                break;
                            case EventsModel.Events.WithdrawEvent:
                                var wtihdrow = WithdrawEvent.DesrelizeLWithdrawEvent(decodedBytes);
                                var withdrowModel = new List<WithdrawEvent> { wtihdrow };
                                MyRepository.SaveModelsToDatabase(withdrowModel, db.WithdrawEvents, db);

                                break;
                            case EventsModel.Events.SaveEvent:
                                var save = SaveEvent.DeserializSaveEvent(decodedBytes);
                                var saveModel = new List<SaveEvent> { save };
                                MyRepository.SaveModelsToDatabase(saveModel, db.SaveEvents, db);

                                break;
                        }



                    }

                }


            }

        }



    }



}
