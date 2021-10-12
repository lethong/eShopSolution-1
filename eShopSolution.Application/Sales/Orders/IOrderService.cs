using eShopSolution.ViewModels.Sales;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Sales.Orders
{
    public interface IOrderService
    {
        Task<int> Create(CheckoutRequest request);
    }
}