# FoodieLab Database

## Yêu cầu môi trường

- .NET 8 SDK
- SQL Server LocalDB hoặc SQL Server
- Visual Studio 2022 hoặc VS Code

## Project đang dùng database gì?

Project dùng SQL Server qua Entity Framework Core:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DACS_Food;Trusted_Connection=True;MultipleActiveResultSets=true"
```

Project hiện chưa có thư mục `Migrations/`. Schema và dữ liệu demo được tạo khi app khởi động qua `DatabaseSeeder.InitializeAsync()`.

## Cách tạo database bằng SQL Server Management Studio

1. Mở SQL Server Management Studio.
2. Kết nối tới SQL Server hoặc LocalDB.
3. Mở file `Database/FoodieLab_FullDatabase.sql`.
4. Chạy script để tạo database `DACS_Food`.
5. Chạy project một lần để seeder tạo bảng và dữ liệu mẫu.

Sau khi app chạy lần đầu, có thể chạy thêm `Database/FoodieLab_SeedData.sql` nếu cần bổ sung dữ liệu mẫu tối thiểu. Script này idempotent, chạy lại nhiều lần không cố ý nhân đôi dữ liệu.

## Cách cấu hình connection string

Copy `appsettings.example.json` thành `appsettings.json` nếu máy clone chưa có file cấu hình riêng, sau đó sửa connection string theo SQL Server của bạn:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=DACS_Food;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

Với LocalDB có thể dùng:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DACS_Food;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

## Cách chạy project

```bash
dotnet restore
dotnet build
dotnet run
```

Khi app khởi động, `Program.cs` gọi `DatabaseSeeder.InitializeAsync()`. Seeder sẽ:

- Tạo schema nếu database đang trống.
- Bổ sung các bảng/cột nghiệp vụ còn thiếu.
- Tạo role `Admin` và `Customer`.
- Tạo tài khoản admin demo.
- Seed món ăn, công thức, bàn đặt chỗ, mã giảm giá và dữ liệu demo khác.

## Tài khoản admin demo

- Email: `admin@foodielab.local`
- Mật khẩu: `Admin@123`

Mật khẩu này chỉ dùng cho môi trường học tập/demo.

## Dùng dotnet ef database update

Hiện project chưa có EF migrations nên `dotnet ef database update` không phải luồng chính. Nếu sau này thêm migration, có thể dùng:

```bash
dotnet ef database update
dotnet ef migrations script --idempotent --output Database/FoodieLab_EF_Migrations_Idempotent.sql
```

Hiện tại file `FoodieLab_EF_Migrations_Idempotent.sql` chỉ ghi chú trạng thái này để người clone project không nhầm là thiếu script.

## Reset database

Trong SQL Server Management Studio:

```sql
ALTER DATABASE [DACS_Food] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE [DACS_Food];
```

Sau đó chạy lại `FoodieLab_FullDatabase.sql` và chạy app một lần.
