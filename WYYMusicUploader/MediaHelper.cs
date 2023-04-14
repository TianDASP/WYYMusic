using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TagLib;

namespace WYYMusicUploader
{
    public class MediaHelper
    {
        public static AudioInfo GetAudioInfo(string filePath, string tempPath)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("路径错误");
            }
            // 创建一个TagLib文件对象
            TagLib.File file = TagLib.File.Create(filePath);
            AudioInfo info = new AudioInfo();
            // 获取歌曲信息
            //string title = file.Tag.Title;
            //string artist = file.Tag.FirstPerformer;
            //string album = file.Tag.Album;
            //TimeSpan duration = file.Properties.Duration;
            // 获取163key,解密从中获取网易云歌曲的id等信息
            var Tag_163key = file.Tag.Comment;
            info.Sequence = (int)file.Tag.Track; //音轨信息即歌曲在专辑中的排序


            Console.WriteLine("开始解密:");
            var Str_163Key = DecryptStringFromBase64Aes(Tag_163key.Replace("163 key(Don't modify):", ""), "#14ljk_!\\]&0U<'(");
            info = FillAudioInfo(info, Str_163Key);

            IPicture picture = file.Tag.Pictures[0];
            string mimeType = picture.MimeType;
            string extension = mimeType.Split('/')[1];
            string fileFullPath = $"{tempPath}/{info.albumPicDocId}." + extension;
            if (!System.IO.File.Exists(fileFullPath))
            {
                using (MemoryStream stream = new MemoryStream(picture.Data.Data))
                {
                    using (FileStream fileStream = new FileStream(fileFullPath, FileMode.Create))
                    {
                        stream.WriteTo(fileStream);
                    }
                }
            }
            info.AlbumPicPath = fileFullPath;
            info.AudioFilePath = filePath;
            Console.WriteLine(info.AlbumName + "--" + info.ArtistsName);
            return info;
        }

        private static AudioInfo FillAudioInfo(AudioInfo info, string Str163)
        {
            using JsonDocument document = JsonDocument.Parse(Str163.Substring(6));
            JsonElement root = document.RootElement;

            info.AlbumName = root.GetProperty("album").GetString();
            info.AlbumWYYId = root.GetProperty("albumId").GetInt64();
            info.albumPicDocId = root.GetProperty("albumPicDocId").GetInt64();

            JsonElement artist = root.GetProperty("artist");
            int length = artist.GetArrayLength();
            for (int i = 0; i < length; i++)
            {
                JsonElement artistinfo = artist[i];
                info.ArtistsName.Add(artistinfo[0].GetString());
                info.ArtistsWYYId.Add(artistinfo[1].GetInt64());
            }

            info.MusicName = root.GetProperty("musicName").GetString();
            info.MusicWYYId = root.GetProperty("musicId").GetInt64();
            info.DurationInMilliSecond = root.GetProperty("duration").GetInt32();

            return info;
        }
        public static Dictionary<long, AudioInfo> GetAllMediaInfoInDir(Dictionary<long, AudioInfo> dic, string filePath)
        { 
            GetAllMediaInfoInDir(ref dic, filePath);
            return dic;
        }
        /// <summary>
        /// 递归遍历文件夹内的所有mp3文件
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static void GetAllMediaInfoInDir(ref Dictionary<long, AudioInfo> dic, string filePath)
        {
            var directoryInfo = new DirectoryInfo(filePath);
            var fileInfos = directoryInfo.GetFiles();
            var directoryInfos = directoryInfo.GetDirectories();
            if (directoryInfos.Length > 0)
            {
                foreach (var item in directoryInfos)
                {
                    GetAllMediaInfoInDir(ref dic, item.FullName);
                }
            }
            foreach (var fileInfo in fileInfos)
            {
                if (fileInfo.Name.EndsWith("mp3"))
                {
                    var info = MediaHelper.GetAudioInfo(fileInfo.FullName, "d:/temp/UploadPic");
                    if (!dic.ContainsKey(info.MusicWYYId))
                    {
                        dic.Add(info.MusicWYYId, info); 
                    }
                }
            }
        }

        public static Dictionary<long, string> GetAllLyricInfoInDir(Dictionary<long, string> dic,string filePath)
        { 
            GetAllLyricInfoInDir(ref dic, filePath);
            return dic;
        }

        private static void GetAllLyricInfoInDir(ref Dictionary<long, string> dic, string filePath)
        {
            var directoryInfo = new DirectoryInfo(filePath);
            var fileInfos = directoryInfo.GetFiles();
            var directoryInfos = directoryInfo.GetDirectories();
            if (directoryInfos.Length > 0)
            {
                foreach (var item in directoryInfos)
                {
                    GetAllLyricInfoInDir(ref dic, item.FullName);
                }
            }
            foreach (var fileInfo in fileInfos)
            {
                long res;
                if (long.TryParse(fileInfo.Name, out res))
                {
                    if (!dic.ContainsKey(res))
                    {
                        dic.Add(res, fileInfo.FullName); 
                    }
                } 
            }
        }

        public static Dictionary<long, AudioInfo> MergeLyricIntoAudio(Dictionary<long, AudioInfo> audioDic, Dictionary<long, string> lyricDic)
        {
            foreach (var item in lyricDic)
            {
                // 如果歌词有对应的歌曲
                if (audioDic.ContainsKey(item.Key))
                {
                    audioDic[item.Key].LyricFilePath= item.Value;
                }
            }
            return audioDic;
        }
        // 解密163key
        public static string DecryptStringFromBase64Aes(string base64EncryptedText, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] encryptedBytes = Convert.FromBase64String(base64EncryptedText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var ms = new System.IO.MemoryStream(encryptedBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new System.IO.StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
