# DACS_Food MVC Overlay

Gói này chuyển giao diện FoodieLab từ HTML tĩnh sang đúng cấu trúc ASP.NET Core MVC.

## Cách dùng trong Visual Studio

1. Giải nén file ZIP này.
2. Mở thư mục `DACS_Food_MVC_Overlay`.
3. Copy toàn bộ các thư mục/file bên trong vào thư mục gốc project `DACS_Food`.
4. Khi Visual Studio hỏi Replace file, chọn Replace cho các file trùng.
5. Chạy project bằng Ctrl + F5.

## Cấu trúc chính

DACS_Food/
├── Controllers/HomeController.cs
├── Models/FoodItem.cs
├── Views/Home/*.cshtml
├── Views/Shared/_Layout.cshtml
├── wwwroot/assets/css/style.css
├── wwwroot/assets/js/main.js
└── AGENTS.md

Không cần copy các file HTML tĩnh vào wwwroot nữa. Bản này dùng Razor View `.cshtml`.
