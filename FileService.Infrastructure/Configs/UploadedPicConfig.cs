using FileService.Domain.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure.Configs
{ 
    internal class UploadedPicConfig : IEntityTypeConfiguration<UploadedPic>
    {
        public void Configure(EntityTypeBuilder<UploadedPic> builder)
        {
            builder.ToTable("T_FS_UploadedPics");
            builder.HasKey(x => x.Id);
            // 文件名长度1024
            builder.Property(e => e.FileName).IsUnicode().HasMaxLength(1024);
            builder.Property(e => e.FileSHA256Hash).IsUnicode().HasMaxLength(64);
            // 对经常要查询的列添加索引
            builder.HasIndex(e => new { e.FileSHA256Hash, e.FileSizeInBytes });
        }
    }
}
