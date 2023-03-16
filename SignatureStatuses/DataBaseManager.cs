using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hexarc.Borsh;
using Hexarc.Borsh.Serialization;

namespace SignatureStatuses
{
    // Define your data model class
    public class MyDataModel
    {
        private readonly List<Field> fields = new();
        private int id;
        private string? name;
        [Key]
        public int Id { get => id; set => id = value; }

        public string Name { get => name; set => name = value; }

        public List<Field> Fields => fields;
    }

   
    public class Field
    {
        private string? name;
        private string? type;
        private bool index;
        [Key]
        public string Name { get => name; set => name = value; }
        public string Type { get => type; set => type = value; }
        public bool Index { get => index; set => index = value; }
    }


    // Define your repository interface
    public interface IMyRepository
    {
        MyDataModel GetById(int id);
        void Save(MyDataModel data);
    }

    // Define your repository implementation class
    public class MyRepository : IMyRepository
    {
        private readonly MyDbContext _dbContext;




        public MyRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;


        }

        public MyDataModel GetById(int id) => _dbContext.MyDataModels.Find(id);

        public void Save(MyDataModel data)
        {
            if (data.Id == 0)
            {
                _dbContext.MyDataModels.Add(data);
            }
            else
            {
                _dbContext.MyDataModels.Update(data);
            }
            _dbContext.SaveChanges();
        }
    }

    // Define your database context class
    public class MyDbContext : DbContext
    {
        public DbSet<MyDataModel> MyDataModels { get; set; }
        public string DbPath { get; }
        public MyDbContext()
        {

            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "SolDatabase.db");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($"Data Source={DbPath}");
        }
    }

}
