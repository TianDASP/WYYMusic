using FluentValidation;

namespace WYYMusic.Main.WebApi.Controllers
{ 
    public record GetMusicsRequest(int limit,int offset,string keywords);

    //把校验规则写到单独的文件，也是DDD的一种原则
    public class AlbumAddRequestValidator : AbstractValidator<GetMusicsRequest>
    {
        public AlbumAddRequestValidator()
        {
            RuleFor(x => x.limit).NotEmpty().NotNull().Must(x=>x>0);
            RuleFor(x => x.offset).NotEmpty().NotNull().Must(x=>x>0);
            RuleFor(x => x.keywords).NotEmpty().NotNull();
        }
    }
}