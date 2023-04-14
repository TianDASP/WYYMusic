using FileService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure
{
    public class FSDbContext : DbContext
    {
        public DbSet<UploadedAudio> UploadedAudios { get; set; }
        public DbSet<UploadedPic> UploadedPics { get; set; }
        public DbSet<UploadedLyric> UploadedLyrics { get; set; }

        public FSDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            //builder.EnableSoftDeletionGlobalFilter();
        }
    }
}
