using FileService.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using Zack.Commons;

namespace FileService.Domain
{
    public class FSDomainService
    {
        private readonly IFSRepository fSRepository;
        private readonly IStorageClient backupStorage;
        private readonly IStorageClient remoteStorage;
        //private readonly YitIdHelper idHelper;

        public FSDomainService(IFSRepository fSRepository, IEnumerable<IStorageClient> storage)
        {
            this.fSRepository = fSRepository;
            // 同一接口的多个实例的注入
            this.backupStorage = storage.First(s => s.StorageType == StorageType.Backup);
            this.remoteStorage = storage.First(s => s.StorageType == StorageType.Public);
        }

        // 领域服务只有抽象的业务逻辑
        public async Task<UploadedItem?> UploadAsync<T>(Stream stream, string fileName, CancellationToken cancellationToken) where T : UploadedItem
        {
            // 64位字符串
            string hash = HashHelper.ComputeSha256Hash(stream);
            long fileSizeInBytes = stream.Length;

            var res = await fSRepository.FindItemAsync(fileSizeInBytes, hash);

            if (res == null)
            { // 根据hash值来创建层级文件夹参数 根据前3个字符创建 3层目录
                string key = $"{hash[0]}/{hash[1]}/{hash[2]}/{fileName}";
                long id = YitIdHelper.NextId();

                if (typeof(T) == typeof(UploadedAudio))
                {
                    key = "FileService/Audio/" + key;
                    stream.Position = 0;
                    Uri backupUrl = await backupStorage.SaveAsync(key, stream, cancellationToken);//保存到备份服务器
                    stream.Position = 0;
                    Uri remoteUrl = await remoteStorage.SaveAsync(key, stream, cancellationToken);//保存到OSS服务器
                    stream.Position = 0;
                    return UploadedAudio.Create(id, fileName, fileSizeInBytes, hash, backupUrl, remoteUrl);
                }
                else if (typeof(T) == typeof(UploadedLyric))
                {
                    key = "FileService/Lyric/" + key;
                    stream.Position = 0;
                    Uri backupUrl = await backupStorage.SaveAsync(key, stream, cancellationToken);//保存到备份服务器
                    stream.Position = 0;
                    Uri remoteUrl = await remoteStorage.SaveAsync(key, stream, cancellationToken);//保存到OSS服务器
                    stream.Position = 0;
                    return UploadedLyric.Create(id, fileName, fileSizeInBytes, hash, backupUrl, remoteUrl);
                }
                else if (typeof(T) == typeof(UploadedPic))
                {
                    key = "FileService/Pic/" + key;
                    stream.Position = 0;
                    Uri backupUrl = await backupStorage.SaveAsync(key, stream, cancellationToken);//保存到备份服务器
                    stream.Position = 0;
                    Uri remoteUrl = await remoteStorage.SaveAsync(key, stream, cancellationToken);//保存到OSS服务器
                    stream.Position = 0;
                    return UploadedPic.Create(id, fileName, fileSizeInBytes, hash, backupUrl, remoteUrl);
                } 
            } 
            return null;  
        }
    }
}
