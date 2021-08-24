using System;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Practice.Models;

#nullable disable

namespace Practice
{
    public class PracticeContext : DbContext
    {
        private const string ConnectionStringLocal = "Server=localhost;Database=Practice;User Id=postgres;Password=impulse123;";
        private const string ConnectionStringRemote = "Server=ec2-54-216-48-43.eu-west-1.compute.amazonaws.com;" +
                                                      "Database=dmtj26d1dobge;" +
                                                      "User Id=lvhmyovkqyetea;" +
                                                      "Password=d90fc19439587842d08eeaf53693f33c3e96b50886a34e10f44885b12e2dda7b;" +
                                                      "Sslmode=Require;" +
                                                      "Trust Server Certificate=true;";
    
        
        public PracticeContext()
        {
        }

        public PracticeContext(DbContextOptions<PracticeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Record> Records { get; set; }
        public virtual DbSet<TagCategory> TagCategories { get; set; }
        
        public string ConnectionString { get; set; }

        private static string FormatConnectionString(string databaseUrl)
        {
            var regex = new Regex("postgres://(.+):(.+)@(.+)/(.+)");
            var groups = regex.Match(databaseUrl).Groups;
            var user = groups[1];
            var password = groups[2];
            var host = groups[3].Value.Split(':')[0];
            var port = groups[3].Value.Split(':')[1];
            var database = groups[4];

            return $"Server={host};Port={port};Database={database};User Id={user};Password={password};Sslmode=Require;Trust Server Certificate=true;";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            var databaseUrlEnvVar = Environment.GetEnvironmentVariable("DATABASE_URL");
            ConnectionString = databaseUrlEnvVar != null ? 
                FormatConnectionString(databaseUrlEnvVar) : 
                ConnectionStringRemote;

#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            optionsBuilder.UseNpgsql(ConnectionString)
                .UseLazyLoadingProxies()
                .UseSnakeCaseNamingConvention()
                ;
        }
    }
}
