# README - Chuc nang kho hang/nguyen lieu

## Muc dich

Chuc nang nay duoc them de admin quan ly tinh trang kho cua tung mon an:

- Biet mon con hang hay het hang.
- Ghi nguyen lieu chinh cua mon.
- Nhap so luong con lai va don vi tinh.
- Ghi chu kho/nguyen lieu.
- Chon thang co ban cho cac mon theo mua.
- Khi mon het hang hoac khong dung thang dang ban, menu se hien dung trang thai va khong cho them vao gio hang.
- Gioi han so luong dat mon tren menu theo so luong con trong kho.
- Gio hang va server cung chan so luong vuot qua ton kho.

## Duong dan su dung

Trang admin:

```text
/admin/kho-nguyen-lieu
```

Trang nay chi danh cho tai khoan co role `Admin`.

## Cach hoat dong

Trang kho hang doc danh sach mon an tu bang `FoodItems`.

Thong tin co san trong database:

- `FoodItem.Ingredients`: nguyen lieu chinh.
- `FoodItem.IsAvailable`: con ban/hit hang tren menu.
- `FoodItem.IsActive`: mon co dang hien thi trong he thong hay khong.

Thong tin kho them moi duoc luu bang file JSON rieng:

```text
Data/inventory-metadata.json
```

File nay luu theo `FoodItemId`, gom:

- `StockQuantity`: so luong con.
- `Unit`: don vi tinh.
- `AvailableMonths`: cac thang co ban.
- `InventoryNote`: ghi chu kho.

Neu `StockQuantity <= 0`, he thong tu dat mon thanh het hang.

Neu co chon `AvailableMonths` ma thang hien tai khong nam trong danh sach do, he thong cung tu dat mon thanh het hang.

Nguoc lai, neu so luong con lon hon 0 va thang hien tai nam trong cac thang co ban, he thong tu bat lai trang thai con ban cho mon.

## File da them

### `Areas/Admin/Controllers/InventoryController.cs`

Controller xu ly trang kho hang.

Code chinh:

- Route GET:

```csharp
[HttpGet("/admin/kho-nguyen-lieu")]
public async Task<IActionResult> Index()
```

- Route POST cap nhat kho:

```csharp
[HttpPost("/admin/kho-nguyen-lieu/cap-nhat/{id:int}")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Update(int id, UpdateInventoryViewModel model)
```

- Logic tu tinh trang thai con ban:

```csharp
var hasStock = !model.StockQuantity.HasValue || model.StockQuantity.Value > 0;
var isInCurrentSeason = months.Length == 0 || months.Contains(DateTime.Now.Month);
var canSell = hasStock && isInCurrentSeason;
```

- Doc/ghi file JSON:

```csharp
private async Task<Dictionary<string, InventoryMetadata>> LoadMetadataAsync()
private async Task SaveMetadataAsync(Dictionary<string, InventoryMetadata> metadata)
```

- Chuan hoa thang hop le tu 1 den 12:

```csharp
private static int[] NormalizeMonths(IEnumerable<int>? months)
```

### `Areas/Admin/Views/Inventory/Index.cshtml`

Giao dien quan ly kho hang.

Code chinh:

- Hien thi tong quan:

```cshtml
@Model.Items.Count
@Model.Items.Count(x => x.IsActive && x.IsAvailable)
@Model.Items.Count(x => x.IsActive && !x.IsAvailable)
```

- Form cap nhat tung mon:

```cshtml
<form class="inventory-form" method="post" action="/admin/kho-nguyen-lieu/cap-nhat/@item.FoodItemId">
```

- Thanh tim kiem trong kho:

```cshtml
<input id="inventorySearchInput" type="search" />
```

- Danh sach thang co ban:

```cshtml
@for (var month = 1; month <= 12; month++)
{
    <label>
        <input type="checkbox" name="AvailableMonths" value="@month" />
        Tháng @(month)
    </label>
}
```

### `ViewModels/InventoryViewModels.cs`

ViewModel cho trang kho hang.

Code chinh:

```csharp
public class InventoryPageViewModel
public class InventoryItemViewModel
public class UpdateInventoryViewModel
```

## File da sua

### `Areas/Admin/Views/Shared/_Layout.cshtml`

Them link tren thanh dieu huong admin:

```html
<a href="/admin/kho-nguyen-lieu">Kho/nguyên liệu</a>
```

### `wwwroot/assets/css/style.css`

Them CSS cho trang kho hang va chinh lai header admin.

Class moi lien quan kho hang:

```css
.inventory-summary
.inventory-list
.inventory-search-panel
.inventory-search-box
.inventory-empty-state
.inventory-card
.inventory-card-head
.inventory-meta
.inventory-season-line
.inventory-form
.inventory-form-grid
.inventory-months
.inventory-actions
```

Responsive da them cho man hinh nho:

```css
@media (max-width: 1080px)
@media (max-width: 640px)
```

Chinh header admin:

```css
.admin-header .navbar
.admin-header .logo-text h2
.admin-header .logo-text span
.admin-header .nav-links
.admin-logout-btn
```

### `Services/FoodService.cs`

Sua API menu de tra ve ca mon dang active nhung tam het hang.

Truoc do chi lay mon con hang:

```csharp
.Where(x => x.IsActive && x.IsAvailable)
```

Sau khi sua:

```csharp
.Where(x => x.IsActive)
```

Ly do: frontend can nhan duoc `IsAvailable = false` de hien dung trang thai `Het mon` va khoa nut them vao gio hang.

### `wwwroot/assets/js/menu.js`

Sua logic tron du lieu menu API voi danh sach mon tinh.

Code chinh:

```javascript
const missingDedicatedItems = DEDICATED_MENU_ITEMS.filter(item =>
  item.slug
    ? !apiSlugs.has(item.slug) && !apiNames.has(normalizeMenuImageKey(item.name))
    : !apiNames.has(normalizeMenuImageKey(item.name)));
```

Ly do: tranh viec mon da het hang trong database bi danh sach mon tinh "hoi sinh" thanh con hang.

Ngoai ra menu card da co san logic:

```javascript
item.isAvailable === false ? 'Hết món' : 'Còn món'
```

Va nut them gio hang se bi khoa khi het mon:

```javascript
${item.isAvailable === false ? 'disabled' : ''}
```

Menu gioi han so luong dat theo ton kho:

```javascript
function getMenuMaxQuantity(item) {
  const stockQuantity = Number.parseInt(item?.stockQuantity, 10);
  if (!Number.isNaN(stockQuantity) && stockQuantity > 0) {
    return Math.min(stockQuantity, MENU_MAX_QUANTITY);
  }

  return MENU_MAX_QUANTITY;
}
```

### `Services/CartService.cs`

Gio hang tren server doc `Data/inventory-metadata.json` de gioi han so luong theo ton kho.

Code chinh:

```csharp
private static int GetMaxQuantity(int foodItemId, IReadOnlyDictionary<int, int?> stockQuantities)
```

Khi them/cap nhat gio hang:

```csharp
quantity = Math.Clamp(quantity, 1, maxQuantity);
```

Khi lay gio hang:

```csharp
var changed = NormalizeCartQuantities(cart, stockQuantities);
```

### `Controllers/Api/CartApiController.cs`

API gio hang tra them `StockQuantity` de frontend biet gioi han hien tai:

```csharp
StockQuantity = cart.StockQuantities.TryGetValue(x.FoodItemId, out var stockQuantity) ? stockQuantity : null
```

### `wwwroot/assets/js/main.js`

Gio hang frontend clamp so luong theo `stockQuantity` va khoa nut `+` khi dat gioi han:

```javascript
function getCartItemMaxQuantity(item) {
  const stockQuantity = Number.parseInt(item?.stockQuantity, 10);
  if (!Number.isNaN(stockQuantity) && stockQuantity > 0) {
    return Math.min(stockQuantity, 20);
  }

  return 20;
}
```

### `Services/OrderService.cs`

Khi tao don, service lay gio hang da duoc chuan hoa boi `CartService` de tranh luu don co so luong vuot ton kho.

## File du lieu phat sinh

### `Data/inventory-metadata.json`

File nay duoc tao khi admin luu kho lan dau.

Vi du cau truc:

```json
{
  "1": {
    "StockQuantity": 20,
    "Unit": "phần",
    "AvailableMonths": [1, 2, 3],
    "InventoryNote": "Nguyen lieu nhap moi moi sang"
  }
}
```

## Luu y khi chay

Sau khi sua code C#, can Stop va Run lai project trong Visual Studio de thay doi co hieu luc.

Neu chi refresh trinh duyet ma app cu van dang chay, trang menu co the chua nhan logic moi.

## Kiem tra da chay

Da build kiem tra bang lenh:

```powershell
dotnet build -o .codex-build
```

Ket qua:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```
