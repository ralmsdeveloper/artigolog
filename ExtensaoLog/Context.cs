using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ExtensaoLog
{
    public class SampleContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        public SampleContext()
        {
            //if (Logs == null)
            //{
            //    this.GetService<ILoggerFactory>().AddProvider(new CustomLoggerProvider());
            //    Logs = new List<string>();
            //}
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var sqlConnectionStringBuilder = "Server=(localdb)\\mssqllocaldb;Database=ExemploExtensao;Integrated Security=True;";
            optionsBuilder.UseSqlServer(sqlConnectionStringBuilder); 
            //optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole());
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>();
        }

        public static IList<string> Logs = null;

        private class CustomLoggerProvider : ILoggerProvider
        {
            public ILogger CreateLogger(string categoryName) => new SampleLogger();

            private class SampleLogger : ILogger
            {
                public void Log<TState>(
                    LogLevel logLevel,
                    EventId eventId,
                    TState state,
                    Exception exception,
                    Func<TState, Exception, string> formatter)
                {
                    if (eventId.Id == RelationalEventId.CommandExecuting.Id)
                    {
                        var log = formatter(state, exception);
                        Logs.Add(log);
                    }
                }

                public bool IsEnabled(LogLevel logLevel) => true;

                public IDisposable BeginScope<TState>(TState state) => null;
            }

            public void Dispose() { }
        }
    }
}
