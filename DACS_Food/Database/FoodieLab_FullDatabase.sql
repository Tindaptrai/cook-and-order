/*
    FoodieLab database bootstrap script
    Database engine: SQL Server / SQL Server LocalDB

    Project note:
    This project currently does not contain EF Core migration files. The app creates
    the schema with ApplicationDbContext + DatabaseSeeder.InitializeAsync() on startup.
    Run this script first to create the database, then run the app once to let the
    seeder create tables and demo data.
*/

IF DB_ID(N'DACS_Food') IS NULL
BEGIN
    CREATE DATABASE [DACS_Food];
END
GO

USE [DACS_Food];
GO

PRINT N'Database DACS_Food is ready.';
PRINT N'Next step: run the ASP.NET Core app once so DatabaseSeeder can create schema and seed demo data.';
GO

/*
    Optional idempotent seed helpers.
    These statements run only if the EF-created tables already exist.
    If you run this script before starting the app, they are skipped safely.
*/

IF OBJECT_ID(N'[FoodCategories]', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [FoodCategories] WHERE [Slug] = N'com')
        INSERT INTO [FoodCategories] ([Name], [Slug], [Description], [IsActive], [SortOrder])
        VALUES (N'Cơm', N'com', N'Các món cơm no bụng', 1, 1);

    IF NOT EXISTS (SELECT 1 FROM [FoodCategories] WHERE [Slug] = N'mon-chay')
        INSERT INTO [FoodCategories] ([Name], [Slug], [Description], [IsActive], [SortOrder])
        VALUES (N'Món chay', N'mon-chay', N'Món nhẹ, ít dầu mỡ', 1, 3);

    IF NOT EXISTS (SELECT 1 FROM [FoodCategories] WHERE [Slug] = N'do-uong')
        INSERT INTO [FoodCategories] ([Name], [Slug], [Description], [IsActive], [SortOrder])
        VALUES (N'Đồ uống', N'do-uong', N'Nước uống', 1, 8);
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

PRINT N'Bootstrap script finished.';
GO
