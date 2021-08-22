using eShopSolution.Ultilities.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public class BaseApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        protected async Task<TResponse> GetAsync<TResponse>(string url)
        {
            using (var client = new HttpClient())
            {
                var sessions = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.Token);
                client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
                var response = await client.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    TResponse objList = (TResponse)JsonConvert.DeserializeObject(body, typeof(TResponse));
                    return objList;
                }
                else
                {
                    return JsonConvert.DeserializeObject<TResponse>(body);
                }
            }
        }

        protected async Task<TResponse> PostAsync<TResponse>(string url, object request)
        {
            using (var client = new HttpClient())
            {
                var requestJSON = JsonConvert.SerializeObject(request);
                var buffer = Encoding.UTF8.GetBytes(requestJSON);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var response = await client.PostAsync(url, byteContent);
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    TResponse objList = (TResponse)JsonConvert.DeserializeObject(body, typeof(TResponse));
                    return objList;
                }
                else
                {
                    return JsonConvert.DeserializeObject<TResponse>(body);
                }
            }
        }

        protected async Task<TResponse> PutAsync<TResponse>(string url, object request)
        {
            using (var client = new HttpClient())
            {
                var requestJSON = JsonConvert.SerializeObject(request);
                var buffer = Encoding.UTF8.GetBytes(requestJSON);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
                var response = await client.PutAsync(url, byteContent);
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    TResponse objList = (TResponse)JsonConvert.DeserializeObject(result, typeof(TResponse));
                    return objList;
                }
                else
                {
                    return JsonConvert.DeserializeObject<TResponse>(result);
                }
            }
        }

        protected async Task<TResponse> DeleteAsync<TResponse>(string url)
        {
            using (var client = new HttpClient())
            {
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
                var response = await client.DeleteAsync(url);
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    TResponse objList = (TResponse)JsonConvert.DeserializeObject(body, typeof(TResponse));
                    return objList;
                }
                else
                {
                    return JsonConvert.DeserializeObject<TResponse>(body);
                }
            }
        }
    }
}