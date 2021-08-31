using eShopSolution.ViewModels.Ultilities.Slides;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopSolution.ApiIntegration.Services
{
    public interface ISlideApiClient
    {
        Task<List<SlideViewModel>> GetAll();
    }
}