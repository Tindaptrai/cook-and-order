# Cook & Order / FoodieLab

Website đặt món ăn trực tuyến kết hợp chia sẻ công thức nấu ăn. Project hỗ trợ giỏ hàng, đặt hàng, thanh toán VietQR, gửi email SMTP và chatbot AI tư vấn món ăn.

## Công nghệ

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Razor View
- HTML/CSS/JavaScript
- MailKit SMTP
- VietQR Quick Link
- Gemini AI API

## Chức năng chính

- Đăng ký, đăng nhập
- Xem danh sách món ăn
- Xem chi tiết món
- Chọn số lượng và thêm vào giỏ hàng
- Đặt hàng
- Thanh toán tiền mặt hoặc VietQR
- Gửi email xác nhận đơn hàng
- Xem công thức nấu ăn
- Chatbot AI tư vấn món ăn
- Quên mật khẩu
- Quản trị món ăn, đơn hàng, bàn đặt và mã giảm giá

## Cấu hình

Không commit API key, SMTP password, connection string production hoặc thông tin ngân hàng cá nhân thật.

1. Copy file mẫu:

```bash
copy DACS_Food\appsettings.example.json DACS_Food\appsettings.Development.json
```

2. Điền các cấu hình cần thiết trong `DACS_Food/appsettings.Development.json`:

- `ConnectionStrings:DefaultConnection`
- `Smtp:UserName`
- `Smtp:Password`
- `Smtp:FromEmail`
- `GeminiSettings:ApiKey` nếu dùng chatbot AI
- `QrPayment:BankAccountNumber` và `QrPayment:BankAccountName` nếu dùng VietQR thật

## Dùng user-secrets khi phát triển

Có thể lưu secret ngoài source code bằng lệnh:

```bash
cd DACS_Food
dotnet user-secrets init
dotnet user-secrets set "Smtp:UserName" "your-email@gmail.com"
dotnet user-secrets set "Smtp:Password" "your-gmail-app-password"
dotnet user-secrets set "Smtp:FromEmail" "your-email@gmail.com"
dotnet user-secrets set "GeminiSettings:ApiKey" "your-gemini-api-key"
dotnet user-secrets set "QrPayment:BankAccountNumber" "your-bank-account-number"
dotnet user-secrets set "QrPayment:BankAccountName" "your-bank-account-name"
```

Production nên dùng biến môi trường:

```text
Smtp__UserName
Smtp__Password
Smtp__FromEmail
GeminiSettings__ApiKey
QrPayment__BankAccountNumber
QrPayment__BankAccountName
```

## Chạy project

```bash
cd DACS_Food
dotnet restore
dotnet ef database update
dotnet run
```

Nếu không dùng migration, có thể tạo database bằng script trong `DACS_Food/Database` rồi chạy app.

## GitHub

Tạo repository rỗng trên GitHub trước, ví dụ:

```text
https://github.com/Tindaptrai/cook-and-order.git
```

Sau khi kiểm tra sạch secret:

```bash
git init
git add .
git status
git commit -m "Initial commit: FoodieLab Cook & Order"
git branch -M main
git remote add origin https://github.com/Tindaptrai/cook-and-order.git
git push -u origin main
```
