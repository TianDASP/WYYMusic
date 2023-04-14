using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYYMusicUploader
{
    public class AudioInfo : ObservableObject
    {
        public string AudioFilePath { get; set; }
        public string LyricFilePath { get; set; }
        public string AlbumPicPath { get; set; }
        private string audioRemoteUrl;
        public string AudioRemoteUrl
        {
            get => audioRemoteUrl;
            set => SetProperty(ref audioRemoteUrl, value);
        }
        private string lyricRemoteUrl;
        public string LyricRemoteUrl { get => lyricRemoteUrl; set => SetProperty(ref lyricRemoteUrl, value); }
        private string picRemoteUrl;
        public string PicRemoteUrl { get => picRemoteUrl; set => SetProperty(ref picRemoteUrl, value); }
        public string AlbumName { get; set; }
        public long AlbumWYYId { get; set; }
        public List<string> ArtistsName { get; set; } = new List<string>();
        public List<long> ArtistsWYYId { get; set; } = new List<long>();
        public string MusicName { get; set; }
        public long MusicWYYId { get; set; }
        public int DurationInMilliSecond { get; set; }
        public int Sequence { get; set; }

        public long albumPicDocId { get; set; }
        // 符号 ✓✗！  
        public ObservableCollection<string> res { get; set; } = new ObservableCollection<string>() { "", "", "", "", "", "" };
    }

    public enum HttpResult
    {
        Success,
        Fail,
        Exist
    }
}
