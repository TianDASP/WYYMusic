using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using Zack.DomainCommons.Models;

namespace WYYMusic.Domain.Entity
{
    public class Album : BaseEntity
    {
        public Album() { }
        public string Name { get; set; }
        public long? WYYId { get; set; }
        public string? PicUrl { get; set; }
        public List<Artist> Artists { get; set; } = new List<Artist>();
        public List<Music> Musics { get; set; } = new List<Music>();
        public bool IsVisible { get; private set; }

        public static Album Create(string name,long? wyyId = null)
        {
            Album album = new Album();
            album.Id = YitIdHelper.NextId();
            album.Name = name;
            album.IsVisible = false;
            album.WYYId = wyyId;
            return album;
        }

        public Album SetArtists(List<Artist> artists)
        {
            this.Artists = artists;
            return this;
        }

        public Album SetPicUrl(string picUrl)
        {
            this.PicUrl = picUrl;
            return this;
        }

        public Album AddMusic(List<Music> musics)
        {
            this.Musics.AddRange(musics);
            return this;
        }


        // 修改专辑名
        public Album ChangeName(string value)
        {
            this.Name = value;
            return this;
        }
        public Album Hide()
        {
            this.IsVisible = false;
            return this;
        }
        public Album Show()
        {
            this.IsVisible = true;
            return this;
        }
    }
}
