using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYYMusic.Domain.Entity;

namespace WYYMusic.Infrastructure.Configs
{
    public class AlbumConfig : IEntityTypeConfiguration<Album>
    {
        public void Configure(EntityTypeBuilder<Album> builder)
        {
            builder.ToTable("T_Albums");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.IsDeleted);
            builder.Property(x => x.PicUrl).HasMaxLength(1000).IsUnicode();
            builder.HasMany(x => x.Musics).WithOne(m => m.Album);
        }
    }
}
