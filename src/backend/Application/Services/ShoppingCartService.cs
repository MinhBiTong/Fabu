using Application.DTOs.Requests.CartRequest;
using Application.DTOs.Requests.PaymentRequest;
using Application.DTOs.Responses.CartResponse;
using Application.DTOs.Responses.OrderResponse;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Application.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public ShoppingCartService(IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }

        public async Task<CartResponse> AddItemAsync(CartItemRequest request)
        {
            var cart = await GetOrCreateActiveCartAsync(request.CustomerId);
            var product = await GetSellableProductAsync(request.ProductId);
            EnsureAvailableStock(product, request.Quantity);

            var item = cart.Items.FirstOrDefault(item => item.ProductId == request.ProductId);
            if (item is null)
            {
                cart.Items.Add(new ShoppingCartItem
                {
                    ProductId = product.Id,
                    Product = product,
                    Quantity = request.Quantity,
                    UnitPrice = product.Price,
                    LineTotal = product.Price * request.Quantity
                });
            }
            else
            {
                var nextQuantity = item.Quantity + request.Quantity;
                EnsureAvailableStock(product, nextQuantity);
                item.Quantity = nextQuantity;
                item.UnitPrice = product.Price;
                item.LineTotal = item.UnitPrice * item.Quantity;
            }

            await _unitOfWork.SaveChangesAsync();
            return CartResponse.FromEntity(cart);
        }

        public async Task<OrderCheckoutResponse> CheckoutAsync(CartCheckoutRequest request)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetActiveCartByCustomerAsync(request.CustomerId);
            if (cart is null || cart.Items.Count == 0)
                throw new AppException(ErrorCode.INVALID_KEY, "Cart is empty.");

            foreach (var item in cart.Items)
            {
                if (item.Product is null)
                    throw new AppException(ErrorCode.INVALID_KEY, "Cart product not found.");

                EnsureSellable(item.Product);
                EnsureAvailableStock(item.Product, item.Quantity);
                item.UnitPrice = item.Product.Price;
                item.LineTotal = item.UnitPrice * item.Quantity;
            }

            var now = DateTimeOffset.UtcNow;
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                OrderCode = BuildOrderCode(),
                Status = OrderStatus.PendingPayment,
                PaymentMethod = request.PaymentMethod,
                CouponCode = request.CouponCode,
                ContactPhone = request.ContactPhone,
                ShippingAddress = request.ShippingAddress,
                Note = request.Note,
                SubTotal = cart.Items.Sum(item => item.LineTotal),
                DiscountAmount = 0,
                TotalAmount = cart.Items.Sum(item => item.LineTotal),
                Items = cart.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductCode = item.Product?.ProductCode ?? string.Empty,
                    ProductName = item.Product?.ProductName ?? string.Empty,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountAmount = 0,
                    LineTotal = item.LineTotal
                }).ToList()
            };

            await using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    cart.Status = ShoppingCartStatus.CheckedOut;
                    cart.CheckedOutAt = now;
                    await _unitOfWork.Orders.AddAsync(order);
                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            try
            {
                var payment = await _paymentService.CreatePaymentAsync(new PaymentCreateRequest
                {
                    CustomerId = request.CustomerId,
                    OrderId = order.Id,
                    Amount = order.TotalAmount,
                    PaymentMethod = request.PaymentMethod,
                    CouponCode = request.CouponCode,
                    UseAccountBalance = request.UseAccountBalance || request.PaymentMethod == PaymentMethod.Cash,
                    TransactionType = TransactionType.ProductPurchase,
                    OrderInfo = $"Fabu order {order.OrderCode}"
                });

                var savedOrder = await _unitOfWork.Orders.GetOrderWithItemsAsync(order.Id) ?? order;
                return new OrderCheckoutResponse
                {
                    Order = OrderResponse.FromEntity(savedOrder),
                    Payment = payment
                };
            }
            catch
            {
                var savedOrder = await _unitOfWork.Orders.GetOrderWithItemsAsync(order.Id);
                if (savedOrder is not null)
                {
                    savedOrder.Status = OrderStatus.Failed;
                    savedOrder.CancelledAt = DateTimeOffset.UtcNow;
                }

                cart.Status = ShoppingCartStatus.Active;
                cart.CheckedOutAt = null;
                await _unitOfWork.SaveChangesAsync();
                throw;
            }
        }

        public async Task<CartResponse> GetActiveCartAsync(long customerId)
        {
            var cart = await GetOrCreateActiveCartAsync(customerId);
            return CartResponse.FromEntity(cart);
        }

        public async Task<CartResponse> RemoveItemAsync(long customerId, long productId)
        {
            var cart = await GetOrCreateActiveCartAsync(customerId);
            var item = cart.Items.FirstOrDefault(item => item.ProductId == productId);
            if (item is not null)
            {
                cart.Items.Remove(item);
                await _unitOfWork.SaveChangesAsync();
            }

            return CartResponse.FromEntity(cart);
        }

        public async Task<CartResponse> UpdateItemAsync(CartItemRequest request)
        {
            var cart = await GetOrCreateActiveCartAsync(request.CustomerId);
            var product = await GetSellableProductAsync(request.ProductId);
            EnsureAvailableStock(product, request.Quantity);

            var item = cart.Items.FirstOrDefault(item => item.ProductId == request.ProductId);
            if (item is null)
                throw new AppException(ErrorCode.INVALID_KEY, "Cart item not found.");

            item.Quantity = request.Quantity;
            item.UnitPrice = product.Price;
            item.LineTotal = item.UnitPrice * item.Quantity;

            await _unitOfWork.SaveChangesAsync();
            return CartResponse.FromEntity(cart);
        }

        private async Task<ShoppingCart> GetOrCreateActiveCartAsync(long customerId)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer is null)
                throw new AppException(ErrorCode.CUSTOMER_NOT_FOUND);

            var cart = await _unitOfWork.ShoppingCarts.GetActiveCartByCustomerAsync(customerId);
            if (cart is not null) return cart;

            cart = new ShoppingCart
            {
                CustomerId = customerId,
                Status = ShoppingCartStatus.Active
            };

            await _unitOfWork.ShoppingCarts.AddAsync(cart);
            await _unitOfWork.SaveChangesAsync();
            return cart;
        }

        private async Task<TelecomProduct> GetSellableProductAsync(long productId)
        {
            var product = await _unitOfWork.TelecomProducts.GetByIdAsync(productId);
            if (product is null || product.IsDeleted)
                throw new AppException(ErrorCode.INVALID_KEY, "Product not found.");

            EnsureSellable(product);
            return product;
        }

        private static void EnsureSellable(TelecomProduct product)
        {
            if (!product.IsActive || !product.IsPublished)
                throw new AppException(ErrorCode.INVALID_KEY, "Product is not available for sale.");
        }

        private static void EnsureAvailableStock(TelecomProduct product, int requestedQuantity)
        {
            if (requestedQuantity <= 0)
                throw new AppException(ErrorCode.INVALID_AMOUNT, "Quantity must be greater than 0.");

            if (product.StockQuantity < requestedQuantity)
                throw new AppException(ErrorCode.INVALID_AMOUNT, "Product stock is not enough.");
        }

        private static string BuildOrderCode()
            => $"ORD_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}"[..30].ToUpperInvariant();
    }
}
