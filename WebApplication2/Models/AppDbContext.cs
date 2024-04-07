using Microsoft.EntityFrameworkCore;
using System.IO;

namespace WebApplication2.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<FileInfo> Files => Set<FileInfo>();
        public DbSet<ChunkInfo> Chunks => Set<ChunkInfo>();
        public DbSet<KeyInfo> Keys => Set<KeyInfo>();
        public DbSet<UserKey> AccessInfos => Set<UserKey>();

        public AppDbContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=test.db");
        }
    }
}
