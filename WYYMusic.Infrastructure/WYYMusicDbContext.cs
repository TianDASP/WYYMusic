using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYYMusic.Domain.Entity;

namespace WYYMusic.Infrastructure
{
    public class WYYMusicDbContext : DbContext
    { 
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get;set; }
        public DbSet<Music> Musics { get; set; }
        public WYYMusicDbContext(DbContextOptions options): base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            builder.EnableSoftDeletionGlobalFilter();
        } 
    }
}
