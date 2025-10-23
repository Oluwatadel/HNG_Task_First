using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using String_Analyzer;
using System.Data.Common;

namespace test
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
    {
        private readonly DbConnection _connection;

        public CustomWebApplicationFactory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<AppDbContext>));

                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                services.AddDbContext<AppDbContext>((container, options) =>
                {
                    options.UseSqlite(_connection);
                });
            });
        }

        public new void Dispose()
        {
            _connection.Close();
            base.Dispose();
        }
    }
}