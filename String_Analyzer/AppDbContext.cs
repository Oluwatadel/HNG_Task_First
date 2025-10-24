using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace String_Analyzer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<StringEntry> Strings { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // store Properties as JSON
            var converter = new ValueConverter<StringProperties, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<StringProperties>(v, (JsonSerializerOptions?)null) ?? new StringProperties());

            modelBuilder.Entity<StringEntry>().Property(e => e.Properties).HasConversion(converter);
        }
    }
}
