
using Microsoft.OpenApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace WYYMusicUploader
{
    public static class HttpHelper
    {  
        public static async Task<(HttpResult,string?)> GetResAsync(this HttpClient client, UploadedItemType type, long fileSizeInByte, string sha256Hash, CancellationToken cancellationToken = default)
        {
            // 回复为 public record FileExistsResponse(bool IsExists, Uri? Url);
            Uri apiUri = new Uri($"http://127.0.0.1:5070/Uploader/FileExists?type={(int)type}&fileSizeInBytes={fileSizeInByte}&sha256Hash={sha256Hash}");
            HttpResponseMessage response =await client.GetAsync(apiUri, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var resJson =await response.Content.ReadAsStringAsync();
                using JsonDocument document = JsonDocument.Parse(resJson);
                JsonElement root = document.RootElement;
                
                if (root.GetProperty("isExists").GetBoolean())
                { 
                    return (HttpResult.Exist, root.GetProperty("url").GetString());
                }
                return (HttpResult.Success, null);
            }else
            {
                return (HttpResult.Fail, null);
            }
        }


        public static async Task<(HttpResult, string?)> UploadAsync(this HttpClient client, UploadedItemType type, HttpContent content, CancellationToken cancellationToken = default)
        {
            Uri baseUri = new Uri($"http://localhost:5070/Uploader/Upload?uploadedItemType={(int)type}");

            HttpResponseMessage response = await client.PostAsync(baseUri, content, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var url = await response.Content.ReadAsStringAsync();
                url = url.Substring(1,url.Length - 2); // 去除包裹的 ""
                return (HttpResult.Success, url);
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return (HttpResult.Exist, null);
            }
            else
            {
                return (HttpResult.Fail, null);
            }
        }

        public static async Task<(HttpResult, long?)> PostAsync(this HttpClient client, CreateItemType type, HttpContent content, CancellationToken cancellationToken = default)
        {
            Uri baseUri = new Uri($"http://127.0.0.1:5156/api/{type}");

            HttpResponseMessage response = await client.PostAsync(baseUri, content, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                long id = long.Parse(body); 
                return (HttpResult.Success, id);
            } 
            else
            {
                return (HttpResult.Fail, null);
            }
        }
        public static string ToHashString(byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                stringBuilder.Append(bytes[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
}
