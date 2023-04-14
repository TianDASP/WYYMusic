using FluentValidation;
using WYYMusic.Infrastructure;
using Zack.DomainCommons.Models;

namespace WYYMusic.Admin.WebApi.Controllers
{
    public record AlbumAddRequest(string Name, string AlbumPicUrl, long? WYYId = null);

    //把校验规则写到单独的文件，也是DDD的一种原则
    public class AlbumAddRequestValidator : AbstractValidator<AlbumAddRequest>
    {
        public AlbumAddRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x=>x.AlbumPicUrl).NotEmpty();
        }
    }
}