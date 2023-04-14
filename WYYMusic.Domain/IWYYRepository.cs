
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYYMusic.Domain.Entity;

namespace WYYMusic.Domain
{
    public interface IWYYRepository
    {
        public Task<Album?> GetAlbumByIdAsync(long albumId);
        public Task<IEnumerable<Album>> GetAllAlbumsAsync();
        public Task<Artist?> GetArtistByIdAsync(long artistId);
        public Task<IEnumerable<Artist>> GetAllArtistsAsync();
        public Task<Music?> GetMusicByIdAsync(long musicId);
        public Task<IEnumerable<Music>> GetAllMusicsAsync();
        public Task<IEnumerable<Music>> GetMusicsByAlbumIdAsync(long albumId);
        public Task<IEnumerable<Music>> GetMusicByArtistIdAsync(long artistId);
        public Task<int> GetMaxSeqOfMusicInAlbum(long albumId);
        public Task<(IEnumerable<Music>,int)> SearchMusicsAsync(int limit, int offset, string keywords);
         

    }
}
