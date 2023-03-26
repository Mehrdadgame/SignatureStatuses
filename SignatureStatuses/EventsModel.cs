
using Hexarc.Borsh;
using Hexarc.Borsh.Serialization;

namespace SignatureStatuses
{
    public record EventsModel
    {
        public enum Events
        {
            RerollEvent,
            HatchEvent,
            BreedEvent,
            CreateItemEvent,
            CreateEggEvent,
            UpgradeEvent,
            LoginEvent,
            WithdrawEvent,
            SaveEvent
        }
    }

    public record SignatureModel
    {
        public string? SignatureDataBase { get; set; }
    }

    [BorshObject]
    public record RerollEvent
    {


        public long ID { get; set; }
        public string Authority { get; set; }

        public string BeeType { get; set; }

        public string Unique { get; set; }

        public string TokenAddress { get; set; }

        public string Universe { get; set; }

        public string Collection { get; set; }

        public string Gen { get; set; }

        public string Url { get; set; }

        public string LastRerollProgram { get; set; }

        public string LastRerollCounter { get; set; }

        public string RerollCost { get; set; }

        public string TotalReroll { get; set; }

        public long Timestamp { get; set; }


        public RerollEvent(long iD, string authority, string beeType, string unique, string tokenAddress, string universe, string collection, string gen, string url, string lastRerollProgram, string lastRerollCounter, string rerollCost, string totalReroll, long timestamp)
        {
            this.ID = iD;
            this.Authority = authority;
            this.BeeType = beeType;
            this.Unique = unique;
            this.TokenAddress = tokenAddress;
            this.Universe = universe;
            this.Collection = collection;
            this.Gen = gen;
            this.Url = url;
            this.LastRerollProgram = lastRerollProgram;
            this.LastRerollCounter = lastRerollCounter;
            this.RerollCost = rerollCost;
            this.TotalReroll = totalReroll;
            this.Timestamp = timestamp;
        }

        public static RerollEvent DesrelizeLRerollEvent(byte[] data)
        {
            var reder = new BorshReader(data);
            var id = reder.ReadInt64();
            var authority = reder.ReadString();
            var beeType = reder.ReadString();
            var unique = reder.ReadString();
            var tokenAddress = reder.ReadString();
            var universe = reder.ReadString();
            var lastRerollProgram = reder.ReadString();
            var rerollCost = reder.ReadString();
            var totalReroll = reder.ReadString();
            var timestamp = reder.ReadInt64();
            var gen = reder.ReadString();
            var url = reder.ReadString();
            var collection = reder.ReadString();
            var lastReroCounter = reder.ReadString();
            Console.WriteLine(timestamp);
            return new RerollEvent(id, authority, beeType, unique, tokenAddress, universe, collection, gen, url, lastRerollProgram, lastReroCounter, rerollCost, totalReroll, timestamp);
        }

    }


    [BorshObject]
    public record HatchEvent
    {


        public ulong Id { get; set; }


        public string Authority { get; set; }


        public string Beetype { get; set; }


        public string Unique { get; set; }


        public string TokenAddress { get; set; }


        public string Universe { get; set; }


        public string Collection { get; set; }


        public string Gen { get; set; }


        public string Url { get; set; }


        public long Timestamp { get; set; }



        public HatchEvent(ulong id, string authority, string beetype, string unique, string tokenAddress, string universe, string collection, string gen, string url, long timestamp)
        {
            Id = id;
            Authority = authority;
            Beetype = beetype;
            Unique = unique;
            TokenAddress = tokenAddress;
            Universe = universe;
            Collection = collection;
            Gen = gen;
            Url = url;
            Timestamp = timestamp;
        }

        public static HatchEvent DeserializeHatchEvent(byte[] data)
        {

            var reader = new BorshReader(data);
            var id = reader.ReadUInt64();
            var authority = reader.ReadString();
            var beetype = reader.ReadString();
            var unique = reader.ReadString();
            var tokenAddress = reader.ReadString();
            var universe = reader.ReadString();
            var collection = reader.ReadString();
            var gen = reader.ReadString();
            var url = reader.ReadString();
            var timeStamp = reader.ReadInt64();
            Console.WriteLine(timeStamp);
            return new HatchEvent(id, authority, beetype, unique, tokenAddress, universe, collection, gen, url, timeStamp);
        }
    }
    public record BreedEvent
    {


        public long ID { get; set; }

        public string? Authority { get; set; }

        public string? Mother { get; set; }


        public string? Father { get; set; }

        public string? BeeType { get; set; }


        public string? TokenAddress { get; set; }

        public long MatingFee { get; set; }

        public string? Collection { get; set; }

        public string? Gen { get; set; }

        public string? Url { get; set; }

        public long Timestamp { get; set; }

        public BreedEvent(long iD, string? authority, string? mother, string? father, string? beeType, string? tokenAddress, long matingFee, string? collection, string? gen, string? url, long timestamp)
        {
            ID = iD;
            Authority = authority;
            Mother = mother;
            Father = father;
            BeeType = beeType;
            TokenAddress = tokenAddress;
            MatingFee = matingFee;
            Collection = collection;
            Gen = gen;
            Url = url;
            Timestamp = timestamp;
        }

        public static BreedEvent DeserializeBreedEvent(byte[] data)
        {

            var reader = new BorshReader(data);
            var id = reader.ReadInt64();
            var authority = reader.ReadString();
            var mother = reader.ReadString();
            var father = reader.ReadString();
            var beeType = reader.ReadString();
            var tokenAddress = reader.ReadString();
            var matingFee = reader.ReadInt64();
            var collection = reader.ReadString();
            var gen = reader.ReadString();
            var url = reader.ReadString();
            var timestamp = reader.ReadInt64();
            Console.WriteLine(timestamp);
            return new BreedEvent(id, authority, mother, father, beeType, tokenAddress, matingFee, collection, gen, url, timestamp);


        }
    }

    public record CreateItemEvent
    {

        public long ID { get; set; }
        public string? Authority { get; set; }

        public string? Id { get; set; }

        public string? IitemType { get; set; }

        public string? TokenAddress { get; set; }

        public string? Stars { get; set; }

        public long Timestamp { get; set; }

        public CreateItemEvent(long iD, string? authority, string? id, string? iitemType, string? tokenAddress, string? stars, long timestamp)
        {
            ID = iD;
            Authority = authority;
            Id = id;
            IitemType = iitemType;
            TokenAddress = tokenAddress;
            Stars = stars;
            Timestamp = timestamp;
        }


        public static CreateItemEvent DeserializeCreateItemEventEvent(byte[] data)
        {
            var reader = new BorshReader(data);
            var id = reader.ReadInt64();
            var authority = reader.ReadString();
            var idType = reader.ReadString();
            var itemType = reader.ReadString();
            var tokenAddress = reader.ReadString();
            var stars = reader.ReadString();
            var timestamp = reader.ReadInt64();
            Console.WriteLine(timestamp);
            return new CreateItemEvent(id, authority, idType, itemType, tokenAddress, stars, timestamp);
        }
    }
    [BorshObject]
    public record CreateEggEvent
    {
        public long ID { get; set; }
        public string Authority { get; set; }
        public string BeeType { get; set; }
        public string TokenAddress { get; set; }
        public string Collection { get; set; }
        public string Gen { get; set; }
        public string Url { get; set; }
        public long Timestamp { get; set; }

        public CreateEggEvent(long id, string authority, string beeType, string tokenAddress, string collection, string gen, string url, long timestamp)
        {
            ID = id;
            Authority = authority;
            BeeType = beeType;
            TokenAddress = tokenAddress;
            Collection = collection;
            Gen = gen;
            Url = url;
            Timestamp = timestamp;
        }


        public static CreateEggEvent DesrelizeCreateEgg(byte[] data)
        {

            var reader = new BorshReader(data);
            var id = reader.ReadInt64();
            var authority = reader.ReadString();
            var beeType = reader.ReadString();
            var tokenAddress = reader.ReadString();
            var collection = reader.ReadString();
            var gen = reader.ReadString();
            var url = reader.ReadString();
            var timestamp = reader.ReadInt64();

            Console.WriteLine(timestamp);
            return new CreateEggEvent(id, authority, beeType, tokenAddress, collection, gen, url, timestamp);
        }



    }

    public record UpgradeEvent
    {


        public long Id { get; set; }
        public string? Authority { get; set; }


        public string? EntityType { get; set; }


        public string? ItemAddress { get; set; }


        public string? UpgradedEntityTokenAddress { get; set; }

        public string? Gen { get; set; }


        public long? Timestamp { get; set; }

        public UpgradeEvent(long id, string? authority, string? entityType, string? itemAddress, string? upgradedEntityTokenAddress, string? gen, long? timestamp)
        {
            Id = id;
            Authority = authority;
            EntityType = entityType;
            ItemAddress = itemAddress;
            UpgradedEntityTokenAddress = upgradedEntityTokenAddress;
            Gen = gen;
            Timestamp = timestamp;
        }

        public static UpgradeEvent DesrelizeUpgrade(byte[] bytes)
        {
            var reader = new BorshReader(bytes);
            var id = reader.ReadInt64();
            var authority = reader.ReadString();
            var entityType = reader.ReadString();
            var itemAddress = reader.ReadString();
            var upgradedEntityTokenAddress = reader.ReadString();
            var gen = reader.ReadString();
            var timestamp = reader.ReadInt64();

            Console.WriteLine(timestamp);
            return new UpgradeEvent(id, authority, entityType, itemAddress, upgradedEntityTokenAddress, gen, timestamp);
        }

    }

    public record LoginEvent
    {
        public LoginEvent(long iD, string? userId, string? userKey, string? pubKey)
        {
            ID = iD;
            UserId = userId;
            UserKey = userKey;
            PubKey = pubKey;
        }

        public long ID { get; set; }
        public string? UserId { get; set; }

        public string? UserKey { get; set; }

        public string? PubKey { get; set; }

        public static LoginEvent DesrelizeLoginEvent(byte[] data)
        {
            var reader = new BorshReader(data);
            var id = reader.ReadInt64();
            var authority = reader.ReadString();
            var userKey = reader.ReadString();
            var pubKey = reader.ReadString();
            return new LoginEvent(id, authority, userKey, pubKey);
        }
    }

    public record WithdrawEvent
    {
        public WithdrawEvent(long id, string? authority, string? treasury, string? cost, long timestamp)
        {
            this.ID = id;
            this.Authority = authority;
            Treasury = treasury;
            Cost = cost;
            Timestamp = timestamp;
        }

        public long ID { get; set; }
        public string? Authority { get; set; }

        public string? Treasury { get; set; }

        public string? Cost { get; set; }

        public long Timestamp { get; set; }

        public static WithdrawEvent DesrelizeLWithdrawEvent(byte[] data)
        {
            var reader = new BorshReader(data);
            var id = reader.ReadInt64();
            var authority = reader.ReadString();
            var treasury = reader.ReadString();
            var cost = reader.ReadString();
            var timestamp = reader.ReadInt64();

            return new WithdrawEvent(id, authority, treasury, cost, timestamp);
        }
    }

    public record SaveEvent
    {
        public SaveEvent(long id, string? authority, string? beeMint, string? gen, long? timestamp)
        {
            Id = id;
            Authority = authority;
            BeeMint = beeMint;
            Gen = gen;
            Timestamp = timestamp;
        }

        public long Id { get; set; }

        public string? Authority { get; set; }

        public string? BeeMint { get; set; }

        public string? Gen { get; set; }

        public long? Timestamp { get; set; }



        public static SaveEvent DeserializSaveEvent(byte[] data)
        {
            var reder = new BorshReader(data);
            var id = reder.ReadInt64();
            var authirty = reder.ReadString();
            var beeMint = reder.ReadString();
            var gen = reder.ReadString();
            var timestmp = reder.ReadInt64();

            return new SaveEvent(id, authirty, beeMint, gen, timestmp);


        }
    }


}
