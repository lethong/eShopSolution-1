using eShopSolution.Ultilities.Constants;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace eShopSolution.ApiIntegration.Services
{
    public class ProductApiClient : BaseApiClient, IProductApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, configuration, httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request)
        {
            return await PutAsync<ApiResult<bool>>($"/api/products/{id}/categories", request);
        }

        public async Task<bool> CreateProduct(ProductCreateRequest request)
        {
            using (var client = new HttpClient())
            {
                var sessions = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.Token);
                client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var languageId = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

                var requestContent = new MultipartFormDataContent();
                if (request.ThumbnailImage != null)
                {
                    byte[] data;
                    using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                    {
                        data = br.ReadBytes((int)request.ThumbnailImage.OpenReadStream().Length);
                    }
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    requestContent.Add(bytes, "thumbnailImage", request.ThumbnailImage.FileName);
                }
                requestContent.Add(new StringContent(request.Price.ToString()), "price");
                requestContent.Add(new StringContent(request.OriginalPrice.ToString()), "originalPrice");
                requestContent.Add(new StringContent(request.Stock.ToString()), "stock");
                requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Name) ? "" : request.Name.ToString()), "name");
                requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Description) ? "" : request.Description.ToString()), "description");
                requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Details) ? "" : request.Details.ToString()), "details");
                requestContent.Add(new StringContent(string.IsNullOrEmpty(request.SeoDescription) ? "" : request.SeoDescription.ToString()), "seoDescription");
                requestContent.Add(new StringContent(string.IsNullOrEmpty(request.SeoTitle) ? "" : request.SeoTitle.ToString()), "seoTitle");
                requestContent.Add(new StringContent(request.SeoAlias.ToString()), "seoAlias");
                requestContent.Add(new StringContent(request.IsFeatured.ToString()), "isFeatured");
                requestContent.Add(new StringContent(languageId), "languageId");

                var response = await client.PostAsync($"/api/products/", requestContent);
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> DeleteProduct(int id)
        {
            using (var client = new HttpClient())
            {
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
                var response = await client.DeleteAsync($"/api/products/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<ProductViewModel> GetById(int id, string languageId)
        {
            return await GetAsync<ProductViewModel>($"/api/products/{id}/{languageId}");
        }

        public async Task<List<ProductViewModel>> GetFeaturedProducts(string languageId, int take)
        {
            return await GetAsync<List<ProductViewModel>>($"/api/products/featured/{take}/{languageId}");
        }

        public async Task<List<ProductViewModel>> GetLatestProducts(string languageId, int take)
        {
            return await GetAsync<List<ProductViewModel>>($"/api/products/latest/{take}/{languageId}");
        }

        public async Task<PagedResult<ProductViewModel>> GetPagings(GetManageProductPagingRequest request)
        {
            return await GetAsync<PagedResult<ProductViewModel>>($"/api/products/paging?pageIndex={request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}&languageId={request.LanguageId}&categoryId={request.CategoryId}");
        }

        public async Task<bool> UpdateProduct(ProductUpdateRequest request)
        {
            using (var client = new HttpClient())
            {
                var sessions = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.Token);
                client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var languageId = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

                var requestContent = new MultipartFormDataContent();
                if (request.ThumbnailImage != null)
                {
                    byte[] data;
                    using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                    {
                        data = br.ReadBytes((int)request.ThumbnailImage.OpenReadStream().Length);
                    }
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    requestContent.Add(bytes, "thumbnailImage", request.ThumbnailImage.FileName);
                }
                //requestContent.Add(new StringContent(request.Id.ToString()), "id");

                requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Name) ? "" : request.Name.ToString()), "name");
                requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Description) ? "" : request.Description.ToString()), "description");
                requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Details) ? "" : request.Details.ToString()), "details");
                requestContent.Add(new StringContent(string.IsNullOrEmpty(request.SeoDescription) ? "" : request.SeoDescription.ToString()), "seoDescription");
                requestContent.Add(new StringContent(string.IsNullOrEmpty(request.SeoTitle) ? "" : request.SeoTitle.ToString()), "seoTitle");
                requestContent.Add(new StringContent(string.IsNullOrEmpty(request.SeoAlias) ? "" : request.SeoAlias.ToString()), "seoAlias");
                requestContent.Add(new StringContent(languageId), "languageId");
                requestContent.Add(new StringContent(request.IsFeatured.ToString()), "isFeatured");

                var response = await client.PutAsync($"/api/products/" + request.Id, requestContent);
                return response.IsSuccessStatusCode;
            }
        }
    }
}