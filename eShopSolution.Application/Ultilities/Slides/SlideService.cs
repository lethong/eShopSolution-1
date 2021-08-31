using eShopSolution.Data.EF;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using eShopSolution.Data.Enums;
using eShopSolution.ViewModels.Ultilities.Slides;

namespace eShopSolution.Application.Ultilities.Slides
{
    public class SlideService : ISlideService
    {
        private readonly EShopDbContext _context;

        public SlideService(EShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<SlideViewModel>> GetAll()
        {
            // 1.Select join
            var data = await _context.Slides.Where(x => x.Status == Status.Active)
                .Select(x => new SlideViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Url = x.Url,
                    Image = x.Image,
                    SortOrder = x.SortOrder,
                }).ToListAsync();
            return data;
        }
    }
}