using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYYMusic.Domain.Entity;

namespace WYYMusic.Infrastructure.Configs
{ 
    public class MusicConfig : IEntityTypeConfiguration<Music>
    {
        public void Configure(EntityTypeBuilder<Music> builder)
        {
            builder.ToTable("T_Musics");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.IsDeleted);
            builder.Property(x => x.AudioUrl).HasMaxLength(1000).IsUnicode();
            builder.Property(x=>x.LyricUrl).HasMaxLength(1000).IsUnicode(); 
        }
    }
}
