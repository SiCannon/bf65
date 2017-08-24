using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace mysql_test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("start");

            using (var context = PieContextFactory.Create(Globals.ConnectionString))
            {
                if (!context.Person.Any())
                {
                    context.Person.Add(new Person { Name = "Jimmy", Age = 0 });
                    context.Person.Add(new Person { Name = "Adina" });
                    context.Person.Add(new Person { Name = "Simon" });
                    context.SaveChanges();
                }
            }

            using (var context = PieContextFactory.Create(Globals.ConnectionString))
            {
                var names = context.Person.Select(x => x.Name).ToList();
                foreach (var name in names)
                {
                    Console.WriteLine("name: " + name);
                }
            }

            Console.WriteLine("end");            
        }
    }

    public class Person
    {
        public int PersonId { get; set; }
        public string Name  { get; set; }
        public int Age { get; set; }
    }

    public class PieContext : DbContext
    {
        public DbSet<Person> Person { get; set; }

        public PieContext(DbContextOptions<PieContext> options) : base(options)
        {
            CommonInit();
        }

        public PieContext()
        {
            CommonInit();
        }

        private void CommonInit()
        {
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(Globals.ConnectionString);
        }
    }

    public static class PieContextFactory
    {
        //public static string connString { get; set; }

        public static PieContext Create(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PieContext>();
            optionsBuilder.UseMySQL(connectionString);
        
            //Ensure database creation
            var context = new PieContext(optionsBuilder.Options);
            context.Database.EnsureCreated();
        
            return context;
        }
    }    

    static class Globals
    {
        private static string RawConnectionString = "server=localhost;Uid=root;Pwd=%PASSWORD%;port=3306;database=test2;sslmode=none";

        public static string ConnectionString
        {
            get
            {
                string password = File.ReadAllText("password.txt");
                return RawConnectionString.Replace("%PASSWORD%", password);
            }
        } 
    }
}
