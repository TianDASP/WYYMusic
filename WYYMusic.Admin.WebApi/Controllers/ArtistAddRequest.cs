using FluentValidation;

namespace WYYMusic.Admin.WebApi.Controllers
{ 
    public record ArtistAddRequest(string Name,long albumId, long? WYYId = null);

    //把校验规则写到单独的文件，也是DDD的一种原则
    public class ArtistAddRequestValidator : AbstractValidator<ArtistAddRequest>
    {
        public ArtistAddRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty(); 
            RuleFor(x=>x.albumId).NotEmpty();
        }
    }
}