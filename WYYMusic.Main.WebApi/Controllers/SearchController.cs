using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WYYMusic.Domain;
using WYYMusic.Domain.Entity;
using WYYMusic.Main.WebApi.Dtos;

namespace WYYMusic.Main.WebApi.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IWYYRepository repository;
        private readonly IMapper mapper;

        public SearchController(IWYYRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Music>>> GetMusics([FromQuery]GetMusicsRequest request)
        {
            (IEnumerable<Music> musics,int songCount) = await repository.SearchMusicsAsync(request.limit, request.offset, request.keywords);
            bool hasMore = songCount > musics.Count();
            var musicsToReturn = mapper.Map<IEnumerable<MusicDto>>(musics);
            return Ok(new {code = 200, result =new {hasMore,songCount,songs = musicsToReturn } });
        }
    }
}
