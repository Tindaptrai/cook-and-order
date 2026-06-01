# Hướng Dẫn Sử Dụng Project FoodieLab

## 1. Yêu cầu cài đặt

- .NET 8 SDK
- SQL Server LocalDB hoặc SQL Server
- Visual Studio 2022 hoặc VS Code
- SQL Server Management Studio nếu muốn chạy script database thủ công

## 2. Mở project

Mở thư mục project trong Visual Studio hoặc VS Code:

```text
DACS_Food
```

File project chính:

```text
DACS_Food.csproj
```

## 3. Cấu hình database

Project đang dùng SQL Server qua connection string trong `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DACS_Food;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

Nếu máy dùng SQL Server thường, có thể sửa thành:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=DACS_Food;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

## 4. Tạo database

Cách khuyến nghị:

1. Mở SQL Server Management Studio.
2. Chạy file:

```text
Database/FoodieLab_FullDatabase.sql
```

3. Chạy project lần đầu. Khi app khởi động, `DatabaseSeeder` sẽ tự tạo bảng và dữ liệu mẫu nếu database đang trống.

Có thể đọc hướng dẫn chi tiết hơn tại:

```text
Database/README_DATABASE.md
```

## 5. Chạy project

Chạy bằng Visual Studio:

1. Mở `DACS_Food.csproj`.
2. Chọn profile chạy web.
3. Bấm Run.

Hoặc chạy bằng terminal:

```bash
dotnet restore
dotnet build
dotnet run
```

Sau đó mở URL được terminal hoặc Visual Studio hiển thị.

## 6. Tài khoản admin demo

Seeder sẽ tạo tài khoản admin demo:

```text
Email: admin@foodielab.local
Mật khẩu: Admin@123
```

Sau khi đăng nhập bằng tài khoản admin, vào khu quản trị:

```text
/admin
```

## 7. Các route chính

- Trang chủ: `/`
- Món ăn: `/menu`
- Công thức: `/recipes`
- Chọn thực phẩm: `/food-guide`
- Đặt bàn & Không gian: `/ban`
- Tra cứu đơn hàng: `/tra-cuu-don-hang`
- Checkout: `/checkout`
- Admin: `/admin`
- Admin quản lý đơn hàng: `/admin/don-hang`
- Admin quản lý bàn/đặt bàn: `/admin/ban`
- Admin mã giảm giá: `/admin/ma-giam-gia`

## 8. Test nhanh nghiệp vụ

### Đặt món online

1. Vào `/menu`.
2. Thêm món vào giỏ hàng.
3. Vào `/checkout`.
4. Nhập họ tên, số điện thoại, địa chỉ.
5. Tạo đơn hàng.
6. Ghi lại mã đơn hàng.

### Quản lý đơn hàng admin

1. Đăng nhập admin.
2. Vào `/admin/don-hang`.
3. Cập nhật trạng thái đơn theo quy trình:
   - Pending
   - Confirmed
   - Preparing
   - ReadyForDelivery
   - Delivering
   - Delivered hoặc Completed

### Xác nhận giao hàng

1. Vào trang xác nhận giao hàng trong khu quản trị.
2. Nhập mã đơn hàng.
3. Nếu đơn đang ở trạng thái Delivering, hệ thống sẽ xác nhận đã giao thành công.

### Tra cứu đơn hàng

1. Vào `/tra-cuu-don-hang`.
2. Nhập mã đơn hàng hoặc số điện thoại.
3. Xem trạng thái đơn hàng và giao hàng.

### Đặt bàn

1. Vào `/ban`.
2. Chọn ngày, bàn, khung giờ.
3. Nhập thông tin khách.
4. Gửi yêu cầu đặt bàn.

Admin có thể quản lý tập trung tại:

```text
/admin/ban
```

## 9. Lưu ý khi giao project

Không cần gửi `bin/` và `obj/` vì đây là thư mục build/cache. Người nhận chạy `dotnet restore`, `dotnet build` là hệ thống tự tạo lại.

Nếu giao project qua ZIP, cần giữ:

- Source code
- `wwwroot/`
- `Database/`
- `appsettings.json`
- `appsettings.Development.json`
- `appsettings.example.json`
- `DACS_Food.csproj`
- `DACS_Food.csproj.user`
- File hướng dẫn này

## 10. Khi giao diện chưa cập nhật

Nếu trình duyệt vẫn hiển thị CSS/JS cũ, bấm:

```text
Ctrl + F5
```

Nếu database vừa reset hoặc đổi connection string, hãy tắt app và chạy lại.
