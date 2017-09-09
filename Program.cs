using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace mysql_test
{
    class Program
    {
        public enum DatabaseType { MySql, Postgres };
        public const DatabaseType dbType = DatabaseType.MySql;

        static void Main(string[] args)
        {
            Console.WriteLine("start");

            using (var context = new PieContext())
            {
                if (!context.Person.Any())
                {
                    context.Person.Add(new Person { Name = "Jimmy", Age = 0 });
                    context.Person.Add(new Person { Name = "Adina", Age = 38 });
                    context.Person.Add(new Person { Name = "Simon", Age = 41 });
                    context.SaveChanges();
                }
            }

            using (var context = new PieContext())
            {
                foreach (var p in context.Person)
                {
                    Console.WriteLine($"name: {p.Name}, age: {p.Age}");
                }
            }

            Console.WriteLine("end");
        }
    }

    public class Person
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
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
            optionsBuilder.UseSepecifiedDatabase();
        }
    }

    public static class DatabaseSelect
    {
        public static DbContextOptionsBuilder UseSepecifiedDatabase(this DbContextOptionsBuilder optionsBuilder)
        {
            switch (Program.dbType)
            {
                case Program.DatabaseType.MySql:
                    return optionsBuilder.UseMySQL(ConnectionString);
                case Program.DatabaseType.Postgres:
                    return optionsBuilder.UseNpgsql(ConnectionString);
            }
        }

        private static string MySqlConnectionString = "server=localhost;Uid=root;Pwd=%PASSWORD%;port=3306;database=pies;sslmode=none";
        private static string PostgresConnectionString = "User ID=postgres;Password=%PASSWORD%;Host=localhost;Port=5432;Database=pies";

        public static string RawConnectionString
        {
            get
            {
                switch (Program.dbType)
                {
                    case Program.DatabaseType.MySql:
                        return MySqlConnectionString;
                    case Program.DatabaseType.Postgres:
                        return PostgresConnectionString;
                }
            }
        }

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
