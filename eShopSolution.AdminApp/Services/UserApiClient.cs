using eShopSolution.ViewModels.System.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public class UserApiClient : IUserApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> Authenticate(LoginRequest request)
        {
            //var json = JsonConvert.SerializeObject(request);
            //var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            //var client = _httpClientFactory.CreateClient();
            ////client.BaseAddress = new Uri("https://localhost:5001/");
            //var response = await client.PostAsync("​/api/Users/authenticate", httpContent);
            //var token = await response.Content.ReadAsStringAsync();
            using (var client = new HttpClient())
            {
                var requestJSON = JsonConvert.SerializeObject(request);
                var buffer = Encoding.UTF8.GetBytes(requestJSON);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client.BaseAddress = new Uri("https://localhost:5001");
                var response = await client.PostAsync("/api/Users/authenticate", byteContent);
                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    return token;
                }
                else
                {
                    return "";
                }
            }
        }
    }
}