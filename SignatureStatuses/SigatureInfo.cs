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
using Solnet.Rpc.Models;
using Solnet.Rpc.Core.Http;
using System.Collections.Generic;

namespace SignatureStatuses
{
    public class SigatureInfo
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.DevNet);
        private readonly MyDbContext DbContext = new();
        private int transactionCount = 10;
        private string SaveLastSignture;
        public void GetInfoSignature()
        {

            var signaturesLast = new RequestResult<List<SignatureStatusInfo>>();
            LastSignatureIndex(DbContext, transactionCount, out signaturesLast);
            foreach (var signature in signaturesLast.Result)
            {
                var sigModel = new List<SignatureModel> { new SignatureModel { SignatureDataBase = signature.Signature } };
                MyRepository.SaveModelsToDatabase(sigModel, DbContext.SignatureModels, DbContext);
                //  Console.WriteLine($"Sig: {signature.Signature}");

                var transaction = rpcClient.GetTransaction(signature.Signature);
                var typeEvent = new object();
                if (transaction == null || transaction.Result.Transaction?.Message?.Instructions == null) continue;

                // TransactionRequest request = JsonConvert.DeserializeObject<TransactionRequest>(transaction.RawRpcRequest);




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
                                MyRepository.SaveModelsToDatabase(reollmModel, DbContext.RerollEvents, DbContext);
                                break;
                            case EventsModel.Events.HatchEvent:
                                var hatch = HatchEvent.DeserializeHatchEvent(decodedBytes);
                                var hatchModel = new List<HatchEvent> { hatch };
                                MyRepository.SaveModelsToDatabase(hatchModel, DbContext.Hatch, DbContext);
                                break;
                            case EventsModel.Events.BreedEvent:
                                var breed = BreedEvent.DeserializeBreedEvent(decodedBytes);
                                var breedModel = new List<BreedEvent> { breed };
                                MyRepository.SaveModelsToDatabase(breedModel, DbContext.BreedEvents, DbContext);
                                break;
                            case EventsModel.Events.CreateItemEvent:
                                var createdItem = CreateItemEvent.DeserializeCreateItemEventEvent(decodedBytes);
                                var createItemModelList = new List<CreateItemEvent> { createdItem };
                                MyRepository.SaveModelsToDatabase(createItemModelList, DbContext.CreateItemEvents, DbContext);
                                break;
                            case EventsModel.Events.CreateEggEvent:
                                var create = CreateEggEvent.DesrelizeCreateEgg(decodedBytes);
                                var createModel = new List<CreateEggEvent> { create };
                                MyRepository.SaveModelsToDatabase(createModel, DbContext.CreateEggEvents, DbContext);
                                break;
                            case EventsModel.Events.UpgradeEvent:
                                var upgradet = UpgradeEvent.DesrelizeUpgrade(decodedBytes);
                                var upgradeModel = new List<UpgradeEvent> { upgradet };
                                MyRepository.SaveModelsToDatabase(upgradeModel, DbContext.UpgradeEvents, DbContext);
                                break;
                            case EventsModel.Events.LoginEvent:
                                var loginEvent = LoginEvent.DesrelizeLoginEvent(decodedBytes);
                                var loginModel = new List<LoginEvent> { loginEvent };
                                MyRepository.SaveModelsToDatabase(loginModel, DbContext.LoginEvents, DbContext);
                                break;
                            case EventsModel.Events.WithdrawEvent:
                                var wtihdrow = WithdrawEvent.DesrelizeLWithdrawEvent(decodedBytes);
                                var withdrowModel = new List<WithdrawEvent> { wtihdrow };
                                MyRepository.SaveModelsToDatabase(withdrowModel, DbContext.WithdrawEvents, DbContext);

                                break;
                            case EventsModel.Events.SaveEvent:
                                var save = SaveEvent.DeserializSaveEvent(decodedBytes);
                                var saveModel = new List<SaveEvent> { save };
                                MyRepository.SaveModelsToDatabase(saveModel, DbContext.SaveEvents, DbContext);

                                break;
                        }



                    }
                    SaveLastSignture = signature.Signature;

                }


            }
            SaveLastSinature(SaveLastSignture, DbContext);

        }
        private void LastSignatureIndex(MyDbContext db, int transactionCount, out RequestResult<List<SignatureStatusInfo>> signatureStatus)
        {


            if (db.SignatureModels.Count() > 0)
            {
                var lastIndex = db.lastSignatures.FirstOrDefault();
                signatureStatus = rpcClient.GetSignaturesForAddress("2Vez3DvZ2rdCQazovZpx5iLBNJwz5K84NXMXWyfcum7g", limit: (ulong)transactionCount, lastIndex.Signature);

            }
            else
            {
                signatureStatus = rpcClient.GetSignaturesForAddress("2Vez3DvZ2rdCQazovZpx5iLBNJwz5K84NXMXWyfcum7g", limit: (ulong)transactionCount);
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
