using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Zack.JWT;
using Zack.Commons;
using FileService.Domain;

namespace FileService.SDK.NETCore
{
    public class FileServiceClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly Uri serverRoot;
        private readonly JWTOptions optionsSnapshot;
        private readonly ITokenService tokenService;

        public FileServiceClient(IHttpClientFactory httpClientFactory, Uri serverRoot, JWTOptions optionsSnapshot, ITokenService tokenService)
        {
            this.httpClientFactory = httpClientFactory;
            this.serverRoot = serverRoot;
            this.optionsSnapshot = optionsSnapshot;
            this.tokenService = tokenService;
        }
        public Task<FileExistsResponse> FileExistsAsync(UploadedItemType type, long fileSize, string sha256Hash, CancellationToken stoppingToken = default)
        {
            string relativeUrl = FormattableStringHelper.BuildUrl($"Uploader/FileExists??type={type}&fileSize={fileSize}&sha256Hash={sha256Hash}");
            Uri requestUri = new Uri(serverRoot, relativeUrl);
            var httpClient = httpClientFactory.CreateClient();
            return httpClient.GetJsonAsync<FileExistsResponse>(requestUri, stoppingToken)!;
        }

        string BuildToken()
        {
            //因为JWT的key等机密信息只有服务器端知道，因此可以这样非常简单的读到配置
            Claim claim1 = new Claim(ClaimTypes.Role, "Admin");
            Claim claim2 = new Claim(ClaimTypes.NameIdentifier, "admin"); 
            return tokenService.BuildToken(new Claim[] { claim1,claim2 }, optionsSnapshot);
        }

        public async Task<Uri> UploadAsync(FileInfo file, UploadedItemType type, CancellationToken stoppingToken = default)
        {
            string token = BuildToken();
            using MultipartFormDataContent content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(file.OpenRead());
            content.Add(fileContent, "file", file.Name);
            var httpClient = httpClientFactory.CreateClient();
            Uri requestUri = new Uri(serverRoot + $"/Uploader/Upload?uploadedItemType={type}");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var respMsg = await httpClient.PostAsync(requestUri, content, stoppingToken);
            if (!respMsg.IsSuccessStatusCode)
            {
                string respString = await respMsg.Content.ReadAsStringAsync(stoppingToken);
                throw new HttpRequestException($"上传失败，状态码：{respMsg.StatusCode}，响应报文：{respString}");
            }
            else
            {
                string respString = await respMsg.Content.ReadAsStringAsync(stoppingToken);
                return respString.ParseJson<Uri>()!;
            }
        }
    }
}
