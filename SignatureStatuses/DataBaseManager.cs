using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace SignatureStatuses
{

    // Define your repository interface
    public interface IMyRepository
    {
        Task<bool> CheckSignature(string id);
        void SaveModelsToDatabase<T>(List<T> models, DbSet<T> dbSet) where T : class;
    }

    // Define your repository implementation class
    public class MyRepository : IMyRepository
    {
        private readonly MyDbContext _dbContext;

        public MyRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }



        public void SaveModelsToDatabase<T>(List<T> models, DbSet<T> dbSet) where T : class
        {
            using (var dbContext = new MyDbContext())
            {
                foreach (var model in models)
                {
                    dbSet.Add(model);
                }
                dbContext.SaveChanges();
            }
        }

        async Task<bool> IMyRepository.CheckSignature(string id)
        {
            var existingRow = await _dbContext.SignatureModels.FirstOrDefaultAsync(x => x.SignatureDataBase == id);

            if (existingRow == null)
            {
                var newRow = new SignatureModel()
                {
                    SignatureDataBase = existingRow.SignatureDataBase
                };

                _dbContext.SignatureModels.Add(newRow);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
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
        public string DbPath { get; }
        public MyDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "SolanaData.dbo");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($"Data Source={DbPath}");
        }
    }

}
