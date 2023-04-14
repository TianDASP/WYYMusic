﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zack.DomainCommons.Models;

namespace FileService.Domain.Entity
{
    public class UploadedAudio :UploadedItem
    {
        public static UploadedAudio Create(long id, string fileName, long fileSizeInBytes, string fileSHA256Hash, Uri backupUrl, Uri remoteUrl)
        {
            return new UploadedAudio()
            {
                Id = id,
                FileName = fileName,
                FileSizeInBytes = fileSizeInBytes,
                FileSHA256Hash = fileSHA256Hash,
                BackupUrl = backupUrl,
                RemoteUrl = remoteUrl,
                CreationTime = DateTime.Now,
            };
        }
    }
}
