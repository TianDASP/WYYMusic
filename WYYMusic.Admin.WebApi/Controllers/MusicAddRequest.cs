using FluentValidation;

namespace WYYMusic.Admin.WebApi.Controllers
{ 
    public record MusicAddRequest(string Name, long albumId,int Duration, int Sequence,
        long[] ArtistIds, string AudioUrl, string? LyricUrl, long? WYYId = null);

    //把校验规则写到单独的文件，也是DDD的一种原则
    public class MusicAddRequestValidator : AbstractValidator<MusicAddRequest>
    {
        public MusicAddRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.albumId).NotEmpty();
            RuleFor(x => x.Duration).NotEmpty();
            RuleFor(x => x.Sequence).NotEmpty();
            RuleFor(x => x.ArtistIds).NotEmpty();
            RuleFor(x => x.AudioUrl).NotEmpty();
        }
    }
}