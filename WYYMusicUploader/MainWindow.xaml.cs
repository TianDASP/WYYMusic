using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TagLib.Ape;
using Microsoft.WindowsAPICodePack.Dialogs;
using TagLib.Mpeg;
using System.Text.Json;
using System.Security.Policy;

namespace WYYMusicUploader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<long, AudioInfo> dic = new Dictionary<long, AudioInfo>();
        private Dictionary<long, string> lyricDicInfo = new Dictionary<long, string>();
        private ObservableCollection<AudioInfo> audios = new ObservableCollection<AudioInfo>(); 
        private static HttpClient client = new HttpClient();
        private string token;
        public Dictionary<long, AudioInfo> Dic
        {
            get { return dic; }
            set { dic = value; }
        }

        public void GetToken(string value1)
        {
            token = value1;
        }
        public MainWindow()
        {
            InitializeComponent();
            Init();
            this.grid1.ItemsSource = audios;
        }
        private void Init()
        {

            string[] args = new string[1] { "D:\\CloudMusic" };
            if (args.Length > 0)
            {
                string filePath = args[0];
                // 判断是文件夹
                if (IsDir(filePath))
                {
                    dic = MediaHelper.GetAllMediaInfoInDir(dic, filePath);
                }
                else
                {
                    var d = MediaHelper.GetAudioInfo(filePath, "d:/temp/UploadPic");
                    dic.Add(d.MusicWYYId, d);
                }
            }

            lyricDicInfo = MediaHelper.GetAllLyricInfoInDir(lyricDicInfo, "C:\\Users\\Zeng\\AppData\\Local\\NetEase\\CloudMusic\\webdata\\lyric");
            dic = MediaHelper.MergeLyricIntoAudio(dic, lyricDicInfo);
            //var itemsource = dic.Where(x=>string.IsNullOrEmpty( x.Value.LyricFilePath));
            // this.grid1.ItemsSource = dic;
            foreach (var item in dic)
            {
                audios.Add(item.Value);
            }
            //audios.Add(dic.First().Value);  
            // 预热 httpclient
            client.SendAsync(new HttpRequestMessage
            {
                Method = new HttpMethod("HEAD"),
                RequestUri = new Uri("http://www.baidu.com")
            }).Result.EnsureSuccessStatusCode();
        }

        // 扫描文件夹内的mp3
        private void AddAudioDir(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Title = "请选择您要导入的音频文件夹";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = dialog.FileName;
                dic = MediaHelper.GetAllMediaInfoInDir(dic, path);
                dic = MediaHelper.MergeLyricIntoAudio(dic, lyricDicInfo);
                audios.Clear();
                foreach (var item in dic)
                {
                    audios.Add(item.Value);
                }
            }
        }

        //扫描文件夹内的Lyric
        private void AddLyricDir(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Title = "请选择您要导入的音频文件夹";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = dialog.FileName;
                lyricDicInfo = MediaHelper.GetAllLyricInfoInDir(lyricDicInfo, path);
                dic = MediaHelper.MergeLyricIntoAudio(dic, lyricDicInfo);
                audios.Clear();
                foreach (var item in dic)
                {
                    audios.Add(item.Value);
                }
            }
        }

        private async void Upload(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(token))
            {
                MessageBox.Show("请先登录");
                return;
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            List<Task> tasks = new List<Task>(); 
                using SHA256 sHA = SHA256.Create();

            
            //dic = (Dictionary<long, AudioInfo>)dic.Take(1);
            //await Task.Delay(1000);
            foreach (var item in audios)
            {
                tasks.Clear(); 
                using var audioStream = new FileStream(item.AudioFilePath, FileMode.Open);
                using var lyricStream = new FileStream(item.LyricFilePath, FileMode.Open);
                using var picStream = new FileStream(item.AlbumPicPath, FileMode.Open);

                var sha256Hash1 = HttpHelper.ToHashString(sHA.ComputeHash(audioStream));
                long fileSizeInBytes1 = audioStream.Length;
                var sha256Hash2 = HttpHelper.ToHashString(sHA.ComputeHash(lyricStream));
                long fileSizeInBytes2 = lyricStream.Length;
                var sha256Hash3 = HttpHelper.ToHashString(sHA.ComputeHash(picStream));
                long fileSizeInBytes3 = picStream.Length;

                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    (HttpResult res, string? url) = await client.GetResAsync(UploadedItemType.Audio, fileSizeInBytes1, sha256Hash1);
                    switch (res)
                    { //  ✓  ✗   ！
                        case HttpResult.Success:
                            item.res[0] = "◌";
                            audioStream.Position = 0;
                            await UploadAsync(UploadedItemType.Audio, item, audioStream, client);
                            break;
                        case HttpResult.Fail:
                            item.res[0] = "✗";
                            break;
                        case HttpResult.Exist:
                            item.res[0] = "!";
                            item.AudioRemoteUrl = url!;
                            break;
                    }
                    //await Task.Delay(Random.Shared.Next(20, 40));
                }));
                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    (HttpResult res, string? url) = await client.GetResAsync(UploadedItemType.Lyric, fileSizeInBytes2, sha256Hash2);
                    switch (res)
                    { //  ✓  ✗   ！
                        case HttpResult.Success:
                            item.res[1] = "◌";
                            lyricStream.Position = 0;
                            await UploadAsync(UploadedItemType.Lyric, item, lyricStream, client);
                            break;
                        case HttpResult.Fail:
                            item.res[1] = "✗";
                            break;
                        case HttpResult.Exist:
                            item.res[1] = "!";
                            item.LyricRemoteUrl = url!;
                            break;
                    }
                }));
                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    (HttpResult res, string? url) = await client.GetResAsync(UploadedItemType.Pic, fileSizeInBytes3, sha256Hash3);
                    switch (res)
                    { //  ✓  ✗   ！
                        case HttpResult.Success:
                            item.res[2] = "◌";
                            picStream.Position = 0;
                            await UploadAsync(UploadedItemType.Pic, item, picStream, client); 
                            break;
                        case HttpResult.Fail:
                            item.res[2] = "✗";
                            break;
                        case HttpResult.Exist:
                            item.res[2] = "!";
                            item.PicRemoteUrl = url!;
                            break;
                    }
                }));
                await Task.WhenAll(tasks);
                await Task.Delay(800);
                if (item.res[0] != "✓" || item.res[1] != "✓" || string.IsNullOrEmpty(item.PicRemoteUrl) || string.IsNullOrEmpty(item.AudioRemoteUrl) || string.IsNullOrEmpty(item.LyricRemoteUrl))
                {
                    continue;
                }
                // 创建实体-Album -> Artist -> Music
                var albumcls = new { name = item.AlbumName, albumPicUrl = item.PicRemoteUrl, wyyId = item.AlbumWYYId };
                (HttpResult albumCreationRes, long? albumId) = await PostAsync(CreateItemType.Album, JsonSerializer.Serialize(albumcls), client);
                if (albumCreationRes != HttpResult.Success)
                {
                    continue;
                }
                item.res[3] = "✓";
                // 继续创建 Artists
                var artistNum = item.ArtistsName.Count;
                List<long> artsitIds = new List<long>();
                for (int i = 0; i < artistNum; i++)
                {
                    var artistcls = new { name = item.ArtistsName[i], albumId = albumId, wyyId = item.ArtistsWYYId[i] };
                    (HttpResult artistCreationRes, long? artistId) = await PostAsync(CreateItemType.Artist, JsonSerializer.Serialize(artistcls), client);
                    if (artistCreationRes != HttpResult.Success) { continue; }
                    artsitIds.Add((long)artistId!);
                }
                if (artsitIds.Any(x => x > 0))
                {
                    item.res[4] = "✓";
                    // 继续创建Music
                    var musiccls = new
                    {
                        name = item.MusicName,
                        albumId = albumId,
                        duration = item.DurationInMilliSecond,
                        sequence = item.Sequence,
                        artistIds = artsitIds,
                        audioUrl = item.AudioRemoteUrl,
                        lyricUrl = item.LyricRemoteUrl,
                        wyyId = item.MusicWYYId
                    };
                    (HttpResult musicCreationRes, long? musicId) = await PostAsync(CreateItemType.Music, JsonSerializer.Serialize(musiccls), client);
                    if (musicCreationRes == HttpResult.Success)
                    {
                        item.res[5] = "✓";
                    }
                }
                await Task.Delay(800);
            }
        }

        public static  Task<(HttpResult, long?)> PostAsync(CreateItemType type,string body, HttpClient client)
        { 

            var httpContent = new StringContent(body, Encoding.UTF8, "application/json");
            return  client.PostAsync(type, httpContent);
            
        }

        public static async Task<HttpResult> UploadAsync(UploadedItemType type, AudioInfo item, Stream stream, HttpClient client)
        {
            using (var content = new MultipartFormDataContent())
            {
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                var fileContent = new ByteArrayContent(memoryStream.ToArray());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                string FileName = type switch
                {
                    UploadedItemType.Audio => System.IO.Path.GetFileName(item.AudioFilePath),
                    UploadedItemType.Lyric => System.IO.Path.GetFileName(item.LyricFilePath)+".json",
                    UploadedItemType.Pic => System.IO.Path.GetFileName(item.AlbumPicPath),
                    _ => throw new NotImplementedException()
                }; 
                content.Add(fileContent, "file", FileName);
                (HttpResult res, string? url) = await client.UploadAsync(type, content);
                switch (res)
                { //  ✓  ✗   ！
                    case HttpResult.Success:
                        item.res[(int)type] = "✓";
                        SetUrl(item, type, url!);
                        break;
                    case HttpResult.Fail:
                        item.res[(int)type] = "✗";
                        break;
                    case HttpResult.Exist:
                        item.res[(int)type] = "!";
                        break;
                }
                return res;
            }
        }

        private static void SetUrl(AudioInfo audioInfo, UploadedItemType type, string url)
        {
            switch (type)
            {
                case UploadedItemType.Audio:
                    audioInfo.AudioRemoteUrl = url;
                    break;
                case UploadedItemType.Lyric:
                    audioInfo.LyricRemoteUrl = url;
                    break;
                case UploadedItemType.Pic:
                    audioInfo.PicRemoteUrl = url;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 判断目标是文件夹还是目录(目录包括磁盘)
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <returns>返回true为一个文件夹，返回false为一个文件</returns>
        public static bool IsDir(string filePath)
        {
            if (Directory.Exists(filePath))
            {
                return true;
            }
            return false;
        }

        private void CreateLoginWindow(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.getTokenHandler = GetToken;
            loginWindow.client = client;
            loginWindow.ShowDialog();
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            this.token = "";
        }
    }
}
