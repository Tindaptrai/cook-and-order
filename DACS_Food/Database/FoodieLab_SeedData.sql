/*
    FoodieLab seed data script
    Run this after the app has created the schema, or simply rely on DatabaseSeeder.
    All inserts are idempotent.
*/

USE [DACS_Food];
GO

IF OBJECT_ID(N'[FoodCategories]', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [FoodCategories] WHERE [Slug] = N'com')
        INSERT INTO [FoodCategories] ([Name], [Slug], [Description], [IsActive], [SortOrder])
        VALUES (N'Cơm', N'com', N'Các món cơm no bụng', 1, 1);

    IF NOT EXISTS (SELECT 1 FROM [FoodCategories] WHERE [Slug] = N'mi')
        INSERT INTO [FoodCategories] ([Name], [Slug], [Description], [IsActive], [SortOrder])
        VALUES (N'Mì', N'mi', N'Mì và món sợi', 1, 2);

    IF NOT EXISTS (SELECT 1 FROM [FoodCategories] WHERE [Slug] = N'mon-chay')
        INSERT INTO [FoodCategories] ([Name], [Slug], [Description], [IsActive], [SortOrder])
        VALUES (N'Món chay', N'mon-chay', N'Món nhẹ, ít dầu mỡ', 1, 3);

    IF NOT EXISTS (SELECT 1 FROM [FoodCategories] WHERE [Slug] = N'healthy')
        INSERT INTO [FoodCategories] ([Name], [Slug], [Description], [IsActive], [SortOrder])
        VALUES (N'Healthy', N'healthy', N'Món cân bằng dinh dưỡng', 1, 4);

    IF NOT EXISTS (SELECT 1 FROM [FoodCategories] WHERE [Slug] = N'do-uong')
        INSERT INTO [FoodCategories] ([Name], [Slug], [Description], [IsActive], [SortOrder])
        VALUES (N'Đồ uống', N'do-uong', N'Nước uống', 1, 8);
END
GO

IF OBJECT_ID(N'[FoodItems]', N'U') IS NOT NULL AND OBJECT_ID(N'[FoodCategories]', N'U') IS NOT NULL
BEGIN
    DECLARE @ComId int = (SELECT TOP 1 [Id] FROM [FoodCategories] WHERE [Slug] = N'com');
    DECLARE @ChayId int = (SELECT TOP 1 [Id] FROM [FoodCategories] WHERE [Slug] = N'mon-chay');
    DECLARE @HealthyId int = (SELECT TOP 1 [Id] FROM [FoodCategories] WHERE [Slug] = N'healthy');

    IF @ComId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [FoodItems] WHERE [Slug] = N'com-ga-sot-tieu-den')
        INSERT INTO [FoodItems]
            ([FoodCategoryId], [Name], [Slug], [Category], [Price], [Tag], [IsBestSeller],
             [ImageUrl], [Description], [Ingredients], [Story], [MainCategory], [Subcategory], [AllergyNote], [IsActive], [CreatedAt])
        VALUES
            (@ComId, N'Cơm gà sốt tiêu đen', N'com-ga-sot-tieu-den', N'cơm', 59000, N'Best seller', 1,
             N'https://images.unsplash.com/photo-1603133872878-684f208fb84b?auto=format&fit=crop&w=800&q=80',
             N'Cơm nóng ăn cùng gà sốt tiêu đen đậm vị.',
             N'Cơm, gà, tiêu đen, rau ăn kèm',
             N'Món no bụng, hợp bữa trưa văn phòng.',
             N'Món mặn', N'Cơm phần', N'Có thể chứa đậu nành, gluten hoặc mè.', 1, SYSUTCDATETIME());

    IF @ChayId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [FoodItems] WHERE [Slug] = N'com-chay-nam')
        INSERT INTO [FoodItems]
            ([FoodCategoryId], [Name], [Slug], [Category], [Price], [Tag], [IsBestSeller],
             [ImageUrl], [Description], [Ingredients], [Story], [MainCategory], [Subcategory], [AllergyNote], [IsActive], [CreatedAt])
        VALUES
            (@ChayId, N'Cơm chay nấm', N'com-chay-nam', N'món chay', 49000, N'Thanh đạm', 1,
             N'https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=800&q=80',
             N'Cơm chay với nấm và rau củ.',
             N'Cơm, nấm, rau củ, nước tương',
             N'Phù hợp ngày ăn nhẹ hoặc ăn chay.',
             N'Món chay', N'Cơm chay', N'Có đậu nành.', 1, SYSUTCDATETIME());

    IF @HealthyId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [FoodItems] WHERE [Slug] = N'salad-uc-ga-menu')
        INSERT INTO [FoodItems]
            ([FoodCategoryId], [Name], [Slug], [Category], [Price], [Tag], [IsBestSeller],
             [ImageUrl], [Description], [Ingredients], [Story], [MainCategory], [Subcategory], [AllergyNote], [IsActive], [CreatedAt])
        VALUES
            (@HealthyId, N'Salad ức gà', N'salad-uc-ga-menu', N'healthy', 69000, N'Healthy', 0,
             N'/assets/images/menu/healthy/salad-uc-ga.jpg',
             N'Salad rau xanh với ức gà áp chảo và sốt mè rang.',
             N'Ức gà, xà lách, cà chua bi, dưa leo, sốt mè rang',
             N'Lựa chọn cân bằng cho bữa trưa văn phòng.',
             N'Healthy', N'Salad', N'Sốt mè có thể chứa mè, đậu nành hoặc gluten.', 1, SYSUTCDATETIME());
END
GO

IF OBJECT_ID(N'[RestaurantTables]', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [RestaurantTables])
    BEGIN
        INSERT INTO [RestaurantTables] ([TableNumber], [Name], [TableType], [Capacity], [Status], [IsActive])
        VALUES
            (1, N'Bàn private 1', 2, 6, 1, 1),
            (2, N'Bàn private 2', 2, 6, 1, 1),
            (3, N'Bàn thường 3', 1, 4, 1, 1),
            (4, N'Bàn thường 4', 1, 4, 1, 1),
            (5, N'Bàn thường 5', 1, 4, 1, 1),
            (6, N'Bàn thường 6', 1, 4, 1, 1);
    END
END
GO

IF OBJECT_ID(N'[DiscountCodes]', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [DiscountCodes] WHERE [Code] = N'GIOVANG10')
        INSERT INTO [DiscountCodes]
            ([Code], [Name], [DiscountType], [DiscountValue], [MinOrderAmount], [MaxDiscountAmount],
             [DiscountScope], [IsActive], [StartAt], [EndAt], [UsageLimit], [UsedCount])
        VALUES
            (N'GIOVANG10', N'Giảm giá giờ vàng', 1, 10, 50000, 30000, 2, 1, NULL, NULL, NULL, 0);

    IF NOT EXISTS (SELECT 1 FROM [DiscountCodes] WHERE [Code] = N'THANTHIET15')
        INSERT INTO [DiscountCodes]
            ([Code], [Name], [DiscountType], [DiscountValue], [MinOrderAmount], [MaxDiscountAmount],
             [DiscountScope], [IsActive], [StartAt], [EndAt], [UsageLimit], [UsedCount])
        VALUES
            (N'THANTHIET15', N'Ưu đãi thành viên thân thiết', 1, 15, 80000, 50000, 3, 1, NULL, NULL, NULL, 0);
END
GO

PRINT N'Seed script finished.';
GO
