using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zack.DomainCommons.Models;

namespace FileService.Domain.Entity
{
    public class UploadedItem : IHasCreationTime
    {
        public long Id { get; protected set; }
        public DateTime CreationTime { get; protected set; }
        /// <summary>
        /// 上传的原始文件名
        /// </summary>
        public string FileName { get; protected set; }
        public long FileSizeInBytes { get; protected set; }

        public string FileSHA256Hash { get; protected set; }
        /// <summary>
        /// 备份文件路径
        /// </summary>
        public Uri BackupUrl { get; protected set; }
        /// <summary>
        /// 公开的外部访问路径
        /// </summary>
        public Uri RemoteUrl { get; protected set; }

        

    }
}
