using DACS_Food.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<FoodCategory> FoodCategories => Set<FoodCategory>();
        public DbSet<FoodItem> FoodItems => Set<FoodItem>();
        public DbSet<Recipe> Recipes => Set<Recipe>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<RestaurantTable> RestaurantTables => Set<RestaurantTable>();
        public DbSet<TableReservation> TableReservations => Set<TableReservation>();
        public DbSet<DiscountCode> DiscountCodes => Set<DiscountCode>();
        public DbSet<DiscountUsage> DiscountUsages => Set<DiscountUsage>();
        public DbSet<EmailOtp> EmailOtps => Set<EmailOtp>();
        public DbSet<OtpSendLog> OtpSendLogs => Set<OtpSendLog>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FoodCategory>()
                .HasIndex(x => x.Slug)
                .IsUnique();

            builder.Entity<FoodItem>()
                .HasIndex(x => x.Slug)
                .IsUnique();

            builder.Entity<ChatMessage>()
                .HasIndex(x => new { x.UserId, x.SessionId, x.CreatedAt });

            builder.Entity<ChatMessage>()
                .Property(x => x.Role)
                .HasMaxLength(32);

            builder.Entity<ChatMessage>()
                .Property(x => x.Intent)
                .HasMaxLength(64);

            builder.Entity<ChatMessage>()
                .Property(x => x.SessionId)
                .HasMaxLength(128);

            builder.Entity<ChatMessage>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<FoodItem>()
                .Property(x => x.MainCategory)
                .HasDefaultValue(string.Empty);

            builder.Entity<FoodItem>()
                .Property(x => x.Subcategory)
                .HasDefaultValue(string.Empty);

            builder.Entity<FoodItem>()
                .Property(x => x.AllergyNote)
                .HasDefaultValue(string.Empty);

            builder.Entity<FoodItem>()
                .Property(x => x.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<FoodItem>()
                .Property(x => x.DiscountPrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Recipe>()
                .Property(x => x.Category)
                .HasDefaultValue(string.Empty);

            builder.Entity<Recipe>()
                .Property(x => x.CategoryLabel)
                .HasDefaultValue(string.Empty);

            builder.Entity<Recipe>()
                .Property(x => x.PrepTime)
                .HasDefaultValue(string.Empty);

            builder.Entity<Recipe>()
                .Property(x => x.CookTime)
                .HasDefaultValue(string.Empty);

            builder.Entity<Recipe>()
                .Property(x => x.Servings)
                .HasDefaultValue(string.Empty);

            builder.Entity<Recipe>()
                .Property(x => x.Tips)
                .HasDefaultValue(string.Empty);

            builder.Entity<Recipe>()
                .Property(x => x.SafetyNote)
                .HasDefaultValue(string.Empty);

            builder.Entity<Recipe>()
                .Property(x => x.AllergyNote)
                .HasDefaultValue(string.Empty);

            builder.Entity<Order>()
                .Property(x => x.OrderCode)
                .HasMaxLength(64);

            builder.Entity<Order>()
                .HasIndex(x => x.OrderCode)
                .IsUnique();

            builder.Entity<Order>()
                .Property(x => x.TrackingCode)
                .HasDefaultValue(string.Empty);

            builder.Entity<Order>()
                .Property(x => x.ShipperName)
                .HasDefaultValue(string.Empty);

            builder.Entity<Order>()
                .Property(x => x.ShipperPhone)
                .HasDefaultValue(string.Empty);

            builder.Entity<Order>()
                .Property(x => x.DeliveryNote)
                .HasDefaultValue(string.Empty);

            builder.Entity<Order>()
                .Property(x => x.UpdatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Entity<Shipment>()
                .Property(x => x.ShipmentCode)
                .HasMaxLength(64);

            builder.Entity<Shipment>()
                .HasIndex(x => x.ShipmentCode)
                .IsUnique();

            builder.Entity<Shipment>()
                .Property(x => x.ShipmentCode)
                .HasDefaultValue(string.Empty);

            builder.Entity<Shipment>()
                .Property(x => x.OrderCode)
                .HasDefaultValue(string.Empty);

            builder.Entity<Shipment>()
                .Property(x => x.CustomerName)
                .HasDefaultValue(string.Empty);

            builder.Entity<Shipment>()
                .Property(x => x.CustomerPhone)
                .HasDefaultValue(string.Empty);

            builder.Entity<Shipment>()
                .Property(x => x.DeliveryAddress)
                .HasDefaultValue(string.Empty);

            builder.Entity<Shipment>()
                .Property(x => x.ShipperName)
                .HasDefaultValue(string.Empty);

            builder.Entity<Shipment>()
                .Property(x => x.ShipperPhone)
                .HasDefaultValue(string.Empty);

            builder.Entity<Shipment>()
                .Property(x => x.DeliveryNote)
                .HasDefaultValue(string.Empty);

            builder.Entity<Shipment>()
                .HasOne(x => x.Order)
                .WithOne(x => x.Shipment)
                .HasForeignKey<Shipment>(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderStatusHistory>()
                .Property(x => x.Note)
                .HasDefaultValue(string.Empty);

            builder.Entity<OrderStatusHistory>()
                .Property(x => x.UpdatedBy)
                .HasDefaultValue(string.Empty);

            builder.Entity<OrderStatusHistory>()
                .HasOne(x => x.Order)
                .WithMany(x => x.StatusHistories)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>()
                .Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Order>()
                .Property(x => x.Subtotal)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Order>()
                .Property(x => x.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Order>()
                .Property(x => x.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<OrderItem>()
                .Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<OrderItem>()
                .Property(x => x.LineTotal)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Payment>()
                .Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<DiscountCode>()
                .HasIndex(x => x.Code)
                .IsUnique();

            builder.Entity<DiscountCode>()
                .Property(x => x.DiscountValue)
                .HasColumnType("decimal(18,2)");

            builder.Entity<DiscountCode>()
                .Property(x => x.MinOrderAmount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<DiscountCode>()
                .Property(x => x.MaxDiscountAmount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<DiscountCode>()
                .Property(x => x.RequiredLoyaltyLevel)
                .HasMaxLength(32);

            builder.Entity<Payment>()
                .HasOne(x => x.Order)
                .WithOne(x => x.Payment)
                .HasForeignKey<Payment>(x => x.OrderId);

            builder.Entity<TableReservation>()
                .HasOne(x => x.RestaurantTable)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.RestaurantTableId);

            SeedFoodCategories(builder);
            SeedRestaurantTables(builder);
            SeedDiscountCodes(builder);
            SeedFoodItems(builder);
        }

        private static void SeedFoodCategories(ModelBuilder builder)
        {
            builder.Entity<FoodCategory>().HasData(
                new FoodCategory { Id = 1, Name = "Cơm", Slug = "com", Description = "Các món cơm no bụng", SortOrder = 1 },
                new FoodCategory { Id = 2, Name = "Mì", Slug = "mi", Description = "Mì và món sợi", SortOrder = 2 },
                new FoodCategory { Id = 3, Name = "Món chay", Slug = "mon-chay", Description = "Món nhẹ, ít dầu mỡ", SortOrder = 3 },
                new FoodCategory { Id = 4, Name = "Healthy", Slug = "healthy", Description = "Món cân bằng dinh dưỡng", SortOrder = 4 },
                new FoodCategory { Id = 5, Name = "Gà", Slug = "ga", Description = "Các món từ gà", SortOrder = 5 },
                new FoodCategory { Id = 6, Name = "Fastfood", Slug = "fastfood", Description = "Đồ ăn nhanh", SortOrder = 6 },
                new FoodCategory { Id = 7, Name = "Ăn vặt", Slug = "an-vat", Description = "Món ăn nhẹ", SortOrder = 7 },
                new FoodCategory { Id = 8, Name = "Đồ uống", Slug = "do-uong", Description = "Nước uống", SortOrder = 8 }
            );
        }

        private static void SeedRestaurantTables(ModelBuilder builder)
        {
            var tables = Enumerable.Range(1, 10).Select(i => new RestaurantTable
            {
                Id = i,
                TableNumber = i,
                Name = i <= 2 ? $"Bàn private {i}" : $"Bàn thường {i}",
                TableType = i <= 2 ? TableType.Private : TableType.Normal,
                Capacity = i <= 2 ? 6 : 4,
                Status = TableStatus.Available,
                IsActive = true
            });

            builder.Entity<RestaurantTable>().HasData(tables);
        }

        private static void SeedDiscountCodes(ModelBuilder builder)
        {
            builder.Entity<DiscountCode>().HasData(
                new DiscountCode
                {
                    Id = 1,
                    Code = "GIOVANG10",
                    Name = "Giảm giá giờ vàng",
                    DiscountType = DiscountType.Percent,
                    DiscountValue = 10,
                    MinOrderAmount = 50000,
                    MaxDiscountAmount = 30000,
                    DiscountScope = DiscountScope.GoldenHour,
                    IsActive = true
                },
                new DiscountCode
                {
                    Id = 2,
                    Code = "THANTHIET15",
                    Name = "Ưu đãi thành viên thân thiết",
                    DiscountType = DiscountType.Percent,
                    DiscountValue = 15,
                    MinOrderAmount = 80000,
                    MaxDiscountAmount = 50000,
                    DiscountScope = DiscountScope.Loyalty,
                    RequiredLoyaltyLevel = "Thành viên",
                    IsActive = true
                }
            );
        }

        private static void SeedFoodItems(ModelBuilder builder)
        {
            builder.Entity<FoodItem>().HasData(
                new FoodItem { Id = 1, FoodCategoryId = 1, Name = "Cơm gà sốt tiêu đen", Slug = "com-ga-sot-tieu-den", Category = "cơm", Price = 59000, Tag = "Best seller", IsBestSeller = true, ImageUrl = "https://images.unsplash.com/photo-1603133872878-684f208fb84b?auto=format&fit=crop&w=800&q=80", Description = "Cơm nóng ăn cùng gà sốt tiêu đen đậm vị.", Ingredients = "Cơm, gà, tiêu đen, rau ăn kèm", Story = "Món no bụng, hợp bữa trưa văn phòng." },
                new FoodItem { Id = 2, FoodCategoryId = 5, Name = "Gà chiên mật ong", Slug = "ga-chien-mat-ong", Category = "gà", Price = 69000, Tag = "Giòn ngọt", IsBestSeller = true, ImageUrl = "https://images.unsplash.com/photo-1626645738196-c2a7c87a8f58?auto=format&fit=crop&w=800&q=80", Description = "Gà chiên phủ sốt mật ong nhẹ.", Ingredients = "Gà, mật ong, mè, gia vị", Story = "Vị ngọt mặn dễ ăn cho nhóm bạn." },
                new FoodItem { Id = 3, FoodCategoryId = 3, Name = "Cơm chay nấm", Slug = "com-chay-nam", Category = "món chay", Price = 49000, Tag = "Thanh đạm", IsBestSeller = true, ImageUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=800&q=80", Description = "Cơm chay với nấm và rau củ.", Ingredients = "Cơm, nấm, rau củ, nước tương", Story = "Phù hợp ngày ăn nhẹ hoặc ăn chay." },
                new FoodItem { Id = 4, FoodCategoryId = 2, Name = "Mì bò cay", Slug = "mi-bo-cay", Category = "mì", Price = 65000, Tag = "Đậm vị", IsBestSeller = false, ImageUrl = "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?auto=format&fit=crop&w=800&q=80", Description = "Mì bò cay nóng, nước dùng đậm.", Ingredients = "Mì, bò, ớt, hành", Story = "Hợp lúc cần món nóng và chắc bụng." },
                new FoodItem { Id = 5, FoodCategoryId = 4, Name = "Salad ức gà", Slug = "salad-uc-ga", Category = "healthy", Price = 62000, Tag = "Healthy", IsBestSeller = false, ImageUrl = "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=800&q=80", Description = "Salad rau xanh cùng ức gà áp chảo.", Ingredients = "Ức gà, xà lách, cà chua, sốt mè", Story = "Lựa chọn cân bằng cho bữa tối nhẹ." },
                new FoodItem { Id = 6, FoodCategoryId = 6, Name = "Burger bò phô mai", Slug = "burger-bo-pho-mai", Category = "fastfood", Price = 79000, Tag = "Nhanh gọn", IsBestSeller = false, ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=800&q=80", Description = "Burger bò kèm phô mai béo nhẹ.", Ingredients = "Bánh burger, bò, phô mai, rau", Story = "Món nhanh cho buổi học hoặc làm việc bận." }
            );
        }
    }
}
