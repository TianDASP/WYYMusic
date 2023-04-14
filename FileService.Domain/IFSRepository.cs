using FileService.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain
{
    public interface IFSRepository
    { 
        // 判断文件是否在数据库已存在
        Task<UploadedAudio?> FindAudioAsync(long fileSizeInBytes, string sha256Hash);
        Task<UploadedLyric?> FindLyricAsync(long fileSizeInBytes, string sha256Hash);
        Task<UploadedPic?> FindPicAsync(long fileSizeInBytes, string sha256Hash);
        Task<UploadedItem?> FindItemAsync(UploadedItemType uploadItemType, long fileSizeInBytes, string sha256Hash);
        Task<UploadedItem?> FindItemAsync(long fileSizeInBytes, string sha256Hash);
    }
}
