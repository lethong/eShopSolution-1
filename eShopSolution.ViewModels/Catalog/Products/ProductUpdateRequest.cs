using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Products
{
    public class ProductUpdateRequest
    {
        public int Id { get; set; }

        [Display(Name = "Tên sản phẩm")]
        public string Name { set; get; }

        [Display(Name = "Mô tả")]
        public string Description { set; get; }

        [Display(Name = "Chi tiết")]
        public string Details { set; get; }

        [Display(Name = "Mô tả Seo")]
        public string SeoDescription { set; get; }

        [Display(Name = "Tiêu đề Seo")]
        public string SeoTitle { set; get; }

        [Display(Name = "Seo Alias")]
        public string SeoAlias { get; set; }

        public string LanguageId { set; get; }

        [Display(Name = "Hình đại diện")]
        public IFormFile ThumbnailImage { get; set; }

        [Display(Name = "Sản phẩm nổi bật")]
        public bool IsFeatured { get; set; }
    }
}