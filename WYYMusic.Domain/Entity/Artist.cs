using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace WYYMusic.Domain.Entity
{
    public class Artist : BaseEntity
    { 
        public Artist() { } 
        // 原名
        public string Name { get; set; }
        // 名字的中文翻译
        public string Trans { get; set; }
        public long? WYYId { get; set; }
        public List<Album> Albums { get; set; } = new List<Album>();
        public List<Music> Musics { get; set; } = new List<Music>();
        public static Artist Create(string name, string trans = "", long? wyyId = null)
        {
            Artist artist = new Artist();
            artist.Name = name;
            artist.Trans = trans;
            artist.WYYId = wyyId;
            artist.Id = YitIdHelper.NextId(); 
            return artist;
        }

        public Artist AddAlbums(params Album[] albums)
        {
            this.Albums.AddRange(albums);
            return this;
        }

        public Artist AddMusics(List<Music> musics)
        {
            this.Musics.AddRange(musics);
            return this;
        }

        // 修改专辑名
        public Artist ChangeName(string value)
        {
            this.Name = value;
            return this;
        }
    }
}
