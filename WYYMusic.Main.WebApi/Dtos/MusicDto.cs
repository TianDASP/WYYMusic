 
using AutoMapper;
using WYYMusic.Domain.Entity;

namespace WYYMusic.Main.WebApi.Dtos
{
    public class AlbumDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string picUrl { get; set; }
    }

    public class MusicDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public string AudioUrl { get; set; }
        public string LyricUrl { get; set; }
        public AlbumDto Album { get; set; }
        public List<Artist>  Artists { get; set; }
    }

    public class MusicAndAlbumProfile : Profile
    {
        public MusicAndAlbumProfile()
        {
            CreateMap<Album, AlbumDto>();
            CreateMap<Music, MusicDto>()
                .ForMember(dest => dest.Album, opt => opt.MapFrom(src => src.Album))
                .ForMember(dest=>dest.Duration,opt=>opt.MapFrom(src=>src.DurationInMilliSecond));
        }
    }
}
