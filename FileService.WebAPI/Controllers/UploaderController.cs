using FileService.Domain;
using FileService.Domain.Entity;
using FileService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileService.WebAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    //[Authorize(Roles ="Admin")]
    public class UploaderController : ControllerBase
    {
        private readonly FSDomainService fSDomainService;
        private readonly IFSRepository fSRepository;
        private readonly FSDbContext fSDbContext;

        public UploaderController(FSDbContext fSDbContext, IFSRepository fSRepository, FSDomainService fSDomainService)
        {
            this.fSDbContext = fSDbContext;
            this.fSRepository = fSRepository;
            this.fSDomainService = fSDomainService;
        }

        /// <summary>
        /// 检查是否有相同文件 
        /// </summary>
        /// <param name="fileSizeInBytes"></param>
        /// <param name="sha256Hash"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<FileExistsResponse> FileExists(UploadedItemType type, long fileSizeInBytes, string sha256Hash)
        {
            if (fileSizeInBytes > 0 && sha256Hash.Length == 64)
            {
                var item = await fSRepository.FindItemAsync(type, fileSizeInBytes, sha256Hash);
                if (item != null)
                {
                    return new FileExistsResponse(true, item.RemoteUrl);
                }
            }
            return new FileExistsResponse(false, null);
        }

        [HttpPost]
        [RequestSizeLimit(60_000_000)]
        public async Task<ActionResult<Uri>?> Upload([FromForm] UploadRequest request, UploadedItemType uploadedItemType, CancellationToken cancellation = default)
        {
            var file = request.File;
            string fileNmae = file.FileName;
            using Stream stream = file.OpenReadStream();
            Uri uri;
            switch (uploadedItemType)
            {
                case UploadedItemType.Audio:
                    var upItem1 = await fSDomainService.UploadAsync<UploadedAudio>(stream, fileNmae, cancellation) as UploadedAudio;
                    if (upItem1 == null)
                    {
                        return BadRequest("已上传该文件!");
                    }
                    fSDbContext.Add(upItem1);
                    uri = upItem1.RemoteUrl;
                    break;
                case UploadedItemType.Lyric:
                    var upItem2 = await fSDomainService.UploadAsync<UploadedLyric>(stream, fileNmae, cancellation) as UploadedLyric;
                    if (upItem2 == null)
                    {
                        return BadRequest("已上传该文件!");
                    }
                    fSDbContext.Add(upItem2);
                    uri = upItem2.RemoteUrl;
                    break;
                case UploadedItemType.Pic:
                    var upItem3 = await fSDomainService.UploadAsync<UploadedPic>(stream, fileNmae, cancellation) as UploadedPic;
                    if (upItem3 == null)
                    {
                        return BadRequest("已上传该文件!");
                    }
                    fSDbContext.Add(upItem3);
                    uri = upItem3.RemoteUrl;
                    break;
                default:
                    return null;
            }
            await fSDbContext.SaveChangesAsync();
            return uri;
        }
    }
}
