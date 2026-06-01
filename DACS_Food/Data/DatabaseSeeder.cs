using DACS_Food.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Data
{
    public static class DatabaseSeeder
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var db = services.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync();
            await EnsureTableReservationsTableAsync(db);
            await EnsureOrderDeliveryColumnsAsync(db);
            await EnsureShipmentTablesAsync(db);
            await EnsureFoodItemMenuColumnsAsync(db);
            await EnsureRecipeCatalogColumnsAsync(db);
            await EnsureDiscountCodeLoyaltyColumnsAsync(db);
            await EnsureUserAvatarColumnAsync(db);
            await EnsureChatMessagesTableAsync(db);
            await UpsertMenuCatalogAsync(db);
            await UpsertRecipeCatalogAsync(db);

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            await EnsureRoleAsync(roleManager, "Admin");
            await EnsureRoleAsync(roleManager, "Customer");

            const string adminEmail = "admin@foodielab.local";
            var admin = await userManager.Users.FirstOrDefaultAsync(x => x.Email == adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "FoodieTTTM Admin",
                    EmailConfirmed = true,
                    IsEmailVerified = true,
                    LoyaltyLevel = "Bạch kim"
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            else if (admin.LoyaltyLevel == "Member")
            {
                admin.LoyaltyLevel = "Bạch kim";
                await userManager.UpdateAsync(admin);
            }

            if (admin != null && !await userManager.IsInRoleAsync(admin, "Admin"))
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            const string customerEmail = "user@foodielab.local";
            var customer = await userManager.Users.FirstOrDefaultAsync(x => x.Email == customerEmail);
            if (customer == null)
            {
                customer = new ApplicationUser
                {
                    UserName = customerEmail,
                    Email = customerEmail,
                    FullName = "Khach hang FoodieTTTM",
                    PhoneNumber = "0900000000",
                    EmailConfirmed = true,
                    IsEmailVerified = true,
                    LoyaltyLevel = "Báº¡c"
                };

                var result = await userManager.CreateAsync(customer, "User@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customer, "Customer");
                }
            }
            else
            {
                customer.EmailConfirmed = true;
                customer.IsEmailVerified = true;
                if (string.IsNullOrWhiteSpace(customer.FullName))
                {
                    customer.FullName = "Khach hang FoodieTTTM";
                }
                if (string.IsNullOrWhiteSpace(customer.LoyaltyLevel))
                {
                    customer.LoyaltyLevel = "Báº¡c";
                }

                await userManager.UpdateAsync(customer);
            }

            if (customer != null && !await userManager.IsInRoleAsync(customer, "Customer"))
            {
                await userManager.AddToRoleAsync(customer, "Customer");
            }

            if (admin != null)
            {
                admin.LoyaltyLevel = "Bạch kim";
                await userManager.UpdateAsync(admin);
            }

            if (customer != null)
            {
                customer.FullName = string.IsNullOrWhiteSpace(customer.FullName) ? "Khách hàng FoodieTTTM" : customer.FullName;
                customer.EmailConfirmed = true;
                customer.IsEmailVerified = true;
                customer.LoyaltyLevel = "Thành viên";
                await userManager.UpdateAsync(customer);
            }
        }

        private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private static async Task EnsureDiscountCodeLoyaltyColumnsAsync(ApplicationDbContext db)
        {
            await db.Database.ExecuteSqlRawAsync("""
                IF COL_LENGTH('DiscountCodes', 'RequiredLoyaltyLevel') IS NULL
                    ALTER TABLE [DiscountCodes] ADD [RequiredLoyaltyLevel] nvarchar(32) NULL;
                """);
        }

        private static async Task EnsureUserAvatarColumnAsync(ApplicationDbContext db)
        {
            await db.Database.ExecuteSqlRawAsync("""
                IF COL_LENGTH('AspNetUsers', 'AvatarUrl') IS NULL
                    ALTER TABLE [AspNetUsers] ADD [AvatarUrl] nvarchar(max) NOT NULL CONSTRAINT [DF_AspNetUsers_AvatarUrl] DEFAULT N'';
                """);
        }

        private static async Task EnsureTableReservationsTableAsync(ApplicationDbContext db)
        {
            await db.Database.ExecuteSqlRawAsync("""
                IF OBJECT_ID(N'[TableReservations]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [TableReservations] (
                        [Id] int NOT NULL IDENTITY,
                        [RestaurantTableId] int NOT NULL,
                        [StartAt] datetime2 NOT NULL,
                        [DurationMinutes] int NOT NULL,
                        [CustomerName] nvarchar(max) NOT NULL,
                        [PhoneNumber] nvarchar(max) NOT NULL,
                        [Note] nvarchar(max) NULL,
                        [Status] int NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_TableReservations] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_TableReservations_RestaurantTables_RestaurantTableId] FOREIGN KEY ([RestaurantTableId]) REFERENCES [RestaurantTables] ([Id]) ON DELETE CASCADE
                    );

                    CREATE INDEX [IX_TableReservations_RestaurantTableId] ON [TableReservations] ([RestaurantTableId]);
                END
                """);
        }

        private static async Task EnsureFoodItemMenuColumnsAsync(ApplicationDbContext db)
        {
            await db.Database.ExecuteSqlRawAsync("""
                IF COL_LENGTH('FoodItems', 'MainCategory') IS NULL
                    ALTER TABLE [FoodItems] ADD [MainCategory] nvarchar(max) NOT NULL CONSTRAINT [DF_FoodItems_MainCategory] DEFAULT N'';

                IF COL_LENGTH('FoodItems', 'Subcategory') IS NULL
                    ALTER TABLE [FoodItems] ADD [Subcategory] nvarchar(max) NOT NULL CONSTRAINT [DF_FoodItems_Subcategory] DEFAULT N'';

                IF COL_LENGTH('FoodItems', 'AllergyNote') IS NULL
                    ALTER TABLE [FoodItems] ADD [AllergyNote] nvarchar(max) NOT NULL CONSTRAINT [DF_FoodItems_AllergyNote] DEFAULT N'';

                IF COL_LENGTH('FoodItems', 'DetailDescription') IS NULL
                    ALTER TABLE [FoodItems] ADD [DetailDescription] nvarchar(max) NOT NULL CONSTRAINT [DF_FoodItems_DetailDescription] DEFAULT N'';

                IF COL_LENGTH('FoodItems', 'DiscountPrice') IS NULL
                    ALTER TABLE [FoodItems] ADD [DiscountPrice] decimal(18,2) NULL;

                IF COL_LENGTH('FoodItems', 'Calories') IS NULL
                    ALTER TABLE [FoodItems] ADD [Calories] int NOT NULL CONSTRAINT [DF_FoodItems_Calories] DEFAULT 0;

                IF COL_LENGTH('FoodItems', 'ServingSize') IS NULL
                    ALTER TABLE [FoodItems] ADD [ServingSize] nvarchar(max) NOT NULL CONSTRAINT [DF_FoodItems_ServingSize] DEFAULT N'';

                IF COL_LENGTH('FoodItems', 'Allergens') IS NULL
                    ALTER TABLE [FoodItems] ADD [Allergens] nvarchar(max) NOT NULL CONSTRAINT [DF_FoodItems_Allergens] DEFAULT N'';

                IF COL_LENGTH('FoodItems', 'SpiceLevel') IS NULL
                    ALTER TABLE [FoodItems] ADD [SpiceLevel] nvarchar(max) NOT NULL CONSTRAINT [DF_FoodItems_SpiceLevel] DEFAULT N'';

                IF COL_LENGTH('FoodItems', 'IsAvailable') IS NULL
                    ALTER TABLE [FoodItems] ADD [IsAvailable] bit NOT NULL CONSTRAINT [DF_FoodItems_IsAvailable] DEFAULT 1;

                IF COL_LENGTH('FoodItems', 'IsFeatured') IS NULL
                    ALTER TABLE [FoodItems] ADD [IsFeatured] bit NOT NULL CONSTRAINT [DF_FoodItems_IsFeatured] DEFAULT 0;

                IF COL_LENGTH('FoodItems', 'IsVegetarian') IS NULL
                    ALTER TABLE [FoodItems] ADD [IsVegetarian] bit NOT NULL CONSTRAINT [DF_FoodItems_IsVegetarian] DEFAULT 0;
                """);
        }

        private static async Task EnsureOrderDeliveryColumnsAsync(ApplicationDbContext db)
        {
            await db.Database.ExecuteSqlRawAsync("""
                IF COL_LENGTH('Orders', 'TrackingCode') IS NULL
                    ALTER TABLE [Orders] ADD [TrackingCode] nvarchar(max) NOT NULL CONSTRAINT [DF_Orders_TrackingCode] DEFAULT N'';

                IF COL_LENGTH('Orders', 'DeliveryStatus') IS NULL
                    ALTER TABLE [Orders] ADD [DeliveryStatus] int NOT NULL CONSTRAINT [DF_Orders_DeliveryStatus] DEFAULT 1;

                IF COL_LENGTH('Orders', 'ShipperName') IS NULL
                    ALTER TABLE [Orders] ADD [ShipperName] nvarchar(max) NOT NULL CONSTRAINT [DF_Orders_ShipperName] DEFAULT N'';

                IF COL_LENGTH('Orders', 'ShipperPhone') IS NULL
                    ALTER TABLE [Orders] ADD [ShipperPhone] nvarchar(max) NOT NULL CONSTRAINT [DF_Orders_ShipperPhone] DEFAULT N'';

                IF COL_LENGTH('Orders', 'DeliveryNote') IS NULL
                    ALTER TABLE [Orders] ADD [DeliveryNote] nvarchar(max) NOT NULL CONSTRAINT [DF_Orders_DeliveryNote] DEFAULT N'';

                IF COL_LENGTH('Orders', 'ShippedAt') IS NULL
                    ALTER TABLE [Orders] ADD [ShippedAt] datetime2 NULL;

                IF COL_LENGTH('Orders', 'DeliveredAt') IS NULL
                    ALTER TABLE [Orders] ADD [DeliveredAt] datetime2 NULL;

                IF COL_LENGTH('Orders', 'UpdatedAt') IS NULL
                    ALTER TABLE [Orders] ADD [UpdatedAt] datetime2 NOT NULL CONSTRAINT [DF_Orders_UpdatedAt] DEFAULT SYSUTCDATETIME();

                UPDATE [Orders]
                SET [OrderCode] = CONCAT(N'FDLEGACY', [Id])
                WHERE [OrderCode] IS NULL OR LTRIM(RTRIM([OrderCode])) = N'';

                UPDATE [Orders]
                SET [OrderCode] = CONCAT(N'FDLEGACY', [Id])
                WHERE LEN([OrderCode]) > 64;

                ;WITH DuplicateOrderCodes AS (
                    SELECT [Id],
                           ROW_NUMBER() OVER (PARTITION BY [OrderCode] ORDER BY [Id]) AS RowNumber
                    FROM [Orders]
                )
                UPDATE o
                SET [OrderCode] = CONCAT(N'FDLEGACY', o.[Id])
                FROM [Orders] o
                INNER JOIN DuplicateOrderCodes d ON d.[Id] = o.[Id]
                WHERE d.RowNumber > 1;

                UPDATE [Orders]
                SET [TrackingCode] = CONCAT(N'VDLEGACY', [Id])
                WHERE [TrackingCode] IS NULL OR LTRIM(RTRIM([TrackingCode])) = N'' OR LEN([TrackingCode]) > 64;

                ;WITH DuplicateTrackingCodes AS (
                    SELECT [Id],
                           ROW_NUMBER() OVER (PARTITION BY [TrackingCode] ORDER BY [Id]) AS RowNumber
                    FROM [Orders]
                )
                UPDATE o
                SET [TrackingCode] = CONCAT(N'VDLEGACY', o.[Id])
                FROM [Orders] o
                INNER JOIN DuplicateTrackingCodes d ON d.[Id] = o.[Id]
                WHERE d.RowNumber > 1;

                IF EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID(N'[Orders]')
                      AND name = N'OrderCode'
                      AND (max_length = -1 OR max_length > 128)
                )
                    ALTER TABLE [Orders] ALTER COLUMN [OrderCode] nvarchar(64) NOT NULL;

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Orders_OrderCode' AND object_id = OBJECT_ID(N'[Orders]'))
                    CREATE UNIQUE INDEX [IX_Orders_OrderCode] ON [Orders] ([OrderCode]);
                """);
        }

        private static async Task EnsureShipmentTablesAsync(ApplicationDbContext db)
        {
            await db.Database.ExecuteSqlRawAsync("""
                IF OBJECT_ID(N'[Shipments]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [Shipments] (
                        [Id] int NOT NULL IDENTITY,
                        [OrderId] int NOT NULL,
                        [ShipmentCode] nvarchar(64) NOT NULL,
                        [OrderCode] nvarchar(max) NOT NULL CONSTRAINT [DF_Shipments_OrderCode] DEFAULT N'',
                        [CustomerName] nvarchar(max) NOT NULL CONSTRAINT [DF_Shipments_CustomerName] DEFAULT N'',
                        [CustomerPhone] nvarchar(max) NOT NULL CONSTRAINT [DF_Shipments_CustomerPhone] DEFAULT N'',
                        [DeliveryAddress] nvarchar(max) NOT NULL CONSTRAINT [DF_Shipments_DeliveryAddress] DEFAULT N'',
                        [ShipperName] nvarchar(max) NOT NULL CONSTRAINT [DF_Shipments_ShipperName] DEFAULT N'',
                        [ShipperPhone] nvarchar(max) NOT NULL CONSTRAINT [DF_Shipments_ShipperPhone] DEFAULT N'',
                        [DeliveryNote] nvarchar(max) NOT NULL CONSTRAINT [DF_Shipments_DeliveryNote] DEFAULT N'',
                        [DeliveryStatus] int NOT NULL,
                        [DeliveryStartedAt] datetime2 NULL,
                        [DeliveredAt] datetime2 NULL,
                        [UpdatedAt] datetime2 NOT NULL CONSTRAINT [DF_Shipments_UpdatedAt] DEFAULT SYSUTCDATETIME(),
                        CONSTRAINT [PK_Shipments] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Shipments_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE
                    );

                    CREATE UNIQUE INDEX [IX_Shipments_OrderId] ON [Shipments] ([OrderId]);
                    CREATE UNIQUE INDEX [IX_Shipments_ShipmentCode] ON [Shipments] ([ShipmentCode]);
                END

                IF OBJECT_ID(N'[OrderStatusHistories]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [OrderStatusHistories] (
                        [Id] int NOT NULL IDENTITY,
                        [OrderId] int NOT NULL,
                        [OrderStatus] int NOT NULL,
                        [DeliveryStatus] int NOT NULL,
                        [Note] nvarchar(max) NOT NULL CONSTRAINT [DF_OrderStatusHistories_Note] DEFAULT N'',
                        [UpdatedBy] nvarchar(max) NOT NULL CONSTRAINT [DF_OrderStatusHistories_UpdatedBy] DEFAULT N'',
                        [CreatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_OrderStatusHistories] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_OrderStatusHistories_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE
                    );

                    CREATE INDEX [IX_OrderStatusHistories_OrderId] ON [OrderStatusHistories] ([OrderId]);
                END

                INSERT INTO [Shipments] (
                    [OrderId], [ShipmentCode], [OrderCode], [CustomerName], [CustomerPhone], [DeliveryAddress],
                    [ShipperName], [ShipperPhone], [DeliveryNote], [DeliveryStatus], [DeliveryStartedAt], [DeliveredAt], [UpdatedAt]
                )
                SELECT
                    o.[Id],
                    CASE WHEN ISNULL(o.[TrackingCode], N'') = N'' THEN CONCAT(N'VD', o.[OrderCode]) ELSE o.[TrackingCode] END,
                    o.[OrderCode],
                    o.[CustomerName],
                    o.[PhoneNumber],
                    o.[Address],
                    ISNULL(o.[ShipperName], N''),
                    ISNULL(o.[ShipperPhone], N''),
                    ISNULL(o.[DeliveryNote], N''),
                    o.[DeliveryStatus],
                    o.[ShippedAt],
                    o.[DeliveredAt],
                    o.[UpdatedAt]
                FROM [Orders] o
                WHERE NOT EXISTS (SELECT 1 FROM [Shipments] s WHERE s.[OrderId] = o.[Id]);
                """);
        }

        private static async Task EnsureRecipeCatalogColumnsAsync(ApplicationDbContext db)
        {
            await db.Database.ExecuteSqlRawAsync("""
                IF COL_LENGTH('Recipes', 'Category') IS NULL
                    ALTER TABLE [Recipes] ADD [Category] nvarchar(max) NOT NULL CONSTRAINT [DF_Recipes_Category] DEFAULT N'';

                IF COL_LENGTH('Recipes', 'CategoryLabel') IS NULL
                    ALTER TABLE [Recipes] ADD [CategoryLabel] nvarchar(max) NOT NULL CONSTRAINT [DF_Recipes_CategoryLabel] DEFAULT N'';

                IF COL_LENGTH('Recipes', 'PrepTime') IS NULL
                    ALTER TABLE [Recipes] ADD [PrepTime] nvarchar(max) NOT NULL CONSTRAINT [DF_Recipes_PrepTime] DEFAULT N'';

                IF COL_LENGTH('Recipes', 'CookTime') IS NULL
                    ALTER TABLE [Recipes] ADD [CookTime] nvarchar(max) NOT NULL CONSTRAINT [DF_Recipes_CookTime] DEFAULT N'';

                IF COL_LENGTH('Recipes', 'Servings') IS NULL
                    ALTER TABLE [Recipes] ADD [Servings] nvarchar(max) NOT NULL CONSTRAINT [DF_Recipes_Servings] DEFAULT N'';

                IF COL_LENGTH('Recipes', 'Tips') IS NULL
                    ALTER TABLE [Recipes] ADD [Tips] nvarchar(max) NOT NULL CONSTRAINT [DF_Recipes_Tips] DEFAULT N'';

                IF COL_LENGTH('Recipes', 'SafetyNote') IS NULL
                    ALTER TABLE [Recipes] ADD [SafetyNote] nvarchar(max) NOT NULL CONSTRAINT [DF_Recipes_SafetyNote] DEFAULT N'';

                IF COL_LENGTH('Recipes', 'AllergyNote') IS NULL
                    ALTER TABLE [Recipes] ADD [AllergyNote] nvarchar(max) NOT NULL CONSTRAINT [DF_Recipes_AllergyNote] DEFAULT N'';
                """);
        }

        private static async Task UpsertMenuCatalogAsync(ApplicationDbContext db)
        {
            var categories = new[]
            {
                new FoodCategory { Name = "Món mặn", Slug = "mon-man", Description = "Cơm phần, món nước, món xào, món gà, món cá", SortOrder = 1 },
                new FoodCategory { Name = "Món chay", Slug = "mon-chay-moi", Description = "Cơm chay, món nước chay, món đậu, món cuốn, món lẩu", SortOrder = 2 },
                new FoodCategory { Name = "Healthy", Slug = "healthy-moi", Description = "Salad, cơm healthy, protein bowl, soup, ăn nhẹ", SortOrder = 3 },
                new FoodCategory { Name = "Đồ uống", Slug = "do-uong-moi", Description = "Trà, nước ép, sinh tố, sữa hạt, healthy drink", SortOrder = 4 }
            };

            foreach (var category in categories)
            {
                var existing = await db.FoodCategories.FirstOrDefaultAsync(x => x.Slug == category.Slug);
                if (existing == null)
                {
                    db.FoodCategories.Add(category);
                }
                else
                {
                    existing.Name = category.Name;
                    existing.Description = category.Description;
                    existing.SortOrder = category.SortOrder;
                    existing.IsActive = true;
                }
            }

            await db.SaveChangesAsync();

            var categoryIds = await db.FoodCategories
                .Where(x => x.Slug == "com" || x.Slug == "mon-man" || x.Slug == "mon-chay-moi" || x.Slug == "healthy-moi" || x.Slug == "do-uong-moi")
                .ToDictionaryAsync(x => x.Slug, x => x.Id);

            var riceCategoryId = categoryIds["com"];
            var savoryCategoryId = categoryIds["mon-man"];
            var vegetarianCategoryId = categoryIds["mon-chay-moi"];
            var healthyCategoryId = categoryIds["healthy-moi"];
            var drinkCategoryId = categoryIds["do-uong-moi"];

            const string allergyNote = "Món có thể chứa hải sản, đậu nành, sữa, trứng, gluten, mè hoặc đậu phộng. Nếu quý khách dị ứng với bất kỳ thành phần nào, chúng tôi khuyến nghị nên chọn món khác hoặc liên hệ nhân viên để được tư vấn.";
            var items = new[]
            {
                MenuItem(savoryCategoryId, "Cơm gà xối mỡ", "com-ga-xoi-mo", "Món mặn", "Món gà", 69000, "/assets/images/menu/mon-man/com-ga-xoi-mo.jpg", "Cơm nóng dùng với gà da giòn, rau dưa và nước mắm chua ngọt.", "Cơm gà xối mỡ được chọn cho bữa trưa cần no bụng nhưng vẫn quen vị Việt.", "Cơm trắng, đùi gà, dưa leo, rau thơm, nước mắm chua ngọt", allergyNote),
                MenuItem(riceCategoryId, "Cơm sườn nướng", "com-suon-nuong", "Món mặn", "Cơm phần", 55000, "/images/foods/com-suon-nuong.jpg", "Cơm nóng ăn kèm sườn heo nướng mềm thơm, trứng ốp la, đồ chua và nước mắm pha.", "Cơm sườn nướng là món ăn quen thuộc với phần sườn heo được tẩm ướp gia vị và nướng vàng thơm. Món ăn dùng cùng cơm trắng, trứng ốp la, dưa leo, cà chua, đồ chua và nước mắm chua ngọt, phù hợp cho bữa trưa hoặc bữa tối.", "Cơm trắng, sườn heo, trứng gà, dưa leo, cà chua, đồ chua, nước mắm, tỏi, hành tím, đường, tiêu, dầu ăn, gia vị ướp", allergyNote),
                MenuItem(savoryCategoryId, "Bún bò Huế", "bun-bo-hue", "Món mặn", "Món nước", 79000, "/assets/images/menu/mon-man/bun-bo-hue.jpg", "Bún bò nước dùng đậm vị, có bò, chả và rau ăn kèm.", "Món nước ấm bụng cho những ngày cần hương vị đậm đà.", "Bún, thịt bò, chả, sả, rau sống", allergyNote),
                MenuItem(savoryCategoryId, "Phở bò", "pho-bo", "Món mặn", "Món nước", 75000, "/assets/images/menu/mon-man/pho-bo.jpg", "Phở bò nước trong, thơm nhẹ, dùng với rau thơm và chanh.", "Phở bò là lựa chọn an toàn cho bữa sáng hoặc bữa tối nhẹ.", "Bánh phở, thịt bò, hành tây, hành lá, rau thơm", allergyNote),
                MenuItem(savoryCategoryId, "Mì xào hải sản", "mi-xao-hai-san", "Món mặn", "Món xào", 85000, "/assets/images/menu/mon-man/mi-xao-hai-san.jpg", "Mì xào cùng tôm, mực và rau củ, vị đậm vừa ăn.", "Món nhanh nhiều rau và có vị biển nhẹ.", "Mì, tôm, mực, cải thìa, cà rốt, nước sốt xào", allergyNote),
                MenuItem(savoryCategoryId, "Gà sốt mật ong", "ga-sot-mat-ong", "Món mặn", "Món gà", 76000, "/assets/images/menu/mon-man/ga-sot-mat-ong.jpg", "Gà chiên phủ sốt mật ong nhẹ, vị ngọt mặn dễ ăn.", "Món hợp nhóm bạn hoặc bữa ăn gia đình nhỏ.", "Thịt gà, mật ong, mè, tỏi, gia vị", allergyNote),
                MenuItem(savoryCategoryId, "Cá hồi áp chảo", "ca-hoi-ap-chao", "Món mặn", "Món cá", 129000, "/assets/images/menu/mon-man/ca-hoi-ap-chao.jpg", "Cá hồi áp chảo dùng với rau củ và sốt chanh nhẹ.", "Lựa chọn món mặn thanh hơn, giữ độ mềm của cá.", "Cá hồi, bông cải, cà rốt, sốt chanh, tiêu", allergyNote),
                MenuItem(savoryCategoryId, "Cá basa kho tộ", "ca-basa-kho-to", "Món mặn", "Món cá", 78000, "/assets/images/menu/mon-man/ca-basa-kho-to.jpg", "Cá basa kho tộ vị đậm, dùng cùng cơm nóng và rau luộc.", "Món cơm gia đình quen thuộc, hợp khách muốn món mặn dễ ăn.", "Cá basa, nước mắm, tiêu, hành tím, ớt, nước màu", allergyNote),
                MenuItem(vegetarianCategoryId, "Cơm chay thập cẩm", "com-chay-thap-cam", "Món chay", "Cơm chay", 59000, "/assets/images/menu/mon-chay/com-chay-thap-cam.jpg", "Cơm chay với rau củ, nấm và đậu hũ sốt nhẹ.", "Món thanh đạm cho ngày rằm hoặc bữa ăn nhẹ.", "Cơm trắng, đậu hũ, nấm, cà rốt, rau cải", allergyNote),
                MenuItem(vegetarianCategoryId, "Bún chay nấm", "bun-chay-nam", "Món chay", "Món nước chay", 62000, "/assets/images/menu/mon-chay/bun-chay-nam.jpg", "Bún nước chay nấu từ nấm, rau củ và đậu hũ.", "Nước dùng lấy vị ngọt từ rau củ, nhẹ bụng.", "Bún, nấm rơm, nấm kim châm, đậu hũ, rau thơm", allergyNote),
                MenuItem(vegetarianCategoryId, "Đậu hũ sốt cà", "dau-hu-sot-ca", "Món chay", "Món đậu", 49000, "/assets/images/menu/mon-chay/dau-hu-sot-ca.jpg", "Đậu hũ mềm nấu cùng sốt cà chua tươi.", "Món chay gần gũi, dễ ăn cùng cơm nóng.", "Đậu hũ, cà chua, hành lá, dầu thực vật, gia vị chay", allergyNote),
                MenuItem(vegetarianCategoryId, "Gỏi cuốn chay", "goi-cuon-chay", "Món chay", "Món cuốn", 52000, "/assets/images/menu/mon-chay/goi-cuon-chay.jpg", "Gỏi cuốn rau củ, bún, đậu hũ và nước chấm chay.", "Món khai vị tươi mát, dễ chịu.", "Bánh tráng, bún, đậu hũ, xà lách, rau thơm", allergyNote),
                MenuItem(vegetarianCategoryId, "Mì xào rau củ", "mi-xao-rau-cu", "Món chay", "Món nước chay", 58000, "/assets/images/menu/mon-chay/mi-xao-rau-cu.jpg", "Mì xào chay với nấm, cải, cà rốt và sốt đậu nành.", "Món chay nhanh nhưng vẫn đủ năng lượng.", "Mì, nấm, cải thìa, cà rốt, nước tương", allergyNote),
                MenuItem(vegetarianCategoryId, "Lẩu nấm chay", "lau-nam-chay", "Món chay", "Món lẩu", 159000, "/assets/images/menu/mon-chay/lau-nam-chay.jpg", "Lẩu chay nhiều loại nấm, rau xanh và đậu hũ.", "Phù hợp đi nhóm trong những ngày muốn ăn thanh đạm.", "Nấm tổng hợp, rau xanh, đậu hũ, bắp, mì hoặc bún", allergyNote),
                MenuItem(healthyCategoryId, "Salad ức gà", "salad-uc-ga-menu", "Healthy", "Salad", 69000, "/assets/images/menu/healthy/salad-uc-ga.jpg", "Salad rau xanh với ức gà áp chảo và sốt mè rang.", "Lựa chọn cân bằng cho bữa trưa văn phòng.", "Ức gà, xà lách, cà chua bi, dưa leo, sốt mè rang", allergyNote),
                MenuItem(healthyCategoryId, "Cơm gạo lứt ức gà", "com-gao-lut-uc-ga", "Healthy", "Cơm healthy", 79000, "/assets/images/menu/healthy/com-gao-lut-uc-ga.jpg", "Gạo lứt, ức gà, rau luộc và sốt nhẹ.", "Món ưu tiên nguyên liệu đơn giản, ít dầu.", "Gạo lứt, ức gà, bông cải, cà rốt, sốt sữa chua", allergyNote),
                MenuItem(healthyCategoryId, "Bowl cá hồi", "bowl-ca-hoi", "Healthy", "Protein bowl", 119000, "/assets/images/menu/healthy/bowl-ca-hoi.jpg", "Bowl cá hồi cùng rau củ, bơ và cơm nền nhẹ.", "Kết hợp protein, chất béo tốt và rau củ.", "Cá hồi, cơm nền, bơ, dưa leo, rong biển", allergyNote),
                MenuItem(healthyCategoryId, "Salad cá ngừ", "salad-ca-ngu", "Healthy", "Salad", 74000, "/assets/images/menu/healthy/salad-ca-ngu.jpg", "Salad cá ngừ, rau xanh, bắp và sốt chanh dầu olive.", "Món nhiều protein và tươi, vị chanh nhẹ.", "Cá ngừ, xà lách, bắp ngọt, cà chua, dầu olive", allergyNote),
                MenuItem(healthyCategoryId, "Yến mạch trái cây", "yen-mach-trai-cay", "Healthy", "Ăn nhẹ", 55000, "/assets/images/menu/healthy/yen-mach-trai-cay.jpg", "Yến mạch dùng với sữa chua, chuối và trái cây theo mùa.", "Món nhẹ cho buổi sáng hoặc xế chiều.", "Yến mạch, sữa chua, chuối, dâu, hạt dinh dưỡng", allergyNote),
                MenuItem(healthyCategoryId, "Soup bí đỏ", "soup-bi-do", "Healthy", "Soup", 59000, "/assets/images/menu/healthy/soup-bi-do.jpg", "Soup bí đỏ mịn, ấm bụng, dùng kèm bánh mì nướng.", "Món ấm nhẹ với vị ngọt tự nhiên từ bí đỏ.", "Bí đỏ, sữa tươi, hành tây, bánh mì, tiêu", allergyNote),
                MenuItem(drinkCategoryId, "Trà đào cam sả", "tra-dao-cam-sa", "Đồ uống", "Trà", 45000, "/assets/images/menu/do-uong/tra-dao-cam-sa.png", "Trà đào thơm cam sả, vị chua ngọt dễ uống.", "Món giải nhiệt hợp với món chiên hoặc món cay.", "Trà, đào, cam, sả, đường", allergyNote),
                MenuItem(drinkCategoryId, "Trà chanh mật ong", "tra-chanh-mat-ong", "Đồ uống", "Trà", 39000, "/assets/images/menu/do-uong/tra-chanh-mat-ong.jpg", "Trà chanh pha mật ong, vị thanh và thơm nhẹ.", "Lựa chọn nhẹ nhàng sau bữa ăn.", "Trà, chanh, mật ong, đá, lá bạc hà", allergyNote),
                MenuItem(drinkCategoryId, "Nước ép cam", "nuoc-ep-cam", "Đồ uống", "Nước ép", 42000, "/assets/images/menu/do-uong/nuoc-ep-cam.jpg", "Nước cam ép tươi, có thể giảm đường theo yêu cầu.", "Món uống quen thuộc, làm dịu vị sau món mặn.", "Cam tươi, đá, đường tùy chọn", allergyNote),
                MenuItem(drinkCategoryId, "Sinh tố bơ", "sinh-to-bo", "Đồ uống", "Sinh tố", 49000, "/assets/images/menu/do-uong/sinh-to-bo.jpg", "Sinh tố bơ béo nhẹ, xay mịn, vị ngọt vừa.", "Món uống no nhẹ, mịn và dễ chịu.", "Bơ, sữa đặc, sữa tươi, đá", allergyNote),
                MenuItem(drinkCategoryId, "Sữa đậu nành", "sua-dau-nanh", "Đồ uống", "Sữa hạt", 35000, "/assets/images/menu/do-uong/sua-dau-nanh.jpg", "Sữa đậu nành thơm nhẹ, dùng nóng hoặc lạnh.", "Món uống đơn giản, hợp bữa sáng hoặc món chay.", "Đậu nành, nước lọc, đường tùy chọn", allergyNote),
                MenuItem(drinkCategoryId, "Nước ép cần tây táo", "nuoc-ep-can-tay-tao", "Đồ uống", "Healthy drink", 52000, "/assets/images/menu/do-uong/nuoc-ep-can-tay-tao.jpg", "Nước ép cần tây kết hợp táo, vị xanh nhẹ và dễ uống.", "Đồ uống healthy, táo giúp vị cần tây mềm hơn.", "Cần tây, táo, chanh, đá", allergyNote)
            };

            foreach (var item in items)
            {
                ApplyMenuDetails(item);
            }

            foreach (var item in items)
            {
                var existing = await db.FoodItems.FirstOrDefaultAsync(x => x.Slug == item.Slug);
                if (existing == null)
                {
                    db.FoodItems.Add(item);
                }
                else
                {
                    existing.FoodCategoryId = item.FoodCategoryId;
                    existing.Name = item.Name;
                    existing.Slug = item.Slug;
                    existing.Category = item.Category;
                    existing.MainCategory = item.MainCategory;
                    existing.Subcategory = item.Subcategory;
                    existing.Price = item.Price;
                    existing.DiscountPrice = item.DiscountPrice;
                    existing.Tag = item.Tag;
                    existing.ImageUrl = item.ImageUrl;
                    existing.Description = item.Description;
                    existing.DetailDescription = item.DetailDescription;
                    existing.Ingredients = item.Ingredients;
                    existing.Calories = item.Calories;
                    existing.ServingSize = item.ServingSize;
                    existing.Story = item.Story;
                    existing.AllergyNote = item.AllergyNote;
                    existing.Allergens = item.Allergens;
                    existing.SpiceLevel = item.SpiceLevel;
                    existing.IsAvailable = item.IsAvailable;
                    existing.IsFeatured = item.IsFeatured;
                    existing.IsVegetarian = item.IsVegetarian;
                    existing.IsBestSeller = item.IsBestSeller;
                    existing.IsActive = true;
                    existing.UpdatedAt = DateTime.UtcNow;
                }
            }

            await db.SaveChangesAsync();
        }

        private static FoodItem MenuItem(
            int categoryId,
            string name,
            string slug,
            string mainCategory,
            string subcategory,
            decimal price,
            string imageUrl,
            string description,
            string story,
            string ingredients,
            string allergyNote)
        {
            return new FoodItem
            {
                FoodCategoryId = categoryId,
                Name = name,
                Slug = slug,
                Category = mainCategory,
                MainCategory = mainCategory,
                Subcategory = subcategory,
                Price = price,
                Tag = subcategory,
                ImageUrl = imageUrl,
                Description = description,
                Ingredients = ingredients,
                DetailDescription = story,
                Calories = 520,
                ServingSize = "1 người",
                Story = story,
                AllergyNote = allergyNote,
                Allergens = allergyNote,
                SpiceLevel = "Không cay",
                IsAvailable = true,
                IsFeatured = slug is "com-ga-xoi-mo" or "com-suon-nuong" or "bun-bo-hue" or "ga-sot-mat-ong" or "salad-uc-ga-menu" or "tra-dao-cam-sa",
                IsVegetarian = mainCategory == "Món chay",
                IsBestSeller = slug is "com-ga-xoi-mo" or "com-suon-nuong" or "bun-bo-hue" or "ga-sot-mat-ong" or "salad-uc-ga-menu" or "tra-dao-cam-sa",
                IsActive = true
            };
        }

        private static void ApplyMenuDetails(FoodItem item)
        {
            var details = item.Slug switch
            {
                "com-ga-xoi-mo" => (
                    "Cơm nóng ăn cùng gà xối mỡ da giòn, dưa leo, đồ chua và nước mắm pha vừa miệng.",
                    "Phần gà được chiên/xối mỡ đến khi lớp da vàng giòn nhưng thịt vẫn mềm. Món hợp bữa trưa cần no nhanh, vị đậm vừa và dễ ăn với cơm nóng.",
                    "Cơm trắng, đùi gà, dưa leo, đồ chua, mỡ hành, nước mắm chua ngọt"),
                "com-suon-nuong" => (
                    "Cơm nóng ăn kèm sườn heo nướng mềm thơm, trứng ốp la, đồ chua và nước mắm pha.",
                    "Cơm sườn nướng là món ăn quen thuộc với phần sườn heo được tẩm ướp gia vị và nướng vàng thơm. Món ăn dùng cùng cơm trắng, trứng ốp la, dưa leo, cà chua, đồ chua và nước mắm chua ngọt, phù hợp cho bữa trưa hoặc bữa tối.",
                    "Cơm trắng, sườn heo, trứng gà, dưa leo, cà chua, đồ chua, nước mắm, tỏi, hành tím, đường, tiêu, dầu ăn, gia vị ướp"),
                "bun-bo-hue" => (
                    "Bún bò nước dùng đậm vị sả, ăn cùng thịt bò, chả, rau sống và sa tế tùy khẩu vị.",
                    "Tô bún được nấu theo hướng thơm sả, cay nhẹ và nhiều topping. Phù hợp khi khách muốn món nước nóng, đậm đà và no lâu.",
                    "Bún, thịt bò, chả, sả, hành tây, rau sống, sa tế"),
                "pho-bo" => (
                    "Phở bò nước trong, thơm quế hồi nhẹ, dùng với rau thơm, hành tây, chanh và tương.",
                    "Nước dùng được giữ vị thanh để dễ ăn vào buổi sáng hoặc tối. Món phù hợp cả khách muốn ăn nhẹ lẫn khách cần một tô nóng đủ năng lượng.",
                    "Bánh phở, thịt bò, hành tây, hành lá, rau thơm, chanh"),
                "mi-xao-hai-san" => (
                    "Mì xào lửa lớn với tôm, mực, rau cải và sốt xào đậm vừa.",
                    "Sợi mì được đảo nhanh để không bở, rau giữ độ giòn và hải sản có vị ngọt tự nhiên. Món hợp cho khách thích đồ xào nóng, thơm và nhiều topping.",
                    "Mì, tôm, mực, cải thìa, cà rốt, hành tây, sốt xào"),
                "ga-sot-mat-ong" => (
                    "Gà chiên phủ sốt mật ong tỏi, vị ngọt mặn nhẹ, dùng cùng cơm hoặc ăn riêng đều hợp.",
                    "Sốt mật ong được nấu sánh để bám đều miếng gà. Món phù hợp nhóm bạn, trẻ em và khách thích vị dễ ăn nhưng vẫn có điểm nhấn.",
                    "Thịt gà, mật ong, tỏi, mè, nước tương, gia vị"),
                "ca-hoi-ap-chao" => (
                    "Cá hồi áp chảo dùng với rau củ và sốt chanh tiêu nhẹ.",
                    "Miếng cá được áp chảo vừa chín để giữ độ mềm và vị béo tự nhiên. Đây là lựa chọn cao cấp hơn cho khách muốn món mặn nhưng thanh, không quá nặng bụng.",
                    "Cá hồi, bông cải, cà rốt, sốt chanh, tiêu, dầu olive"),
                "ca-basa-kho-to" => (
                    "Cá basa kho tộ vị mặn ngọt đậm đà, dùng cùng cơm nóng và rau luộc.",
                    "Nước kho được giữ sánh vừa để chan cơm. Món gợi cảm giác bữa cơm gia đình, hợp khách thích vị Việt quen thuộc.",
                    "Cá basa, nước mắm, tiêu, hành tím, ớt, nước màu"),
                "com-chay-thap-cam" => (
                    "Cơm chay với đậu hũ, nấm, rau củ xào và sốt chay nhẹ.",
                    "Món tập trung vào vị tự nhiên của rau củ, phù hợp ngày rằm hoặc những hôm muốn ăn thanh. Phần cơm vẫn đủ no nhưng không nặng dầu.",
                    "Cơm trắng, đậu hũ, nấm, cà rốt, rau cải, sốt chay"),
                "bun-chay-nam" => (
                    "Bún nước chay nấu từ nấm, rau củ và đậu hũ, vị ngọt thanh.",
                    "Nước dùng lấy vị từ nấm và củ quả thay vì nêm quá gắt. Món hợp khi muốn ăn nóng nhưng vẫn nhẹ bụng.",
                    "Bún, nấm rơm, nấm kim châm, đậu hũ, cà rốt, rau thơm"),
                "dau-hu-sot-ca" => (
                    "Đậu hũ mềm nấu cùng sốt cà chua tươi, dùng ngon nhất với cơm nóng.",
                    "Vị chua nhẹ của cà giúp món chay dễ ăn hơn. Đây là lựa chọn đơn giản, quen thuộc và phù hợp nhiều lứa tuổi.",
                    "Đậu hũ, cà chua, hành lá, dầu thực vật, gia vị chay"),
                "goi-cuon-chay" => (
                    "Gỏi cuốn rau củ, bún, đậu hũ và nước chấm chay.",
                    "Món giữ độ tươi của rau, phù hợp khai vị hoặc bữa nhẹ. Cuốn mềm, mát và không gây ngấy.",
                    "Bánh tráng, bún, đậu hũ, xà lách, rau thơm, nước chấm chay"),
                "mi-xao-rau-cu" => (
                    "Mì xào chay với nấm, cải, cà rốt và sốt đậu nành.",
                    "Rau củ được xào vừa chín để còn độ giòn. Món dành cho khách muốn món chay nhanh, no và thơm mùi xào.",
                    "Mì, nấm, cải thìa, cà rốt, nước tương, tỏi"),
                "lau-nam-chay" => (
                    "Lẩu chay nhiều loại nấm, rau xanh, đậu hũ và nước dùng rau củ.",
                    "Phù hợp đi nhóm hoặc dùng vào ngày muốn ăn thanh đạm. Nước lẩu có vị ngọt tự nhiên từ nấm và bắp.",
                    "Nấm tổng hợp, rau xanh, đậu hũ, bắp, mì hoặc bún, nước dùng rau củ"),
                "salad-uc-ga-menu" => (
                    "Salad rau xanh với ức gà áp chảo và sốt mè rang.",
                    "Ức gà cung cấp protein, rau xanh tạo cảm giác nhẹ bụng. Món hợp bữa trưa văn phòng hoặc khách đang kiểm soát khẩu phần.",
                    "Ức gà, xà lách, cà chua bi, dưa leo, bắp, sốt mè rang"),
                "com-gao-lut-uc-ga" => (
                    "Gạo lứt, ức gà, rau luộc và sốt nhẹ.",
                    "Món ưu tiên nguyên liệu ít dầu, dễ tính calo và vẫn đủ no. Phù hợp khách ăn healthy nhưng không muốn bỏ tinh bột.",
                    "Gạo lứt, ức gà, bông cải, cà rốt, sốt sữa chua"),
                "bowl-ca-hoi" => (
                    "Bowl cá hồi cùng rau củ, bơ, rong biển và cơm nền nhẹ.",
                    "Món cân bằng giữa protein, chất béo tốt và rau. Phù hợp khách muốn ăn no nhưng vẫn tươi và sạch vị.",
                    "Cá hồi, cơm nền, bơ, dưa leo, rong biển, rau xanh"),
                "salad-ca-ngu" => (
                    "Salad cá ngừ, rau xanh, bắp ngọt và sốt chanh dầu olive.",
                    "Vị chanh nhẹ giúp món sáng miệng, cá ngừ bổ sung protein. Đây là lựa chọn hợp cho bữa nhẹ sau giờ học hoặc làm việc.",
                    "Cá ngừ, xà lách, bắp ngọt, cà chua, dầu olive, chanh"),
                "yen-mach-trai-cay" => (
                    "Yến mạch dùng với sữa chua, chuối và trái cây theo mùa.",
                    "Món nhẹ cho buổi sáng hoặc xế chiều, có vị ngọt tự nhiên từ trái cây. Có thể dùng như món tráng miệng lành mạnh.",
                    "Yến mạch, sữa chua, chuối, dâu, hạt dinh dưỡng"),
                "soup-bi-do" => (
                    "Soup bí đỏ mịn, ấm bụng, dùng kèm bánh mì nướng.",
                    "Bí đỏ tạo vị ngọt tự nhiên và kết cấu mịn. Món hợp khi muốn ăn nhẹ, ấm và dễ tiêu.",
                    "Bí đỏ, sữa tươi, hành tây, bánh mì, tiêu"),
                "tra-dao-cam-sa" => (
                    "Trà đào thơm cam sả, vị chua ngọt dễ uống.",
                    "Hương sả làm ly trà tươi hơn, hợp dùng kèm món chiên hoặc món cay. Có thể giảm ngọt theo yêu cầu.",
                    "Trà, đào, cam, sả, đường, đá"),
                "tra-chanh-mat-ong" => (
                    "Trà chanh pha mật ong, vị thanh và thơm nhẹ.",
                    "Lựa chọn nhẹ nhàng sau bữa ăn, vị chanh giúp cân bằng các món nhiều đạm. Mật ong tạo hậu vị dịu.",
                    "Trà, chanh, mật ong, đá, lá bạc hà"),
                "nuoc-ep-cam" => (
                    "Nước cam ép tươi, có thể giảm đường theo yêu cầu.",
                    "Ly nước quen thuộc, hợp bữa sáng hoặc sau món mặn. Cam tươi đem lại vị chua ngọt tự nhiên.",
                    "Cam tươi, đá, đường tùy chọn"),
                "sinh-to-bo" => (
                    "Sinh tố bơ béo nhẹ, xay mịn, vị ngọt vừa.",
                    "Bơ chín tạo độ mịn và cảm giác no nhẹ. Món phù hợp dùng xế chiều hoặc sau bữa ăn.",
                    "Bơ, sữa đặc, sữa tươi, đá"),
                "sua-dau-nanh" => (
                    "Sữa đậu nành thơm nhẹ, dùng nóng hoặc lạnh.",
                    "Món uống đơn giản, hợp bữa sáng hoặc món chay. Vị được giữ vừa phải để dễ dùng hằng ngày.",
                    "Đậu nành, nước lọc, đường tùy chọn"),
                "nuoc-ep-can-tay-tao" => (
                    "Nước ép cần tây kết hợp táo, vị xanh nhẹ và dễ uống.",
                    "Táo giúp vị cần tây mềm hơn, phù hợp khách thích đồ uống healthy. Có thể dùng sau bữa ăn để tạo cảm giác tươi.",
                    "Cần tây, táo, chanh, đá"),
                _ => (item.Description, item.Story, item.Ingredients)
            };

            item.Description = details.Item1;
            item.Story = details.Item2;
            item.DetailDescription = details.Item2;
            item.Ingredients = details.Item3;
            item.ServingSize = "1 người";
            item.SpiceLevel = "Không cay";
            item.Allergens = item.Slug switch
            {
                "com-suon-nuong" => "Có thể chứa trứng gà, nước mắm/cá, tỏi, hành tím và các gia vị ướp. Người dị ứng với trứng, hải sản/cá hoặc gia vị lên men cần lưu ý.",
                "com-ga-xoi-mo" => "Có thể chứa nước mắm/cá, tỏi, hành, gluten hoặc đậu nành nếu phần nước chấm/sốt có dùng nước tương.",
                "bun-bo-hue" => "Có thể chứa nước mắm/cá, bò, chả có phụ gia, sả và ớt. Người dị ứng cá/hải sản hoặc nhạy cảm với gia vị cay cần lưu ý.",
                "pho-bo" => "Có thịt bò và gia vị nước dùng như quế, hồi, hành. Bánh phở thường từ gạo nhưng nước dùng có thể chứa nước mắm/cá.",
                "mi-xao-hai-san" => "Có chứa hải sản như tôm, mực; mì có thể chứa gluten; sốt xào có thể chứa đậu nành và gluten.",
                "ga-sot-mat-ong" => "Có thịt gà, mật ong, tỏi, mè và nước tương. Có thể chứa mè, đậu nành và gluten.",
                "ca-hoi-ap-chao" => "Có chứa cá hồi. Sốt chanh có thể có bơ hoặc sữa tùy cách chế biến; người dị ứng cá cần tránh.",
                "ca-basa-kho-to" => "Có chứa cá basa, nước mắm/cá, hành tím, tiêu và ớt. Người dị ứng cá/hải sản hoặc gia vị cay cần lưu ý.",
                "com-chay-thap-cam" => "Có đậu hũ/đậu nành, nấm và rau củ. Sốt chay có thể chứa đậu nành hoặc gluten.",
                "bun-chay-nam" => "Có nấm, đậu hũ/đậu nành và rau thơm. Người dị ứng nấm hoặc đậu nành cần lưu ý.",
                "dau-hu-sot-ca" => "Có chứa đậu hũ/đậu nành, cà chua và hành lá. Người dị ứng đậu nành cần lưu ý.",
                "goi-cuon-chay" => "Có bánh tráng, bún, đậu hũ/đậu nành và rau sống. Nước chấm chay có thể chứa đậu phộng, mè hoặc đậu nành.",
                "mi-xao-rau-cu" => "Mì có thể chứa gluten; nước tương có thể chứa đậu nành và gluten; món có nấm và tỏi.",
                "lau-nam-chay" => "Có nhiều loại nấm, đậu hũ/đậu nành, bắp và rau. Mì ăn kèm có thể chứa gluten.",
                "salad-uc-ga-menu" => "Có thịt gà; sốt mè rang có thể chứa mè, đậu nành, trứng hoặc gluten tùy loại sốt.",
                "com-gao-lut-uc-ga" => "Có thịt gà; sốt sữa chua có chứa sữa hoặc chế phẩm từ sữa.",
                "bowl-ca-hoi" => "Có chứa cá hồi và rong biển. Sốt ăn kèm có thể chứa đậu nành, mè hoặc gluten.",
                "salad-ca-ngu" => "Có chứa cá ngừ. Sốt chanh dầu olive thường không có sữa nhưng người dị ứng cá cần tránh.",
                "yen-mach-trai-cay" => "Có yến mạch, sữa chua và hạt dinh dưỡng. Có thể chứa sữa, gluten hoặc các loại hạt.",
                "soup-bi-do" => "Có sữa tươi hoặc chế phẩm từ sữa; bánh mì ăn kèm có thể chứa gluten.",
                "tra-dao-cam-sa" => "Có đào, cam và sả. Người dị ứng đào, cam/quýt hoặc sả cần lưu ý.",
                "tra-chanh-mat-ong" => "Có trà, chanh và mật ong. Người dị ứng mật ong hoặc nhạy cảm với chanh/quýt cần lưu ý.",
                "nuoc-ep-cam" => "Có cam tươi. Người dị ứng hoặc nhạy cảm với cam/quýt cần lưu ý.",
                "sinh-to-bo" => "Có bơ, sữa đặc và sữa tươi; chứa sữa hoặc chế phẩm từ sữa.",
                "sua-dau-nanh" => "Có chứa đậu nành. Người dị ứng đậu nành cần tránh.",
                "nuoc-ep-can-tay-tao" => "Có cần tây, táo và chanh. Người dị ứng cần tây, táo hoặc chanh/quýt cần lưu ý.",
                _ => item.AllergyNote
            };
            item.AllergyNote = item.Allergens;
            item.IsAvailable = true;
            item.IsFeatured = item.IsBestSeller;
            item.IsVegetarian = item.MainCategory == "Món chay";
            item.Calories = item.Slug switch
            {
                "com-suon-nuong" => 650,
                "com-ga-xoi-mo" => 720,
                "bun-bo-hue" => 680,
                "pho-bo" => 560,
                "mi-xao-hai-san" => 700,
                "ga-sot-mat-ong" => 610,
                "ca-hoi-ap-chao" => 520,
                "ca-basa-kho-to" => 590,
                "com-chay-thap-cam" => 480,
                "bun-chay-nam" => 430,
                "dau-hu-sot-ca" => 360,
                "goi-cuon-chay" => 300,
                "mi-xao-rau-cu" => 520,
                "lau-nam-chay" => 620,
                "salad-uc-ga-menu" => 420,
                "com-gao-lut-uc-ga" => 540,
                "bowl-ca-hoi" => 610,
                "salad-ca-ngu" => 390,
                "yen-mach-trai-cay" => 350,
                "soup-bi-do" => 260,
                "tra-dao-cam-sa" => 160,
                "tra-chanh-mat-ong" => 120,
                "nuoc-ep-cam" => 140,
                "sinh-to-bo" => 320,
                "sua-dau-nanh" => 150,
                "nuoc-ep-can-tay-tao" => 110,
                _ => item.Calories
            };

            if (item.Slug == "com-suon-nuong")
            {
                item.DetailDescription = "Cơm sườn nướng là món ăn quen thuộc với phần sườn heo được tẩm ướp gia vị và nướng vàng thơm. Món ăn dùng cùng cơm trắng, trứng ốp la, dưa leo, cà chua, đồ chua và nước mắm chua ngọt, phù hợp cho bữa trưa hoặc bữa tối.";
                item.SpiceLevel = "Không cay hoặc cay nhẹ tùy nước mắm";
                item.Allergens = "Có thể chứa trứng gà, nước mắm/cá, tỏi, hành tím và các gia vị ướp. Người dị ứng với trứng, hải sản/cá hoặc gia vị lên men cần lưu ý.";
                item.AllergyNote = item.Allergens;
                item.IsFeatured = true;
                item.IsVegetarian = false;
            }
        }

        private static async Task UpsertRecipeCatalogAsync(ApplicationDbContext db)
        {
            var safety = "Luôn rửa tay trước khi nấu, rửa sạch nguyên liệu, nấu chín kỹ thịt/cá/trứng/hải sản và không dùng thực phẩm có mùi lạ.";
            var recipes = new[]
            {
                RecipeItem("Trứng chiên hành", "trung-chien-hanh", "mon-nhanh", "Món nhanh", "/assets/images/recipes/mon-man/trung-chien-hanh.jpg", "Dễ", "5 phút", "7 phút", "2 người", "Món trứng chiên đơn giản, phù hợp bữa cơm nhanh.", "3 quả trứng gà;2 nhánh hành lá;Nước mắm;Tiêu;Dầu ăn", "Rửa hành lá, cắt nhỏ.;Đập trứng, thêm hành, nước mắm và tiêu rồi đánh đều.;Làm nóng chảo với ít dầu.;Chiên lửa vừa đến khi vàng hai mặt.;Dùng khi còn nóng.", "Chiên lửa vừa để trứng chín đều.", safety, "Món có trứng."),
                RecipeItem("Canh rau ngót thịt bằm", "canh-rau-ngot-thit-bam", "mon-man", "Món mặn", "/assets/images/recipes/mon-man/canh-rau-ngot-thit-bam.jpg", "Dễ", "10 phút", "15 phút", "3 người", "Canh rau ngót thịt bằm thanh nhẹ, hợp bữa cơm gia đình.", "Rau ngót;Thịt heo bằm;Hành tím;Muối;Nước mắm;Tiêu", "Rửa rau ngót nhiều lần.;Ướp thịt bằm với hành tím và gia vị.;Đun nước sôi, cho thịt vào khuấy tơi.;Cho rau vào nấu mềm.;Nêm lại vừa ăn.", "Vò nhẹ rau trước khi nấu để rau mềm hơn.", safety, "Có thể chứa nước mắm."),
                RecipeItem("Cơm chiên trứng", "com-chien-trung", "mon-nhanh", "Món nhanh", "/assets/images/recipes/mon-man/com-chien-trung.jpg", "Dễ", "8 phút", "12 phút", "2 người", "Tận dụng cơm nguội, trứng và rau củ cho bữa nhanh.", "Cơm nguội;Trứng;Cà rốt;Hành lá;Dầu ăn;Nước mắm;Tiêu", "Rửa và cắt nhỏ rau củ.;Đánh tan trứng.;Đảo trứng tơi.;Cho cơm và rau củ vào rang săn.;Nêm gia vị và thêm hành lá.", "Dùng cơm nguội để hạt cơm tơi.", safety, "Có trứng; có thể chứa gluten nếu dùng nước tương."),
                RecipeItem("Gà kho gừng", "ga-kho-gung", "mon-man", "Món mặn", "/assets/images/recipes/mon-man/ga-kho-gung.jpg", "Trung bình", "15 phút", "25 phút", "3 người", "Gà kho gừng thơm ấm, hợp ăn cùng cơm nóng.", "Thịt gà;Gừng;Hành tím;Nước mắm;Đường;Tiêu", "Rửa gà và để ráo.;Cắt gừng sợi.;Ướp gà với gia vị.;Phi thơm gừng, đảo săn gà.;Kho nhỏ lửa đến khi gà chín kỹ.", "Kho lửa nhỏ để thịt thấm đều.", safety, "Có thể chứa nước mắm."),
                RecipeItem("Đậu hũ sốt cà chua", "dau-hu-sot-ca-chua", "mon-chay", "Món chay", "/assets/images/recipes/mon-chay/dau-hu-sot-ca-chua.jpg", "Dễ", "8 phút", "15 phút", "2 người", "Đậu hũ mềm sốt cà chua, dễ ăn cùng cơm nóng.", "Đậu hũ;Cà chua;Hành lá;Dầu ăn;Muối;Tiêu", "Rửa cà chua và hành lá.;Cắt đậu hũ vừa ăn.;Xào cà chua thành sốt.;Cho đậu hũ vào nêm vừa ăn.;Đun nhỏ lửa vài phút.", "Đảo nhẹ tay để đậu không nát.", safety, "Có đậu nành."),
                RecipeItem("Rau muống xào tỏi", "rau-muong-xao-toi", "mon-chay", "Món chay", "/assets/images/recipes/mon-chay/rau-muong-xao-toi.jpg", "Dễ", "10 phút", "7 phút", "3 người", "Rau muống xanh giòn, xào nhanh với tỏi thơm.", "Rau muống;Tỏi;Dầu ăn;Muối;Hạt nêm chay", "Nhặt và rửa rau kỹ.;Đập dập tỏi.;Phi thơm tỏi.;Cho rau vào xào lửa lớn.;Nêm vừa ăn và tắt bếp.", "Xào nhanh lửa lớn để rau xanh.", safety, "Thường không có nhóm dị ứng phổ biến."),
                RecipeItem("Canh bí đỏ thịt bằm", "canh-bi-do-thit-bam", "mon-man", "Món mặn", "/assets/images/recipes/mon-man/canh-bi-do-thit-bam.jpg", "Dễ", "12 phút", "18 phút", "3 người", "Canh bí đỏ ngọt nhẹ, thịt bằm mềm.", "Bí đỏ;Thịt bằm;Hành lá;Muối;Nước mắm;Tiêu", "Gọt và rửa bí đỏ.;Ướp thịt bằm.;Đun nước, cho thịt vào khuấy tơi.;Cho bí vào nấu mềm.;Nêm và thêm hành lá.", "Không cắt bí quá nhỏ để tránh nát.", safety, "Có thể chứa nước mắm."),
                RecipeItem("Mì xào rau củ", "mi-xao-rau-cu-recipe", "mon-nhanh", "Món nhanh", "/assets/images/recipes/mon-chay/mi-xao-rau-cu.jpg", "Dễ", "10 phút", "10 phút", "2 người", "Mì xào nhanh với rau củ cho bữa tối đơn giản.", "Mì;Cải thìa;Cà rốt;Nấm;Tỏi;Nước tương", "Rửa rau củ và nấm.;Trụng mì vừa mềm.;Phi thơm tỏi.;Xào rau củ.;Thêm mì và nước tương, đảo đều.", "Không trụng mì quá lâu.", safety, "Có thể chứa gluten và đậu nành."),
                RecipeItem("Salad ức gà", "salad-uc-ga-recipe", "healthy", "Healthy", "/assets/images/recipes/healthy/salad-uc-ga.jpg", "Dễ", "12 phút", "12 phút", "2 người", "Salad rau xanh với ức gà áp chảo, nhẹ bụng và đủ protein.", "Ức gà;Xà lách;Cà chua bi;Dưa leo;Sốt mè rang", "Rửa rau kỹ và để ráo.;Ướp ức gà.;Áp chảo gà chín hoàn toàn.;Cắt gà lát mỏng.;Trộn rau, gà và sốt.", "Để gà nghỉ vài phút trước khi cắt.", safety, "Sốt mè có thể chứa mè, đậu nành hoặc gluten."),
                RecipeItem("Cơm gạo lứt ức gà", "com-gao-lut-uc-ga-recipe", "healthy", "Healthy", "/assets/images/recipes/healthy/com-gao-lut-uc-ga.jpg", "Trung bình", "15 phút", "30 phút", "2 người", "Bữa healthy gồm gạo lứt, ức gà và rau củ.", "Gạo lứt;Ức gà;Bông cải;Cà rốt;Dầu olive", "Nấu gạo lứt.;Rửa và cắt rau củ.;Ướp ức gà.;Áp chảo hoặc luộc gà chín kỹ.;Bày cùng rau luộc.", "Ngâm gạo lứt trước khi nấu để mềm hơn.", safety, "Tùy sốt ăn kèm có thể chứa sữa hoặc đậu nành."),
                RecipeItem("Yến mạch trái cây", "yen-mach-trai-cay-recipe", "healthy", "Healthy", "/assets/images/recipes/healthy/yen-mach-trai-cay.jpg", "Dễ", "10 phút", "0 phút", "1 người", "Bữa sáng nhanh với yến mạch, sữa chua và trái cây.", "Yến mạch;Sữa chua;Chuối;Dâu hoặc táo;Hạt dinh dưỡng", "Rửa trái cây.;Cắt trái cây.;Cho yến mạch vào tô.;Thêm sữa chua và trái cây.;Trộn đều và dùng.", "Có thể ngâm yến mạch qua đêm.", safety, "Có thể chứa sữa, gluten hoặc hạt."),
                RecipeItem("Soup bí đỏ", "soup-bi-do-recipe", "healthy", "Healthy", "/assets/images/recipes/healthy/soup-bi-do.jpg", "Dễ", "12 phút", "20 phút", "2 người", "Soup bí đỏ mịn, ấm bụng.", "Bí đỏ;Hành tây;Sữa tươi;Nước dùng;Muối;Tiêu", "Rửa và cắt bí đỏ.;Xào hành tây.;Nấu bí mềm.;Xay mịn.;Thêm sữa và nêm lại.", "Thêm sữa sau khi bí mềm.", safety, "Có sữa."),
                RecipeItem("Trà chanh mật ong", "tra-chanh-mat-ong-recipe", "do-uong", "Đồ uống", "/assets/images/recipes/do-uong/tra-chanh-mat-ong.jpg", "Dễ", "5 phút", "5 phút", "1 người", "Trà chanh mật ong thanh nhẹ, dễ pha tại nhà.", "Trà;Chanh;Mật ong;Nước nóng;Đá", "Rửa sạch chanh.;Hãm trà.;Để trà bớt nóng rồi thêm mật ong.;Vắt chanh và khuấy đều.;Thêm đá nếu muốn.", "Không cho mật ong vào nước quá sôi.", safety, "Mật ong không phù hợp cho trẻ dưới 1 tuổi."),
                RecipeItem("Nước ép cam", "nuoc-ep-cam-recipe", "do-uong", "Đồ uống", "/assets/images/recipes/do-uong/nuoc-ep-cam.jpg", "Dễ", "7 phút", "0 phút", "1 người", "Nước cam ép tươi, vị chua ngọt tự nhiên.", "Cam tươi;Đá;Đường tùy chọn", "Rửa sạch vỏ cam.;Cắt đôi cam.;Ép lấy nước.;Nêm ngọt nếu cần.;Dùng ngay.", "Chọn cam chắc tay, vỏ không mốc.", safety, "Người nhạy cảm với cam/quýt nên dùng lượng nhỏ trước."),
                RecipeItem("Sinh tố bơ", "sinh-to-bo-recipe", "do-uong", "Đồ uống", "/assets/images/recipes/do-uong/sinh-to-bo.jpg", "Dễ", "8 phút", "0 phút", "1 người", "Sinh tố bơ béo nhẹ, xay mịn và dễ uống.", "Bơ chín;Sữa tươi;Sữa đặc;Đá", "Rửa vỏ bơ.;Bổ bơ, bỏ hạt.;Cho nguyên liệu vào máy xay.;Xay mịn.;Dùng ngay.", "Dùng bơ chín vừa để sinh tố mịn.", safety, "Có sữa."),
                RecipeItem("Sữa đậu nành", "sua-dau-nanh-recipe", "do-uong", "Đồ uống", "/assets/images/recipes/do-uong/sua-dau-nanh.jpg", "Trung bình", "8 giờ ngâm đậu", "25 phút", "4 người", "Sữa đậu nành tự nấu, thơm nhẹ.", "Đậu nành;Nước;Đường tùy khẩu vị;Lá dứa tùy chọn", "Nhặt và ngâm đậu.;Rửa lại đậu.;Xay với nước và lọc.;Đun sữa, khuấy thường xuyên.;Nấu sôi kỹ rồi thêm đường.", "Khuấy đều để sữa không khét đáy.", safety, "Có đậu nành.")
            };

            foreach (var recipe in recipes)
            {
                var existing = await db.Recipes.FirstOrDefaultAsync(x => x.Slug == recipe.Slug);
                if (existing == null)
                {
                    db.Recipes.Add(recipe);
                }
                else
                {
                    existing.Title = recipe.Title;
                    existing.Category = recipe.Category;
                    existing.CategoryLabel = recipe.CategoryLabel;
                    existing.Description = recipe.Description;
                    existing.Ingredients = recipe.Ingredients;
                    existing.Steps = recipe.Steps;
                    existing.ImageUrl = recipe.ImageUrl;
                    existing.Difficulty = recipe.Difficulty;
                    existing.PrepTime = recipe.PrepTime;
                    existing.CookTime = recipe.CookTime;
                    existing.Servings = recipe.Servings;
                    existing.Tips = recipe.Tips;
                    existing.SafetyNote = recipe.SafetyNote;
                    existing.AllergyNote = recipe.AllergyNote;
                    existing.CookingTimeMinutes = recipe.CookingTimeMinutes;
                    existing.IsActive = true;
                }
            }

            await db.SaveChangesAsync();
        }

        private static Recipe RecipeItem(
            string title,
            string slug,
            string category,
            string categoryLabel,
            string imageUrl,
            string difficulty,
            string prepTime,
            string cookTime,
            string servings,
            string description,
            string ingredients,
            string steps,
            string tips,
            string safetyNote,
            string allergyNote)
        {
            var cookMinutes = int.TryParse(new string(cookTime.TakeWhile(char.IsDigit).ToArray()), out var minutes) ? minutes : 0;
            return new Recipe
            {
                Title = title,
                Slug = slug,
                Category = category,
                CategoryLabel = categoryLabel,
                ImageUrl = imageUrl,
                Difficulty = difficulty,
                PrepTime = prepTime,
                CookTime = cookTime,
                Servings = servings,
                Description = description,
                Ingredients = ingredients,
                Steps = steps,
                Tips = tips,
                SafetyNote = safetyNote,
                AllergyNote = allergyNote,
                CookingTimeMinutes = cookMinutes,
                IsActive = true
            };
        }

        private static async Task EnsureChatMessagesTableAsync(ApplicationDbContext db)
        {
            await db.Database.ExecuteSqlRawAsync("""
IF OBJECT_ID(N'[ChatMessages]', N'U') IS NULL
BEGIN
    CREATE TABLE [ChatMessages](
        [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ChatMessages] PRIMARY KEY,
        [UserId] nvarchar(450) NULL,
        [SessionId] nvarchar(128) NOT NULL,
        [Role] nvarchar(32) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [Intent] nvarchar(64) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [PageUrl] nvarchar(max) NULL,
        [MetadataJson] nvarchar(max) NULL,
        CONSTRAINT [FK_ChatMessages_AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE SET NULL
    );
    CREATE INDEX [IX_ChatMessages_UserId_SessionId_CreatedAt] ON [ChatMessages]([UserId], [SessionId], [CreatedAt]);
END
""");
        }
    }
}

