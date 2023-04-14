using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYYMusic.Domain.Entity;

namespace WYYMusic.Domain
{
    public class WYYDomainService
    {
        private readonly IWYYRepository repository;
        public WYYDomainService(IWYYRepository repository)
        {
            this.repository = repository;
        }

        // 创建各个实体并返回
        public Album AddAlbum(string name, long? wyyId = null)
        {
            return Album.Create(name, wyyId);
        }

        public Artist AddArtist(string name, string trans = "", long? wyyId = null)
        {
            return Artist.Create(name, trans, wyyId);
        }

        public Music AddMusic(string name, int durationInMilliSecond, int sequence, long? wyyId = null)
        { 
            var builder = new Music.Builder();
            builder.Name(name).DurationInMilliSecond(durationInMilliSecond).Sequence(sequence).WYYId(wyyId);
            return builder.Build();
        }

        //public async Task<IEnumerable<Album>> AddAlbumsAsync()
        //{

        //}

        //public async Task<IEnumerable<Artist>> AddArtistsAsync()
        //{

        //}
        //public async Task<IEnumerable<Music>> AddMusicsAsync()
        //{

        //}

        public async Task SortMusicsAsync(long albumId, long[] sortedMusicIds)
        {

        }
    }
}
