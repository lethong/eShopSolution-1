using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.Ultilities.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using eShopSolution.Application.Common;
using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.Ultilities.Constants;
using eShopSolution.ViewModels.Catalog.Categories;

namespace eShopSolution.Application.Catalog.Products
{
    public class ProductService : IProductService
    {
        private readonly EShopDbContext _context;
        private readonly IStorageService _storageService;

        public ProductService(EShopDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task AddViewCount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            product.ViewCount += 1;
            await _context.SaveChangesAsync();
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            var languages = _context.Languages;
            var translations = new List<ProductTranslation>();
            foreach (var language in languages)
            {
                if (language.Id == request.LanguageId)
                {
                    translations.Add(new ProductTranslation()
                    {
                        Name = request.Name,
                        Description = request.Description,
                        Details = request.Details,
                        SeoDescription = request.SeoDescription,
                        SeoAlias = request.SeoAlias,
                        SeoTitle = request.SeoTitle,
                        LanguageId = request.LanguageId
                    });
                }
                else
                {
                    translations.Add(new ProductTranslation()
                    {
                        Name = SystemConstants.ProductConstants.NA,
                        Description = SystemConstants.ProductConstants.NA,
                        SeoAlias = SystemConstants.ProductConstants.NA,
                        LanguageId = language.Id
                    });
                }
            }
            var product = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = 0,
                DateCreated = DateTime.Now,
                ProductTranslations = translations,
                IsFeatured = request.IsFeatured
            };

            //Save Image
            if (request.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption = "Thumbnail image",
                        DateCreated = DateTime.Now,
                        FileSize = request.ThumbnailImage.Length,
                        ImagePath = await this.SaveFile(request.ThumbnailImage),
                        IsDefault = true,
                        SortOrder = 1
                    }
                };
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        public async Task<int> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Cannot find a product: {productId}");
            var images = _context.ProductImages.Where(i => i.ProductId == productId);
            foreach (var image in images)
            {
                await _storageService.DeleteFileAsync(image.ImagePath);
            }
            _context.Products.Remove(product);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            var productTranslations = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == request.Id && x.LanguageId == request.LanguageId);

            if (product == null || productTranslations == null) throw new EShopException($"Can not find a product with id: {request.Id}");
            product.IsFeatured = request.IsFeatured;
            productTranslations.Name = request.Name;
            productTranslations.SeoAlias = request.SeoAlias;
            productTranslations.SeoDescription = request.SeoDescription;
            productTranslations.SeoTitle = request.SeoTitle;
            productTranslations.Description = request.Description;
            productTranslations.Details = request.Details;

            //Save Image
            if (request.ThumbnailImage != null)
            {
                var thumbnailImage = _context.ProductImages.FirstOrDefault(i => i.IsDefault == true && i.ProductId == request.Id);
                if (thumbnailImage != null)
                {
                    thumbnailImage.FileSize = request.ThumbnailImage.Length;
                    thumbnailImage.ImagePath = await this.SaveFile(request.ThumbnailImage);
                    _context.ProductImages.Update(thumbnailImage);
                }
                else
                {
                    product.ProductImages = new List<ProductImage>()
                        {
                            new ProductImage()
                            {
                                Caption = "Thumbnail image",
                                DateCreated = DateTime.Now,
                                FileSize = request.ThumbnailImage.Length,
                                ImagePath = await this.SaveFile(request.ThumbnailImage),
                                IsDefault = true,
                                SortOrder = 1
                            }
                        };
                }
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<ProductViewModel> GetById(int productId, string languageId)
        {
            var product = await _context.Products.FindAsync(productId);
            var productTranslation = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == productId && x.LanguageId == languageId);
            var productImage = await _context.ProductImages.FirstOrDefaultAsync(x => x.ProductId == productId && x.IsDefault == true);
            var categoriesquery = from c in _context.Categories
                                  join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                                  join pic in _context.ProductInCategories on c.Id equals pic.CategoryId
                                  where pic.ProductId == productId && ct.LanguageId == languageId
                                  select new { c, ct };
            var categories = await categoriesquery.Select(x => new CategoryViewModel()
            {
                Id = x.c.Id,
                Name = x.ct.Name,
                LanguageId = x.ct.LanguageId,
                ParentId = x.c.ParentId
            }).ToListAsync();

            var productViewModel = new ProductViewModel()
            {
                Id = product.Id,
                DateCreated = product.DateCreated,
                Price = product.Price,
                OriginalPrice = product.OriginalPrice,
                Stock = product.Stock,
                ViewCount = product.ViewCount,
                LanguageId = productTranslation != null ? productTranslation.LanguageId : null,
                Name = productTranslation != null ? productTranslation.Name : null,
                Description = productTranslation != null ? productTranslation.Description : null,
                Details = productTranslation != null ? productTranslation.Details : null,
                SeoAlias = productTranslation != null ? productTranslation.SeoAlias : null,
                SeoDescription = productTranslation != null ? productTranslation.SeoDescription : null,
                SeoTitle = productTranslation != null ? productTranslation.SeoTitle : null,
                IsFeatured = product.IsFeatured,
                ThumbnailImage = productImage != null ? productImage.ImagePath : "",
                Categories = categories
            };

            return productViewModel;
        }

        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Can not find a product with id: {productId}");
            product.Price = newPrice;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStock(int productId, int addedQuantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Can not find a product with id: {productId}");
            product.Stock += addedQuantity;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request)
        {
            // 1.Select join
            var query = from p in _context.Products
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into tmppi
                        from pi in tmppi.DefaultIfEmpty()
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId into tmppic
                        from pic in tmppic.DefaultIfEmpty()
                        join c in _context.Categories on pic.CategoryId equals c.Id into tmpc
                        from c in tmpc.DefaultIfEmpty()
                        where pt.LanguageId == request.LanguageId && (pi == null || pi.IsDefault == true)
                        select new { p, pt, pic, pi };

            // 2.Filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.pt.Name.Contains(request.Keyword));
            if (request.CategoryId.HasValue && request.CategoryId > 0)
            {
                query = query.Where(p => p.pic.CategoryId == request.CategoryId);
            }
            // 3.Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductViewModel()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt == null ? SystemConstants.ProductConstants.NA : x.pt.Description,
                    Details = x.pt == null ? SystemConstants.ProductConstants.NA : x.pt.Details,
                    LanguageId = x.pt == null ? SystemConstants.ProductConstants.NA : x.pt.LanguageId,
                    Price = x.p.Price,
                    OriginalPrice = x.p.OriginalPrice,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    SeoAlias = x.pt == null ? SystemConstants.ProductConstants.NA : x.pt.SeoAlias,
                    SeoDescription = x.pt == null ? SystemConstants.ProductConstants.NA : x.pt.SeoDescription,
                    SeoTitle = x.pt == null ? SystemConstants.ProductConstants.NA : x.pt.SeoTitle,
                    ThumbnailImage = x.pi.ImagePath
                }).ToListAsync();

            // 4.Select and Projection
            var PageResult = new PagedResult<ProductViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };
            return PageResult;
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }

        public async Task<int> AddImage(int productId, ProductImageCreateRequest request)
        {
            var productImage = new ProductImage()
            {
                Caption = request.Caption,
                DateCreated = DateTime.Now,
                IsDefault = request.IsDefault,
                ProductId = productId,
                SortOrder = request.SortOrder
            };
            //Save Image
            if (request.ImageFile != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync();
            return productImage.Id;
        }

        public async Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request)
        {
            var productImage = await _context.ProductImages.FindAsync(imageId);
            if (productImage == null) throw new EShopException($"Cannot find an image with id {imageId}");
            //Save Image
            if (request.ImageFile != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            _context.ProductImages.Update(productImage);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoveImage(int imageId)
        {
            var productImage = await _context.ProductImages.FindAsync(imageId);
            if (productImage == null) throw new EShopException($"Cannot find an image with id {imageId}");
            _context.ProductImages.Remove(productImage);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<ProductImageViewModel>> GetListImages(int productId)
        {
            return await _context.ProductImages.Where(x => x.ProductId == productId)
                .Select(i => new ProductImageViewModel()
                {
                    Caption = i.Caption,
                    DateCreated = i.DateCreated,
                    FileSize = i.FileSize,
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    IsDefault = i.IsDefault,
                    ProductId = i.ProductId,
                    SortOrder = i.SortOrder
                }).ToListAsync();
        }

        public async Task<ProductImageViewModel> GetImageById(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null) throw new EShopException($"Cannot find an image with id {imageId}");
            var viewModel = new ProductImageViewModel()
            {
                Caption = image.Caption,
                DateCreated = image.DateCreated,
                FileSize = image.FileSize,
                Id = image.Id,
                ImagePath = image.ImagePath,
                IsDefault = image.IsDefault,
                ProductId = image.ProductId,
                SortOrder = image.SortOrder
            };
            return viewModel;
        }

        public async Task<PagedResult<ProductViewModel>> GetAllByCategoryId(string languageId, GetPublicProductPagingRequest request)
        {
            // 1.Select join
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        where pt.LanguageId == languageId
                        select new { p, pt, pic };
            // 2.Filter
            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
            {
                query = query.Where(p => p.pic.CategoryId == request.CategoryId);
            }
            // 3.Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductViewModel()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    Price = x.p.Price,
                    OriginalPrice = x.p.OriginalPrice,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle
                }).ToListAsync();

            // 4.Select and Projection
            var PageResult = new PagedResult<ProductViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };
            return PageResult;
        }

        public async Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return new ApiErrorResult<bool>($"Sản phẩm với id {id} không tồn tại");
            }
            foreach (var category in request.Categories)
            {
                var productInCategory = await _context.ProductInCategories.FirstOrDefaultAsync(x => x.CategoryId == int.Parse(category.Id) && x.ProductId == id);
                if (productInCategory != null && category.Selected == false)
                {
                    _context.ProductInCategories.Remove(productInCategory);
                }
                else if (productInCategory == null && category.Selected == true)
                {
                    await _context.ProductInCategories.AddAsync(new ProductInCategory()
                    {
                        ProductId = id,
                        CategoryId = int.Parse(category.Id)
                    });
                }
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>();
        }

        public async Task<List<ProductViewModel>> GetFeaturedProducts(string languageId, int take)
        {
            // 1.Select join
            var query = from p in _context.Products
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into tmppi
                        from pi in tmppi.DefaultIfEmpty()
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId into tmppic
                        from pic in tmppic.DefaultIfEmpty()
                        join c in _context.Categories on pic.CategoryId equals c.Id into tmpc
                        from c in tmpc.DefaultIfEmpty()
                        where pt.LanguageId == languageId && p.IsFeatured == true && (pi == null || pi.IsDefault == true)
                        select new { p, pt, pic, pi };

            var data = await query.OrderByDescending(x => x.p.DateCreated).Take(take).Select(x => new ProductViewModel()
            {
                Id = x.p.Id,
                Name = x.pt.Name,
                DateCreated = x.p.DateCreated,
                Description = x.pt.Description,
                Details = x.pt.Details,
                LanguageId = x.pt.LanguageId,
                Price = x.p.Price,
                OriginalPrice = x.p.OriginalPrice,
                Stock = x.p.Stock,
                ViewCount = x.p.ViewCount,
                SeoAlias = x.pt.SeoAlias,
                SeoDescription = x.pt.SeoDescription,
                SeoTitle = x.pt.SeoTitle,
                ThumbnailImage = x.pi.ImagePath
            }).ToListAsync();

            return data;
        }

        public async Task<List<ProductViewModel>> GetLatestProducts(string languageId, int take)
        {
            // 1.Select join
            var query = from p in _context.Products
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into tmppi
                        from pi in tmppi.DefaultIfEmpty()
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId into tmppic
                        from pic in tmppic.DefaultIfEmpty()
                        join c in _context.Categories on pic.CategoryId equals c.Id into tmpc
                        from c in tmpc.DefaultIfEmpty()
                        where pt.LanguageId == languageId && (pi == null || pi.IsDefault == true)
                        select new { p, pt, pic, pi };

            var data = await query.OrderByDescending(x => x.p.DateCreated).Take(take).Select(x => new ProductViewModel()
            {
                Id = x.p.Id,
                Name = x.pt.Name,
                DateCreated = x.p.DateCreated,
                Description = x.pt.Description,
                Details = x.pt.Details,
                LanguageId = x.pt.LanguageId,
                Price = x.p.Price,
                OriginalPrice = x.p.OriginalPrice,
                Stock = x.p.Stock,
                ViewCount = x.p.ViewCount,
                SeoAlias = x.pt.SeoAlias,
                SeoDescription = x.pt.SeoDescription,
                SeoTitle = x.pt.SeoTitle,
                ThumbnailImage = x.pi.ImagePath
            }).ToListAsync();

            return data;
        }
    }
}