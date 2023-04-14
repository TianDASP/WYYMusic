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
    public class MusicController : ControllerBase
    {
        private readonly IWYYRepository repository;
        private readonly WYYMusicDbContext dbctx;
        private readonly WYYDomainService domainService;
        public MusicController(IWYYRepository repository, WYYMusicDbContext dbctx, WYYDomainService domainService)
        {
            this.repository = repository;
            this.dbctx = dbctx;
            this.domainService = domainService;
        }
         
        [HttpGet]
        [Route("id")]
        public async Task<ActionResult<Music?>> FindById(long id)
        {
            return await repository.GetMusicByIdAsync(id);
        }
         
        [HttpPost]
        public async Task<long> Create(MusicAddRequest request)
        {
            // 1: 根据wyyid查询是否数据库中已有
            if (request.WYYId != null)
            {
                var music = await dbctx.Musics.AsNoTracking().FirstOrDefaultAsync(x => x.WYYId == request.WYYId);
                if (music != null)
                {
                    return music.Id;
                }
            }
            // 没有就Create
            Music musicToCreate = domainService.AddMusic(request.Name,request.Duration,request.Sequence,request.WYYId);
            var albumOfThisMusic =await repository.GetAlbumByIdAsync(request.albumId);
            var artistsOfThisMusic = new List<Artist>();
            foreach (var artistId in request.ArtistIds)
            {
                var artist =  await repository.GetArtistByIdAsync(artistId);
                if (artist != null)
                {
                    artistsOfThisMusic.Add(artist);
                }
            }
            if (artistsOfThisMusic != null)
            {
                musicToCreate.Artists = artistsOfThisMusic;
            }
            if (albumOfThisMusic != null)
            {
                musicToCreate.Album = albumOfThisMusic;
            }
            if (request.AudioUrl != null)
            {
                musicToCreate.AudioUrl = request.AudioUrl;
            }
            if(request.LyricUrl != null)
            {
                musicToCreate.LyricUrl = request.LyricUrl;
            }
            dbctx.Add(musicToCreate);
            await dbctx.SaveChangesAsync();
            return musicToCreate.Id;
        }
    }
}
