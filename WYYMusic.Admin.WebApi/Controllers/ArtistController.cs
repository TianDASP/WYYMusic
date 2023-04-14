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
    public class ArtistController : ControllerBase
    {
        private readonly IWYYRepository repository;
        private readonly WYYMusicDbContext dbctx;
        private readonly WYYDomainService domainService;
        public ArtistController(IWYYRepository repository, WYYMusicDbContext dbctx, WYYDomainService domainService)
        {
            this.repository = repository;
            this.dbctx = dbctx;
            this.domainService = domainService;
        }

        [HttpGet]
        [Route("id")]
        public async Task<ActionResult<Artist?>> FindById(long id)
        {
            return await repository.GetArtistByIdAsync(id);
        }

        [HttpGet]
        [Route("{id}/{music}")]
        public async Task<ActionResult<IEnumerable<Music>>> GetMusics(long artistId)
        {
            var musics = await repository.GetMusicByArtistIdAsync(artistId);
            return musics?.Count() > 0 ? Ok(musics) : BadRequest("没有此歌手");
        }

        [HttpPost]
        public async Task<long> Create(ArtistAddRequest request)
        {
            // 1: 根据wyyid查询是否数据库中已有
            if (request.WYYId != null)
            {
                var artist = await dbctx.Artists.AsNoTracking().FirstOrDefaultAsync(x => x.WYYId == request.WYYId);
                if (artist != null)
                {
                    return artist.Id;
                }
            }
            // 没有就Create
            var artistToCreate = domainService.AddArtist(request.Name, wyyId: request.WYYId);
            var albumsOfThisArtist =await repository.GetAlbumByIdAsync(request.albumId);
            if (albumsOfThisArtist != null)
            {
                artistToCreate.AddAlbums(albumsOfThisArtist);
            }
            dbctx.Add(artistToCreate);
            await dbctx.SaveChangesAsync();
            return artistToCreate.Id;
        }
    }
}
