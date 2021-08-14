using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Http;
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
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResult<string>> Authenticate(LoginRequest request)
        {
            using (var client = new HttpClient())
            {
                var requestJSON = JsonConvert.SerializeObject(request);
                var buffer = Encoding.UTF8.GetBytes(requestJSON);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var response = await client.PostAsync("/api/users/authenticate", byteContent);
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    return JsonConvert.DeserializeObject<ApiErrorResult<string>>(await response.Content.ReadAsStringAsync());
                }
            }
        }

        public async Task<ApiResult<UserViewModel>> GetById(Guid id)
        {
            using (var client = new HttpClient())
            {
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
                var response = await client.GetAsync($"/api/users/{id}");
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiSuccessResult<UserViewModel>>(body);
                }
                else
                {
                    return JsonConvert.DeserializeObject<ApiErrorResult<UserViewModel>>(body);
                }
            }
        }

        public async Task<ApiResult<PagedResult<UserViewModel>>> GetUsersPagings(GetUserPagingRequest request)
        {
            using (var client = new HttpClient())
            {
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
                var response = await client.GetAsync($"/api/users/paging?pageIndex={request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}");
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<ApiSuccessResult<PagedResult<UserViewModel>>>(body);
                    return users;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<ApiResult<bool>> RegisterUser(RegisterRequest request)
        {
            using (var client = new HttpClient())
            {
                var requestJSON = JsonConvert.SerializeObject(request);
                var buffer = Encoding.UTF8.GetBytes(requestJSON);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var response = await client.PostAsync($"/api/users", byteContent);
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);
                }
                else
                {
                    return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
                }
            }
        }

        public async Task<ApiResult<bool>> UpdateUser(Guid id, UserUpdateRequest request)
        {
            using (var client = new HttpClient())
            {
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                var requestJSON = JsonConvert.SerializeObject(request);
                var buffer = Encoding.UTF8.GetBytes(requestJSON);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
                var response = await client.PutAsync($"/api/users/{id}", byteContent);
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);
                }
                else
                {
                    return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
                }
            }
        }
    }
}