
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYYMusic.Domain;
using WYYMusic.Domain.Entity;

namespace WYYMusic.Infrastructure
{
    public class WYYRepository : IWYYRepository
    {
        private readonly WYYMusicDbContext dbCtx;
        public WYYRepository(WYYMusicDbContext ctx)
        {
            this.dbCtx = ctx;
        }
        public async Task<Album?> GetAlbumByIdAsync(long albumId)
        {
            return await dbCtx.FindAsync<Album>(albumId);
        }

        public async Task<IEnumerable<Album>> GetAllAlbumsAsync()
        {
            return await dbCtx.Albums.OrderBy(x => x.Name).ToArrayAsync();
        }

        public async Task<IEnumerable<Artist>> GetAllArtistsAsync()
        {
            return await dbCtx.Artists.OrderBy(x => x.Name).ToArrayAsync();
        }

        public async Task<IEnumerable<Music>> GetAllMusicsAsync()
        {
            return await dbCtx.Musics.OrderBy(x => x.Name).ToArrayAsync();
        }

        public async Task<Artist?> GetArtistByIdAsync(long artistId)
        {
            return await dbCtx.FindAsync<Artist>(artistId);
        }

        public async Task<int> GetMaxSeqOfMusicInAlbum(long albumId)
        {
            if (await GetAlbumByIdAsync(albumId) != null)
            {
                return await dbCtx.Set<Album>().AsNoTracking().Include(x => x.Musics).Where(x => x.Id == albumId)
                  .SelectMany(x => x.Musics).MaxAsync(x => x.No);
            }
            return 0;
        }

        public async Task<Music?> GetMusicByIdAsync(long musicId)
        {
            return await dbCtx.FindAsync<Music>(musicId);
        }

        public async Task<IEnumerable<Music>> GetMusicsByAlbumIdAsync(long albumId)
        {
            var album = await dbCtx.Albums.AsNoTracking().Include(x => x.Musics).FirstOrDefaultAsync(x => x.Id == albumId);
            return album?.Musics;
        }

        public async Task<IEnumerable<Music>> GetMusicByArtistIdAsync(long artistId)
        {
            var artist = await dbCtx.Artists.AsNoTracking().Include(x=>x.Musics).FirstOrDefaultAsync(x=>x.Id == artistId);
            return artist?.Musics;
        }

        public async Task<(IEnumerable<Music>, int)> SearchMusicsAsync(int limit, int offset, string keywords)
        {
            var queryExpression = dbCtx.Musics.Include(x=>x.Album).Include(x=>x.Artists)
                                .Where(x=>x.Name.Contains(keywords) || x.Album.Name.Contains(keywords) || x.Artists.Any(a=>a.Name.Contains(keywords)));
            var count = await queryExpression.CountAsync();
            var items = await queryExpression.Skip(offset-limit).Take(limit).ToListAsync();
            return (items,count);
        }
         
    }
}
