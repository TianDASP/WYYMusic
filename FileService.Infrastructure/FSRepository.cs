using FileService.Domain;
using FileService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure
{
    public class FSRepository : IFSRepository
    {
        private readonly FSDbContext ctx;
        public FSRepository(FSDbContext ctx)
        {
            this.ctx = ctx;
        }

        public Task<UploadedAudio?> FindAudioAsync(long fileSizeInBytes, string sha256Hash)
        {
            return ctx.UploadedAudios.FirstOrDefaultAsync(x => x.FileSHA256Hash == sha256Hash && x.FileSizeInBytes == fileSizeInBytes);
        }

        public Task<UploadedLyric?> FindLyricAsync(long fileSizeInBytes, string sha256Hash)
        {
            return ctx.UploadedLyrics.FirstOrDefaultAsync(x => x.FileSHA256Hash == sha256Hash && x.FileSizeInBytes == fileSizeInBytes);
        }

        public Task<UploadedPic?> FindPicAsync(long fileSizeInBytes, string sha256Hash)
        {
            return ctx.UploadedPics.FirstOrDefaultAsync(x => x.FileSHA256Hash == sha256Hash && x.FileSizeInBytes == fileSizeInBytes);
        }

        public async Task<UploadedItem?> FindItemAsync(UploadedItemType uploadedItemType, long fileSizeInBytes, string sha256Hash)
        {
            switch (uploadedItemType)
            {
                case UploadedItemType.Audio:
                    return await FindAudioAsync(fileSizeInBytes, sha256Hash);
                case UploadedItemType.Lyric:
                    return await FindLyricAsync(fileSizeInBytes, sha256Hash);
                case UploadedItemType.Pic:
                    return await FindPicAsync(fileSizeInBytes, sha256Hash);
                default:
                    return null;
            }
        }

        public async Task<UploadedItem?> FindItemAsync(long fileSizeInBytes, string sha256Hash)
        {
            UploadedItem? res;
            res = await FindAudioAsync(fileSizeInBytes, sha256Hash);
            res ??= await FindLyricAsync(fileSizeInBytes, sha256Hash);
            res ??= await FindPicAsync(fileSizeInBytes, sha256Hash);
            return res;
        }
    }
}
