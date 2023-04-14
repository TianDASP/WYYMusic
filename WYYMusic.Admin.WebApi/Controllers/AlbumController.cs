using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WYYMusic.Domain;
using WYYMusic.Domain.Entity;
using WYYMusic.Infrastructure;

namespace WYYMusic.Admin.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class AlbumController : ControllerBase
    {
        private readonly IWYYRepository repository;
        private readonly WYYMusicDbContext dbctx;
        private readonly WYYDomainService domainService;
        public AlbumController(IWYYRepository repository, WYYMusicDbContext dbctx, WYYDomainService domainService)
        {
            this.repository = repository;
            this.dbctx = dbctx;
            this.domainService = domainService;
        }

        [HttpGet]
        [Route("id")]
        public async Task<ActionResult<Album?>> FindById(long id)
        {
            return await repository.GetAlbumByIdAsync(id);
        }

        [HttpGet]
        [Route("{albumId}/music")]
        public async Task<ActionResult<IEnumerable<Music>>> GetMusics(long albumId)
        {
            var musics = await repository.GetMusicsByAlbumIdAsync(albumId);
            return musics?.Count() > 0 ? Ok(musics) : BadRequest("没有此专辑");
        }

        [HttpPost]
        public async Task<long> Create(AlbumAddRequest request)
        {
            // 1: 根据wyyid查询是否数据库中已有
            if (request.WYYId != null)
            {
                var album = await dbctx.Albums.AsNoTracking().FirstOrDefaultAsync(x => x.WYYId == request.WYYId);
                if (album != null)
                {
                    return album.Id;
                }
            }
            // 没有就Create
            var albumToCreate = domainService.AddAlbum(request.Name, request.WYYId);
            albumToCreate.SetPicUrl(request.AlbumPicUrl);
            dbctx.Add(albumToCreate);
            await dbctx.SaveChangesAsync();
            return albumToCreate.Id;
        }
    }
}
