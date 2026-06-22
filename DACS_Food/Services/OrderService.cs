using DACS_Food.Data;
using DACS_Food.Models;
using DACS_Food.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace DACS_Food.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICartService _cartService;
        private readonly IDiscountService _discountService;
        private readonly IPaymentService _paymentService;
        private readonly ILoyaltyService _loyaltyService;
        private readonly IEmailSender _emailSender;

        public OrderService(ApplicationDbContext db, ICartService cartService, IDiscountService discountService, IPaymentService paymentService, ILoyaltyService loyaltyService, IEmailSender emailSender)
        {
            _db = db;
            _cartService = cartService;
            _discountService = discountService;
            _paymentService = paymentService;
            _loyaltyService = loyaltyService;
            _emailSender = emailSender;
        }

        public async Task<Order> CreateOrderAsync(string? userId, string sessionId, CreateOrderViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                throw new InvalidOperationException("Vui lòng nhập số điện thoại để quán liên hệ xác nhận đơn hàng.");
            }

            if (model.OrderType == OrderType.Delivery && string.IsNullOrWhiteSpace(model.Address))
            {
                throw new InvalidOperationException("Vui lòng nhập địa chỉ giao hàng.");
            }

            var cart = await _cartService.GetOrCreateCartAsync(userId, sessionId);
            var normalizedCart = await _cartService.GetCartViewModelAsync(userId, sessionId);
            var draftItems = normalizedCart.Items.Select(x => new OrderDraftItem(
                x.FoodItemId,
                x.FoodItem?.Name ?? "Món ăn",
                x.UnitPrice,
                x.Quantity)).ToList();

            if (!draftItems.Any() && !string.IsNullOrWhiteSpace(model.CartJson))
            {
                draftItems = await ParseClientCartAsync(model.CartJson);
            }

            if (!draftItems.Any())
            {
                throw new InvalidOperationException("Giỏ hàng đang trống.");
            }

            var subtotal = draftItems.Sum(x => x.UnitPrice * x.Quantity);
            var discountResult = await _discountService.CalculateDiscountAsync(model.DiscountCode, subtotal, userId);

            if (!string.IsNullOrWhiteSpace(discountResult.Message))
            {
                throw new InvalidOperationException(discountResult.Message);
            }

            var orderCode = await GenerateOrderCodeAsync();
            var shipmentCode = await GenerateShipmentCodeAsync(orderCode);
            var now = DateTime.UtcNow;

            var order = new Order
            {
                OrderCode = orderCode,
                TrackingCode = shipmentCode,
                UserId = userId,
                CustomerName = model.CustomerName.Trim(),
                PhoneNumber = model.PhoneNumber.Trim(),
                Address = model.Address.Trim(),
                OrderType = model.OrderType,
                TableId = model.TableId,
                Subtotal = subtotal,
                DiscountAmount = discountResult.Amount,
                TotalAmount = subtotal - discountResult.Amount,
                PaymentMethod = model.PaymentMethod,
                PaymentStatus = PaymentStatus.Unpaid,
                OrderStatus = OrderStatus.Pending,
                DeliveryStatus = model.OrderType == OrderType.Delivery ? DeliveryStatus.Pending : DeliveryStatus.Delivered,
                DiscountCodeId = discountResult.Code?.Id,
                CreatedAt = now,
                UpdatedAt = now,
                Items = draftItems.Select(x => new OrderItem
                {
                    FoodItemId = x.FoodItemId,
                    FoodName = x.FoodName,
                    UnitPrice = x.UnitPrice,
                    Quantity = x.Quantity,
                    LineTotal = x.UnitPrice * x.Quantity
                }).ToList(),
                Shipment = new Shipment
                {
                    ShipmentCode = shipmentCode,
                    OrderCode = orderCode,
                    CustomerName = model.CustomerName.Trim(),
                    CustomerPhone = model.PhoneNumber.Trim(),
                    DeliveryAddress = model.Address.Trim(),
                    DeliveryStatus = model.OrderType == OrderType.Delivery ? DeliveryStatus.Pending : DeliveryStatus.Delivered,
                    UpdatedAt = now
                },
                StatusHistories = new List<OrderStatusHistory>
                {
                    new()
                    {
                        OrderStatus = OrderStatus.Pending,
                        DeliveryStatus = model.OrderType == OrderType.Delivery ? DeliveryStatus.Pending : DeliveryStatus.Delivered,
                        Note = "Khách tạo đơn hàng online.",
                        UpdatedBy = string.IsNullOrWhiteSpace(userId) ? "Khách vãng lai" : "Khách đăng nhập",
                        CreatedAt = now
                    }
                }
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            await _discountService.MarkUsedAsync(discountResult.Code, userId, order.Id);
            await _paymentService.CreatePaymentAsync(order);
            await SendOrderConfirmationEmailAsync(order, model.CustomerEmail);
            if (cart.Items.Any())
            {
                await _cartService.ClearAsync(cart);
            }

            return order;
        }

        public Task<Order?> GetByCodeAsync(string orderCode)
        {
            return _db.Orders
                .Include(x => x.Items)
                .Include(x => x.Payment)
                .Include(x => x.Shipment)
                .Include(x => x.StatusHistories.OrderByDescending(h => h.CreatedAt))
                .FirstOrDefaultAsync(x => x.OrderCode == orderCode);
        }

        public Task<Order?> GetByIdAsync(int id)
        {
            return _db.Orders
                .Include(x => x.Items)
                .Include(x => x.Payment)
                .Include(x => x.Shipment)
                .Include(x => x.StatusHistories.OrderByDescending(h => h.CreatedAt))
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Order>> TrackAsync(string? orderCode, string? phone)
        {
            if (!string.IsNullOrWhiteSpace(orderCode))
            {
                var code = orderCode.Trim();
                var query = _db.Orders
                    .AsNoTracking()
                    .Include(x => x.Items)
                    .Include(x => x.Payment)
                    .Include(x => x.Shipment)
                    .Include(x => x.StatusHistories.OrderByDescending(h => h.CreatedAt))
                    .Where(x => x.OrderCode == code);

                if (!string.IsNullOrWhiteSpace(phone))
                {
                    var normalizedPhone = phone.Trim();
                    query = query.Where(x => x.PhoneNumber == normalizedPhone);
                }

                return await query
                    .ToListAsync();
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var normalizedPhone = phone.Trim();
                return await _db.Orders
                    .AsNoTracking()
                    .Include(x => x.Items)
                    .Include(x => x.Payment)
                    .Include(x => x.Shipment)
                    .Where(x => x.PhoneNumber == normalizedPhone)
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(10)
                    .ToListAsync();
            }

            return Array.Empty<Order>();
        }

        public async Task<IReadOnlyList<Order>> GetByUserIdAsync(string userId, int count = 10)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Array.Empty<Order>();
            }

            return await _db.Orders
                .AsNoTracking()
                .Include(x => x.Items)
                .Include(x => x.Payment)
                .Include(x => x.Shipment)
                .Include(x => x.StatusHistories.OrderByDescending(h => h.CreatedAt))
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message, Order? Order)> UpdateStatusAsync(int id, OrderStatus status, string updatedBy)
        {
            var order = await _db.Orders
                .Include(x => x.Shipment)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
            {
                return (false, "Không tìm thấy đơn hàng.", null);
            }

            if (!CanMoveOrderStatus(order.OrderStatus, status))
            {
                return (false, GetInvalidStatusMessage(order.OrderStatus, status), order);
            }

            ApplyOrderStatus(order, status);
            AddHistory(order, $"Cập nhật trạng thái đơn hàng sang {GetOrderStatusLabel(status)}.", updatedBy);
            await _db.SaveChangesAsync();
            await _loyaltyService.RefreshUserLevelAsync(order.UserId);
            return (true, "Đã cập nhật trạng thái đơn hàng.", order);
        }

        public async Task<(bool Success, string Message, Order? Order)> UpdateDeliveryAsync(int id, DeliveryStatus deliveryStatus, string? shipperName, string? shipperPhone, string? deliveryNote, string updatedBy)
        {
            var order = await _db.Orders
                .Include(x => x.Shipment)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
            {
                return (false, "Không tìm thấy đơn hàng.", null);
            }

            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                return (false, "Đơn hàng đã hủy, không thể cập nhật vận đơn.", order);
            }

            if (deliveryStatus == DeliveryStatus.Delivering && order.OrderStatus != OrderStatus.ReadyForDelivery && order.OrderStatus != OrderStatus.Delivering)
            {
                return (false, "Chỉ được bắt đầu giao khi đơn đã chuẩn bị xong.", order);
            }

            EnsureShipment(order);
            order.ShipperName = shipperName?.Trim() ?? string.Empty;
            order.ShipperPhone = shipperPhone?.Trim() ?? string.Empty;
            order.DeliveryNote = deliveryNote?.Trim() ?? string.Empty;
            order.DeliveryStatus = deliveryStatus;
            order.UpdatedAt = DateTime.UtcNow;

            order.Shipment!.ShipperName = order.ShipperName;
            order.Shipment.ShipperPhone = order.ShipperPhone;
            order.Shipment.DeliveryNote = order.DeliveryNote;
            order.Shipment.DeliveryStatus = deliveryStatus;
            order.Shipment.UpdatedAt = order.UpdatedAt;

            if (deliveryStatus == DeliveryStatus.Delivering)
            {
                order.OrderStatus = OrderStatus.Delivering;
                order.ShippedAt ??= DateTime.UtcNow;
                order.Shipment.DeliveryStartedAt ??= order.ShippedAt;
            }

            if (deliveryStatus == DeliveryStatus.Delivered)
            {
                order.OrderStatus = OrderStatus.Completed;
                order.DeliveredAt ??= DateTime.UtcNow;
                order.Shipment.DeliveredAt ??= order.DeliveredAt;
            }

            AddHistory(order, $"Cập nhật vận đơn sang {GetDeliveryStatusLabel(deliveryStatus)}.", updatedBy);
            await _db.SaveChangesAsync();
            await _loyaltyService.RefreshUserLevelAsync(order.UserId);
            return (true, "Đã cập nhật vận đơn.", order);
        }

        public async Task<(bool Success, string Message, Order? Order)> ConfirmDeliveredAsync(string orderCode, string updatedBy)
        {
            if (string.IsNullOrWhiteSpace(orderCode))
            {
                return (false, "Vui lòng nhập mã đơn hàng.", null);
            }

            var order = await _db.Orders
                .Include(x => x.Shipment)
                .FirstOrDefaultAsync(x => x.OrderCode == orderCode.Trim());
            if (order == null)
            {
                return (false, "Mã đơn không tồn tại.", null);
            }

            if (order.OrderStatus == OrderStatus.Completed || order.OrderStatus == OrderStatus.Delivered || order.DeliveryStatus == DeliveryStatus.Delivered)
            {
                return (false, "Đơn hàng đã hoàn thành trước đó.", order);
            }

            if (order.OrderStatus != OrderStatus.Delivering && order.DeliveryStatus != DeliveryStatus.Delivering)
            {
                return (false, "Đơn hàng chưa trong trạng thái đang giao.", order);
            }

            EnsureShipment(order);
            order.OrderStatus = OrderStatus.Completed;
            order.DeliveryStatus = DeliveryStatus.Delivered;
            order.DeliveredAt ??= DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            order.Shipment!.DeliveryStatus = DeliveryStatus.Delivered;
            order.Shipment.DeliveredAt ??= order.DeliveredAt;
            order.Shipment.UpdatedAt = order.UpdatedAt;
            AddHistory(order, "Nhân viên xác nhận shipper đã giao thành công.", updatedBy);

            await _db.SaveChangesAsync();
            await _loyaltyService.RefreshUserLevelAsync(order.UserId);
            return (true, "Đã xác nhận giao hàng thành công.", order);
        }

        private async Task<List<OrderDraftItem>> ParseClientCartAsync(string cartJson)
        {
            try
            {
                var items = JsonSerializer.Deserialize<List<ClientCartItem>>(cartJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<ClientCartItem>();

                var validItems = items
                    .Where(x => x.Id > 0 && x.Quantity > 0)
                    .GroupBy(x => x.Id)
                    .Select(x => new
                    {
                        Id = x.Key,
                        Quantity = Math.Min(20, x.Sum(item => Math.Clamp(item.Quantity, 1, 20)))
                    })
                    .ToList();

                if (!validItems.Any())
                {
                    return new List<OrderDraftItem>();
                }

                var foodIds = validItems.Select(x => x.Id).Distinct().ToList();
                var foods = await _db.FoodItems
                    .Where(x => foodIds.Contains(x.Id) && x.IsActive && x.IsAvailable)
                    .ToDictionaryAsync(x => x.Id);

                return validItems
                    .Where(x => foods.ContainsKey(x.Id))
                    .Select(x =>
                    {
                        var food = foods[x.Id];
                        return new OrderDraftItem(food.Id, food.Name, food.Price, x.Quantity);
                    })
                    .ToList();
            }
            catch (JsonException)
            {
                return new List<OrderDraftItem>();
            }
        }

        private async Task SendOrderConfirmationEmailAsync(Order order, string? customerEmail)
        {
            if (string.IsNullOrWhiteSpace(customerEmail))
            {
                return;
            }

            var body = BuildOrderConfirmationBody(order);
            try
            {
                await _emailSender.SendAsync(customerEmail.Trim(), $"FoodieLab xác nhận đơn hàng {order.OrderCode}", body);
            }
            catch
            {
                // Không làm hỏng luồng đặt hàng nếu SMTP tạm thời lỗi.
            }
        }

        private static string BuildOrderConfirmationBody(Order order)
        {
            var builder = new StringBuilder();
            builder.AppendLine("FoodieLab xác nhận đơn hàng của bạn.");
            builder.AppendLine();
            builder.AppendLine($"Mã đơn hàng: {order.OrderCode}");
            builder.AppendLine($"Tên khách hàng: {order.CustomerName}");
            builder.AppendLine($"Số điện thoại: {order.PhoneNumber}");
            if (!string.IsNullOrWhiteSpace(order.Address))
            {
                builder.AppendLine($"Địa chỉ giao hàng: {order.Address}");
            }

            builder.AppendLine();
            builder.AppendLine("Danh sách món đã đặt:");
            foreach (var item in order.Items)
            {
                builder.AppendLine($"- {item.FoodName} x {item.Quantity}: {item.UnitPrice:N0}đ/món, thành tiền {item.LineTotal:N0}đ");
            }

            builder.AppendLine();
            builder.AppendLine($"Tổng tiền: {order.TotalAmount:N0}đ");
            builder.AppendLine($"Phương thức thanh toán: {(order.PaymentMethod == PaymentMethod.COD ? "Tiền mặt / COD" : "Chuyển khoản QR")}");
            builder.AppendLine($"Trạng thái đơn hàng: {GetOrderStatusLabel(order.OrderStatus)}");
            builder.AppendLine($"Trạng thái thanh toán: {GetPaymentStatusLabel(order.PaymentStatus)}");
            if (order.PaymentMethod == PaymentMethod.QR && order.Payment != null)
            {
                builder.AppendLine($"Nội dung chuyển khoản: {order.Payment.QrContent}");
                builder.AppendLine($"Ngân hàng: {order.Payment.BankName}");
                builder.AppendLine($"Số tài khoản: {order.Payment.BankAccountNumber}");
                builder.AppendLine($"Chủ tài khoản: {order.Payment.BankAccountName}");
            }

            return builder.ToString();
        }

        private async Task<string> GenerateOrderCodeAsync()
        {
            for (var i = 0; i < 8; i++)
            {
                var code = $"FD{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}";
                if (!await _db.Orders.AnyAsync(x => x.OrderCode == code))
                {
                    return code;
                }
            }

            return $"FD{Guid.NewGuid():N}"[..18].ToUpperInvariant();
        }

        private async Task<string> GenerateShipmentCodeAsync(string orderCode)
        {
            var code = $"VD{orderCode[2..]}";
            if (!await _db.Shipments.AnyAsync(x => x.ShipmentCode == code) &&
                !await _db.Orders.AnyAsync(x => x.TrackingCode == code))
            {
                return code;
            }

            return $"VD{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}";
        }

        private static bool CanMoveOrderStatus(OrderStatus current, OrderStatus next)
        {
            if (current == next) return true;
            if (current == OrderStatus.Cancelled) return false;
            if (current == OrderStatus.Completed || current == OrderStatus.Delivered) return next == OrderStatus.Completed;
            if (next == OrderStatus.Cancelled) return true;

            return current switch
            {
                OrderStatus.Pending or OrderStatus.New => next == OrderStatus.Confirmed,
                OrderStatus.Confirmed => next == OrderStatus.Preparing,
                OrderStatus.Preparing => next == OrderStatus.ReadyForDelivery,
                OrderStatus.ReadyForDelivery => next == OrderStatus.Delivering,
                OrderStatus.Delivering => next == OrderStatus.Delivered || next == OrderStatus.Completed,
                _ => false
            };
        }

        private static string GetInvalidStatusMessage(OrderStatus current, OrderStatus next)
        {
            if (current == OrderStatus.Cancelled)
            {
                return "Đơn hàng đã hủy, không thể cập nhật sang trạng thái mới.";
            }

            return $"Không thể chuyển từ {GetOrderStatusLabel(current)} sang {GetOrderStatusLabel(next)}.";
        }

        private static void ApplyOrderStatus(Order order, OrderStatus status)
        {
            EnsureShipment(order);
            order.OrderStatus = status;
            order.UpdatedAt = DateTime.UtcNow;

            if (status == OrderStatus.ReadyForDelivery)
            {
                order.DeliveryStatus = DeliveryStatus.ReadyForDelivery;
            }
            else if (status == OrderStatus.Delivering)
            {
                order.DeliveryStatus = DeliveryStatus.Delivering;
                order.ShippedAt ??= DateTime.UtcNow;
                order.Shipment!.DeliveryStartedAt ??= order.ShippedAt;
            }
            else if (status == OrderStatus.Delivered || status == OrderStatus.Completed)
            {
                order.OrderStatus = OrderStatus.Completed;
                order.DeliveryStatus = DeliveryStatus.Delivered;
                order.DeliveredAt ??= DateTime.UtcNow;
                order.Shipment!.DeliveredAt ??= order.DeliveredAt;
            }
            else if (status == OrderStatus.Cancelled)
            {
                order.DeliveryStatus = DeliveryStatus.Cancelled;
            }

            order.Shipment!.DeliveryStatus = order.DeliveryStatus;
            order.Shipment.UpdatedAt = order.UpdatedAt;
        }

        private static void EnsureShipment(Order order)
        {
            order.Shipment ??= new Shipment
            {
                OrderId = order.Id,
                ShipmentCode = string.IsNullOrWhiteSpace(order.TrackingCode) ? $"VD{order.OrderCode}" : order.TrackingCode,
                OrderCode = order.OrderCode,
                CustomerName = order.CustomerName,
                CustomerPhone = order.PhoneNumber,
                DeliveryAddress = order.Address,
                ShipperName = order.ShipperName,
                ShipperPhone = order.ShipperPhone,
                DeliveryNote = order.DeliveryNote,
                DeliveryStatus = order.DeliveryStatus,
                DeliveryStartedAt = order.ShippedAt,
                DeliveredAt = order.DeliveredAt,
                UpdatedAt = DateTime.UtcNow
            };
        }

        private static void AddHistory(Order order, string note, string updatedBy)
        {
            order.StatusHistories.Add(new OrderStatusHistory
            {
                OrderStatus = order.OrderStatus,
                DeliveryStatus = order.DeliveryStatus,
                Note = note,
                UpdatedBy = updatedBy,
                CreatedAt = DateTime.UtcNow
            });
        }

        public static string GetOrderStatusLabel(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending or OrderStatus.New => "Chờ xác nhận",
                OrderStatus.Confirmed => "Đã xác nhận",
                OrderStatus.Preparing => "Đang chuẩn bị món",
                OrderStatus.ReadyForDelivery => "Đã chuẩn bị xong, chờ giao hàng",
                OrderStatus.Delivering => "Đang giao hàng",
                OrderStatus.Delivered => "Đã giao đến khách",
                OrderStatus.Completed => "Hoàn thành",
                OrderStatus.Cancelled => "Đã hủy",
                _ => status.ToString()
            };
        }

        public static string GetDeliveryStatusLabel(DeliveryStatus status)
        {
            return status switch
            {
                DeliveryStatus.Pending => "Chờ xử lý",
                DeliveryStatus.ReadyForDelivery or DeliveryStatus.AssigningShipper => "Chờ giao hàng",
                DeliveryStatus.Delivering or DeliveryStatus.Shipping => "Đang giao hàng",
                DeliveryStatus.Delivered => "Đã giao thành công",
                DeliveryStatus.Failed => "Giao thất bại",
                DeliveryStatus.Cancelled => "Đã hủy",
                _ => status.ToString()
            };
        }

        public static string GetPaymentStatusLabel(PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Unpaid => "Chưa thanh toán",
                PaymentStatus.Pending => "Chờ thanh toán",
                PaymentStatus.Paid => "Đã thanh toán",
                PaymentStatus.Failed => "Thanh toán thất bại",
                _ => status.ToString()
            };
        }

        private sealed record OrderDraftItem(int FoodItemId, string FoodName, decimal UnitPrice, int Quantity);

        private sealed class ClientCartItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }
    }
}
