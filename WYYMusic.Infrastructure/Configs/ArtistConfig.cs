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
    public class ArtistConfig : IEntityTypeConfiguration<Artist>
    {
        public void Configure(EntityTypeBuilder<Artist> builder)
        {
            builder.ToTable("T_Artists");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.IsDeleted);
            builder.HasMany(x => x.Musics).WithMany(a => a.Artists).UsingEntity(j => j.ToTable("T_Musics_Artists"));
            builder.HasMany(x => x.Albums).WithMany(a => a.Artists).UsingEntity(j => j.ToTable("T_Artists_Albums"));
        }
    }
}
