using DACS_Food.Models;
using DACS_Food.ViewModels;

namespace DACS_Food.Services
{
    public interface IFoodService
    {
        Task<MenuViewModel> GetMenuAsync(string? category, string? keyword, int page, int pageSize, string? mainCategory = null, string? subcategory = null);
        Task<IReadOnlyList<FoodItem>> GetBestSellersAsync(int count);
        Task<IReadOnlyList<FoodItem>> GetRelatedAsync(FoodItem food, int count);
        Task<FoodItem?> GetBySlugAsync(string slug);
        Task<FoodItem?> GetByIdAsync(int id);
    }

    public interface IChatbotService
    {
        ChatbotAskResponse Ask(string message);
        IReadOnlyList<Recipe> GetRecipes();
        Recipe? GetRecipeBySlug(string slug);
    }

    public interface IGeminiChatService
    {
        Task<ChatbotResponse> AskAsync(ChatbotRequest request, string ipAddress);
    }

    public interface ICartService
    {
        Task<Cart> GetOrCreateCartAsync(string? userId, string sessionId);
        Task<CartViewModel> GetCartViewModelAsync(string? userId, string sessionId);
        Task AddAsync(string? userId, string sessionId, int foodItemId, int quantity);
        Task UpdateAsync(string? userId, string sessionId, int cartItemId, int quantity);
        Task RemoveAsync(string? userId, string sessionId, int cartItemId);
        Task ClearAsync(Cart cart);
    }

    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string? userId, string sessionId, CreateOrderViewModel model);
        Task<Order?> GetByCodeAsync(string orderCode);
        Task<Order?> GetByIdAsync(int id);
        Task<IReadOnlyList<Order>> TrackAsync(string? orderCode);
        Task<IReadOnlyList<Order>> GetByUserIdAsync(string userId, int count = 10);
        Task<(bool Success, string Message, Order? Order)> UpdateStatusAsync(int id, OrderStatus status, string updatedBy);
        Task<(bool Success, string Message, Order? Order)> UpdateDeliveryAsync(int id, DeliveryStatus deliveryStatus, string? shipperName, string? shipperPhone, string? deliveryNote, string updatedBy);
        Task<(bool Success, string Message, Order? Order)> ConfirmDeliveredAsync(string orderCode, string updatedBy);
    }

    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(Order order);
        Task<bool> ConfirmDemoPaymentAsync(string orderCode);
    }

    public interface IQrPaymentService
    {
        QrPaymentInfoViewModel CreateQrInfo(Order order);
    }

    public interface ITableService
    {
        Task<IReadOnlyList<RestaurantTable>> GetActiveTablesAsync();
        Task<bool> UpdateStatusAsync(int tableId, TableStatus status);
        Task<TablePageViewModel> GetTablePageAsync(DateOnly? date = null);
        Task<IReadOnlyList<ReservationSlotViewModel>> GetReservationSlotsAsync(DateOnly date, int tableId);
        Task<(bool Success, string Message)> CreateReservationAsync(CreateTableReservationViewModel model);
        Task<(bool Success, string Message)> UpdateReservationStatusAsync(int reservationId, ReservationStatus status);
    }

    public interface IReservationRateLimitService
    {
        (bool Allowed, string Message) CanCreateReservation(string ipAddress, string phoneNumber);
        void RecordCreateReservation(string ipAddress, string phoneNumber);
    }

    public interface IDiscountService
    {
        bool IsGoldenHour(DateTime localTime);
        Task<(DiscountCode? Code, decimal Amount, string? Message)> CalculateDiscountAsync(string? code, decimal subtotal, string? userId);
        Task MarkUsedAsync(DiscountCode? code, string? userId, int orderId);
    }

    public interface ILoyaltyService
    {
        string GetLevel(decimal totalSpent);
        string GetNextLevel(string currentLevel);
        decimal? GetNextLevelTarget(string currentLevel);
        int GetLevelRank(string? level);
        Task<decimal> GetEligibleTotalSpentAsync(string userId);
        Task<string> RefreshUserLevelAsync(string? userId);
    }

    public interface IOtpRateLimitService
    {
        Task<(bool Allowed, string? Message)> CanSendAsync(string email, string ipAddress, OtpPurpose purpose);
        Task RecordSendAsync(string email, string ipAddress, OtpPurpose purpose);
    }

    public interface IOtpService
    {
        Task<(bool Success, string? Message)> SendOtpAsync(string? userId, string email, OtpPurpose purpose, string ipAddress);
        Task<(bool Success, string? Message)> VerifyAsync(string email, string code, OtpPurpose purpose);
    }

    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string body);
    }
}
