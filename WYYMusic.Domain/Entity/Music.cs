using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace WYYMusic.Domain.Entity
{
    public class Music : BaseEntity
    {
        public Music() { }  
        // 原名
        public string Name { get; set; } 
        public int DurationInMilliSecond { get; set; }  
        public string AudioUrl { get; set; }
        public string LyricUrl { get; set; }
        //歌曲在专辑中的排序
        public int No { get; set; }
        public long? WYYId { get; set; }

        public Album Album { get; set; } = new Album();
        public List<Artist> Artists { get; set; } =  new List<Artist>();

        public Music ChangeName(string name)
        {
            this.Name = name;
            return this;
        }
        public Music ChangeSequence(int sequence)
        {
            this.No = sequence;
            return this;
        } 

        public class Builder
        { 
            private string name; 
            private int durationInMilliSecond; 
            //歌曲在专辑中的排序
            private int no;
            private long? wyyId;   
            public Builder Name(string name)
            {
                this.name = name;
                return this;
            }
            public Builder DurationInMilliSecond(int durationInMilliSecond)
            {
                this.durationInMilliSecond= durationInMilliSecond;
                return this;
            }
            public Builder Sequence(int sequence)
            {
                this.no = sequence;
                return this;
            }

            public Builder WYYId(long? wyyId = null)
            {
                this.wyyId = wyyId;
                return this;
            }

            public Music Build()
            { 
                Music music = new Music();
                music.Id = YitIdHelper.NextId();
                music.Name= name;
                music.DurationInMilliSecond= durationInMilliSecond;
                music.No= no;
                music.WYYId= wyyId; 
                return music;
            }
        }
    }
}
