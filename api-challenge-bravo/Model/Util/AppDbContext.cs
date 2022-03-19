﻿using Microsoft.EntityFrameworkCore;

namespace api_challenge_bravo.Model.Util
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var host = System.Diagnostics.Debugger.IsAttached ? "localhost" : "db_MySQL";
            optionsBuilder
                .UseMySql(@$"Server={host};Database=currencydb;Uid=root;Pwd=dbdevpassword;");
        }
        public DbSet<Currency> Currencies { get; set; }

    }
}