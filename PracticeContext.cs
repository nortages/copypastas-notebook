using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Practice.Models;

#nullable disable

namespace Practice
{
    public class PracticeContext : DbContext
    {
        public PracticeContext()
        {
            
        }

        public PracticeContext(DbContextOptions<PracticeContext> options, IWebHostEnvironment env, IConfiguration configuration)
            : base(options)
        {
            CurrentEnvironment = env;
            Configuration = configuration;
        }

        private IConfiguration Configuration{ get; set; } 
        private IWebHostEnvironment CurrentEnvironment{ get; set; } 
        
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Record> Records { get; set; }
        public virtual DbSet<TagCategory> TagCategories { get; set; }
        
        public string ConnectionString { get; set; }
        private static readonly Regex UnformattedConnectionStringRegex = new("postgres://(.+):(.+)@(.+)/(.+)");

        private static string FormatConnectionString(string databaseUrl)
        {
            var groups = UnformattedConnectionStringRegex.Match(databaseUrl).Groups;
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
            ConnectionString = CurrentEnvironment.IsDevelopment() ?
                Configuration["ConnectionStrings:Remote"] :
                FormatConnectionString(databaseUrlEnvVar);

#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            optionsBuilder.UseNpgsql(ConnectionString)
                .UseLazyLoadingProxies()
                .UseSnakeCaseNamingConvention()
                ;
        }
    }
}
