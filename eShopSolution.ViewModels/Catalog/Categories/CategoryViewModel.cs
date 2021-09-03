using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Categories
{
    public class CategoryViewModel
    {
        public int Id { set; get; }
        public string Name { get; set; }
        public string LanguageId { get; set; }
        public int? ParentId { get; set; }
    }
}