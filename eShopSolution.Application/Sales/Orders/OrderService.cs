using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Sales;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Sales.Orders
{
    public class OrderService : IOrderService
    {
        private readonly EShopDbContext _context;

        public OrderService(EShopDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(CheckoutRequest request)
        {
            var order = new Order()
            {
                OrderDate = DateTime.Now.Date,
                ShipName = request.Name,
                ShipAddress = request.Address,
                ShipEmail = request.Email,
                ShipPhoneNumber = request.PhoneNumber
            };
            var orderDetails = new List<OrderDetail>();
            foreach (var item in request.OrderDetails)
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }
            order.OrderDetails = orderDetails;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order.Id;
        }
    }
}