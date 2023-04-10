using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace SignatureStatuses
{

    // Define your repository interface


    // Define your repository implementation class
    public class MyRepository
    {
        // private readonly MyDbContext _dbContext;

        public MyRepository(MyDbContext dbContext)
        {
            // _dbContext = dbContext;
        }



        public static void SaveModelsToDatabase<T>(List<T> models, MyDbContext myDbContext) where T : class
        {


            foreach (var model in models)
            {
                myDbContext.Add(model);

            }
            myDbContext.SaveChanges();

        }

       
    }

    // Define your database context class
    public class MyDbContext : DbContext
    {
        public DbSet<HatchEvent> Hatch { get; set; }
        public DbSet<BreedEvent> BreedEvents { get; set; }
        public DbSet<CreateEggEvent> CreateEggEvents { get; set; }
        public DbSet<CreateItemEvent> CreateItemEvents { get; set; }
        public DbSet<LoginEvent> LoginEvents { get; set; }
        public DbSet<RerollEvent> RerollEvents { get; set; }
        public DbSet<SaveEvent> SaveEvents { get; set; }
        public DbSet<UpgradeEvent> UpgradeEvents { get; set; }
        public DbSet<WithdrawEvent> WithdrawEvents { get; set; }
        public DbSet<SignatureModel> SignatureModels { get; set; }
        public DbSet<LastSignature> lastSignatures { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(local);Integrated Security=True; Initial Catalog=SolanaDataBase;TrustServerCertificate=True;User ID=DESKTOP-NSMVPHG\\PC");
        }
    }

}
