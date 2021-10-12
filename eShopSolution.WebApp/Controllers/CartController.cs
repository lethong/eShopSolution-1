using eShopSolution.ApiIntegration.Services;
using eShopSolution.Ultilities.Constants;
using eShopSolution.ViewModels.Sales;
using eShopSolution.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.WebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductApiClient _productApiClient;
        private readonly IOrderApiClient _orderApiClient;

        public CartController(IProductApiClient productApiClient, IOrderApiClient orderApiClient)
        {
            _productApiClient = productApiClient;
            _orderApiClient = orderApiClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return View(GetCheckoutViewModel());
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Checkout([FromForm] CheckoutViewModel request)
        {
            if (!ModelState.IsValid)
                return View(request);
            var model = GetCheckoutViewModel();
            var orderDetails = new List<OrderDetailViewModel>();
            foreach (var item in model.CartItems)
            {
                orderDetails.Add(new OrderDetailViewModel()
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }
            var checkoutRequest = new CheckoutRequest()
            {
                Name = request.Checkout.Name,
                Address = request.Checkout.Address,
                Email = request.Checkout.Email,
                PhoneNumber = request.Checkout.PhoneNumber,
                OrderDetails = orderDetails
            };
            //Add to API
            var result = await _orderApiClient.CreateOrder(checkoutRequest);
            if (result)
            {
                TempData["SuccessMsg"] = "Thêm mới đơn hàng thành công";
                HttpContext.Session.Remove(SystemConstants.CartSession);
            }
            else
            {
                TempData["SuccessMsg"] = "Thêm mới đơn hàng thất bại";
            }
            return View(request);
        }

        [HttpGet]
        public IActionResult GetListItems()
        {
            var session = HttpContext.Session.GetString(SystemConstants.CartSession);
            List<CartItemViewModel> currentCart = new List<CartItemViewModel>();
            if (session != null)
                currentCart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(HttpContext.Session.GetString(SystemConstants.CartSession));

            return Ok(currentCart);
        }

        public async Task<IActionResult> AddToCart(int id, string languageId)
        {
            var product = await _productApiClient.GetById(id, languageId);
            var session = HttpContext.Session.GetString(SystemConstants.CartSession);
            List<CartItemViewModel> currentCart = new List<CartItemViewModel>();
            if (session != null)
                currentCart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(HttpContext.Session.GetString(SystemConstants.CartSession));

            int quantity = 1;
            if (currentCart.Any(x => x.ProductId == id))
            {
                var updateItem = currentCart.FirstOrDefault(x => x.ProductId == id);
                quantity = updateItem.Quantity + 1;
                updateItem.Quantity = quantity;
            }
            else
            {
                var cartItem = new CartItemViewModel()
                {
                    ProductId = id,
                    Description = product.Description,
                    Name = product.Name,
                    Image = product.ThumbnailImage,
                    Quantity = quantity,
                    Price = product.Price
                };
                currentCart.Add(cartItem);
            }
            HttpContext.Session.SetString(SystemConstants.CartSession, JsonConvert.SerializeObject(currentCart));
            return Ok(currentCart);
        }

        public IActionResult UpdateCart(int id, int quantity)
        {
            var session = HttpContext.Session.GetString(SystemConstants.CartSession);
            List<CartItemViewModel> currentCart = new List<CartItemViewModel>();
            if (session != null)
                currentCart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(HttpContext.Session.GetString(SystemConstants.CartSession));

            foreach (var item in currentCart)
            {
                if (item.ProductId == id)
                {
                    if (quantity == 0)
                    {
                        currentCart.Remove(item);
                    }
                    item.Quantity = quantity;
                    break;
                }
            }
            HttpContext.Session.SetString(SystemConstants.CartSession, JsonConvert.SerializeObject(currentCart));
            return Ok(currentCart);
        }

        private CheckoutViewModel GetCheckoutViewModel()
        {
            var session = HttpContext.Session.GetString(SystemConstants.CartSession);
            List<CartItemViewModel> currentCart = new List<CartItemViewModel>();
            if (session != null)
                currentCart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(HttpContext.Session.GetString(SystemConstants.CartSession));
            var checkoutViewModel = new CheckoutViewModel()
            {
                CartItems = currentCart,
                Checkout = new CheckoutRequest()
            };

            return checkoutViewModel;
        }
    }
}