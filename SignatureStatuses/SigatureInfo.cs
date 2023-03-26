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

namespace SignatureStatuses
{
    public class SigatureInfo
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.DevNet);

        public void GetInfoSignature()
        {



            var transactionCount = 100;
            var signaturesAll = rpcClient.GetSignaturesForAddress("2Vez3DvZ2rdCQazovZpx5iLBNJwz5K84NXMXWyfcum7g", limit: (ulong)transactionCount);
            foreach (var signature in signaturesAll.Result)
            {
                //  Console.WriteLine($"Sig: {signature.Signature}");
                var transaction = rpcClient.GetTransaction(signature.Signature);
                var typeEvent = new object();
                if (transaction == null || transaction.Result.Transaction?.Message?.Instructions == null) continue;



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
                                break;
                            case EventsModel.Events.HatchEvent:
                                var hatch = HatchEvent.DeserializeHatchEvent(decodedBytes);
                                break;
                            case EventsModel.Events.BreedEvent:
                                var breed = BreedEvent.DeserializeBreedEvent(decodedBytes);
                                break;
                            case EventsModel.Events.CreateItemEvent:
                                var createdItem = CreateEggEvent.DesrelizeCreateEgg(decodedBytes);
                                break;
                            case EventsModel.Events.CreateEggEvent:
                                var create = CreateEggEvent.DesrelizeCreateEgg(decodedBytes);
                                break;
                            case EventsModel.Events.UpgradeEvent:
                                var upgradet = UpgradeEvent.DesrelizeUpgrade(decodedBytes);
                                break;
                            case EventsModel.Events.LoginEvent:
                                var loginEvent = LoginEvent.DesrelizeLoginEvent(decodedBytes);
                                break;
                            case EventsModel.Events.WithdrawEvent:
                                var wtihdrow = WithdrawEvent.DesrelizeLWithdrawEvent(decodedBytes);
                                break;
                            case EventsModel.Events.SaveEvent:
                                var save = SaveEvent.DeserializSaveEvent(decodedBytes);
                                break;
                            default:
                                break;
                        }



                    }

                }


            }

        }



    }



}
