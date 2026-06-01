using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers.Api
{
    [ApiController]
    [Route("api/food-guide")]
    public class FoodGuideApiController : ControllerBase
    {
        private static readonly FoodGuideItem[] Items =
        {
            Item(1, "Rau cải xanh", "Mustard Greens", "rau-cu", "Rau củ", "/assets/images/food-guide/rau-cu/rau-cai-xanh.jpg", "Rau lá xanh thường dùng để luộc, xào hoặc nấu canh."),
            Item(2, "Rau muống", "Water Spinach", "rau-cu", "Rau củ", "/assets/images/food-guide/rau-cu/rau-muong.jpg", "Rau phổ biến để luộc, xào tỏi hoặc nấu canh chua."),
            Item(3, "Cà chua", "Tomato", "rau-cu", "Rau củ", "/assets/images/food-guide/rau-cu/ca-chua.jpg", "Loại quả phổ biến trong món canh, sốt, salad và món xào."),
            Item(4, "Cà rốt", "Carrot", "rau-cu", "Rau củ", "/assets/images/food-guide/rau-cu/ca-rot.jpg", "Củ giòn ngọt, dùng cho món canh, xào, salad hoặc nước ép."),
            Item(5, "Khoai tây", "Potato", "rau-cu", "Rau củ", "/assets/images/food-guide/rau-cu/khoai-tay.jpg", "Củ giàu tinh bột, dùng để hầm, chiên, nghiền hoặc nấu soup."),
            Item(6, "Bí đỏ", "Pumpkin", "rau-cu", "Rau củ", "/assets/images/food-guide/rau-cu/bi-do.jpg", "Bí vị ngọt nhẹ, dùng cho canh, soup, cháo hoặc món hấp."),
            Item(7, "Cam", "Orange", "trai-cay", "Trái cây", "/assets/images/food-guide/trai-cay/cam.jpg", "Trái cây mọng nước, dùng ăn trực tiếp hoặc ép nước."),
            Item(8, "Táo", "Apple", "trai-cay", "Trái cây", "/assets/images/food-guide/trai-cay/tao.jpg", "Trái cây giòn ngọt, dễ mang theo và dùng làm món ăn nhẹ."),
            Item(9, "Chuối", "Banana", "trai-cay", "Trái cây", "/assets/images/food-guide/trai-cay/chuoi.jpg", "Trái cây mềm, ngọt, phù hợp bữa phụ hoặc làm sinh tố."),
            Item(10, "Bơ", "Avocado", "trai-cay", "Trái cây", "/assets/images/food-guide/trai-cay/bo.jpg", "Quả béo nhẹ, thường dùng làm sinh tố, salad hoặc món healthy."),
            Item(11, "Dưa hấu", "Watermelon", "trai-cay", "Trái cây", "/assets/images/food-guide/trai-cay/dua-hau.jpg", "Trái cây nhiều nước, hợp dùng giải khát."),
            Item(12, "Xoài", "Mango", "trai-cay", "Trái cây", "/assets/images/food-guide/trai-cay/xoai.jpg", "Trái cây thơm ngọt, ăn trực tiếp hoặc làm sinh tố."),
            Item(13, "Thịt heo", "Pork", "thit", "Thịt", "/assets/images/food-guide/thit/thit-heo.jpg", "Nguồn đạm quen thuộc cho món kho, xào, luộc và nấu canh."),
            Item(14, "Thịt bò", "Beef", "thit", "Thịt", "/assets/images/food-guide/thit/thit-bo.jpg", "Thịt đỏ dùng cho món xào, phở, bún hoặc áp chảo."),
            Item(15, "Thịt gà", "Chicken", "thit", "Thịt", "/assets/images/food-guide/thit/thit-ga.jpg", "Nguồn đạm dễ chế biến cho món luộc, kho, chiên hoặc salad."),
            Item(16, "Sườn heo", "Pork Ribs", "thit", "Thịt", "/assets/images/food-guide/thit/suon-heo.jpg", "Nguyên liệu hợp cho món nướng, rim, kho hoặc nấu canh."),
            Item(17, "Cá hồi", "Salmon", "ca-hai-san", "Cá & hải sản", "/assets/images/food-guide/ca-hai-san/ca-hoi.jpg", "Cá béo, thường dùng áp chảo, nướng hoặc làm salad."),
            Item(18, "Cá basa", "Basa Fish", "ca-hai-san", "Cá & hải sản", "/assets/images/food-guide/ca-hai-san/ca-basa.jpg", "Cá thịt mềm, dùng kho, chiên hoặc nấu canh chua."),
            Item(19, "Tôm", "Shrimp", "ca-hai-san", "Cá & hải sản", "/assets/images/food-guide/ca-hai-san/tom.jpg", "Hải sản phổ biến cho món hấp, xào, canh và gỏi."),
            Item(20, "Mực", "Squid", "ca-hai-san", "Cá & hải sản", "/assets/images/food-guide/ca-hai-san/muc.jpg", "Hải sản có độ giòn, thường dùng xào, hấp hoặc nướng."),
            Item(21, "Trứng gà", "Chicken Egg", "trung-sua", "Trứng & sữa", "/assets/images/food-guide/trung-sua/trung-ga.jpg", "Nguyên liệu nhanh gọn cho món chiên, luộc, canh hoặc làm bánh."),
            Item(22, "Sữa tươi", "Fresh Milk", "trung-sua", "Trứng & sữa", "/assets/images/food-guide/trung-sua/sua-tuoi.jpg", "Thực phẩm dùng uống trực tiếp, pha chế hoặc nấu món nhẹ."),
            Item(23, "Đậu hũ", "Tofu", "dau-hat-ngu-coc", "Đậu, hạt & ngũ cốc", "/assets/images/food-guide/dau-hat-ngu-coc/dau-hu.jpg", "Thực phẩm từ đậu nành, dùng cho món chay, món sốt và món canh."),
            Item(24, "Đậu nành", "Soybean", "dau-hat-ngu-coc", "Đậu, hạt & ngũ cốc", "/assets/images/food-guide/dau-hat-ngu-coc/dau-nanh.jpg", "Hạt dùng nấu sữa, làm đậu hũ hoặc chế biến món chay."),
            Item(25, "Gạo lứt", "Brown Rice", "dau-hat-ngu-coc", "Đậu, hạt & ngũ cốc", "/assets/images/food-guide/dau-hat-ngu-coc/gao-lut.jpg", "Ngũ cốc nguyên cám, thường dùng cho bữa ăn healthy."),
            Item(26, "Yến mạch", "Oats", "dau-hat-ngu-coc", "Đậu, hạt & ngũ cốc", "/assets/images/food-guide/dau-hat-ngu-coc/yen-mach.jpg", "Ngũ cốc dùng cho bữa sáng, cháo hoặc món ăn nhẹ."),
            Item(27, "Hành tím", "Shallot", "gia-vi-thuc-pham-kho", "Gia vị & thực phẩm khô", "/assets/images/food-guide/gia-vi-thuc-pham-kho/hanh-tim.jpg", "Gia vị tạo mùi thơm cho món kho, xào, canh và nước chấm."),
            Item(28, "Tỏi", "Garlic", "gia-vi-thuc-pham-kho", "Gia vị & thực phẩm khô", "/assets/images/food-guide/gia-vi-thuc-pham-kho/toi.jpg", "Gia vị thơm dùng trong nhiều món xào, nướng và nước sốt."),
            Item(29, "Gừng", "Ginger", "gia-vi-thuc-pham-kho", "Gia vị & thực phẩm khô", "/assets/images/food-guide/gia-vi-thuc-pham-kho/gung.jpg", "Gia vị cay ấm, hợp món kho, hấp, trà và khử mùi thực phẩm."),
            Item(30, "Nấm khô", "Dried Mushroom", "gia-vi-thuc-pham-kho", "Gia vị & thực phẩm khô", "/assets/images/food-guide/gia-vi-thuc-pham-kho/nam-kho.jpg", "Thực phẩm khô tạo mùi thơm cho món canh, xào và món chay.")
        };

        [HttpGet]
        public IActionResult GetItems([FromQuery] string? category, [FromQuery] string? keyword)
        {
            var query = Items.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(category) && category != "all")
            {
                query = query.Where(x => x.Category == category);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var term = keyword.Trim();
                query = query.Where(x =>
                    x.NameVi.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.NameEn.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            var items = query.OrderBy(x => x.Id).ToArray();
            return Ok(new { totalItems = items.Length, items });
        }

        [HttpGet("{id:int}")]
        public IActionResult GetItem(int id)
        {
            var item = Items.FirstOrDefault(x => x.Id == id);
            return item == null ? NotFound() : Ok(item);
        }

        private static FoodGuideItem Item(int id, string nameVi, string nameEn, string category, string categoryLabel, string image, string shortDesc)
        {
            var chooseTips = category switch
            {
                "thit" => new[] { "Chọn màu tự nhiên, bề mặt khô ráo, không nhớt.", "Ấn nhẹ thấy có độ đàn hồi.", "Ưu tiên nơi bán có bảo quản lạnh và nguồn gốc rõ ràng." },
                "ca-hai-san" => new[] { "Chọn thực phẩm có mùi biển nhẹ, không hôi tanh gắt.", "Thịt chắc, không mềm nhũn hoặc chảy dịch lạ.", "Ưu tiên mua khi còn lạnh và được che đậy sạch." },
                "trung-sua" => new[] { "Kiểm tra bao bì hoặc vỏ còn nguyên, sạch.", "Chọn sản phẩm còn hạn dùng rõ ràng.", "Không mua nếu có mùi lạ hoặc dấu hiệu rò rỉ." },
                "dau-hat-ngu-coc" => new[] { "Chọn hạt hoặc sản phẩm khô ráo, không mốc.", "Bao bì còn kín, thông tin rõ ràng.", "Mùi tự nhiên, không hôi dầu hoặc chua." },
                "gia-vi-thuc-pham-kho" => new[] { "Chọn củ/hàng khô chắc, khô ráo.", "Không có mốc, mọt hoặc mùi lạ.", "Ưu tiên bao bì sạch, tránh hàng ẩm." },
                "trai-cay" => new[] { "Chọn quả cầm chắc tay, màu tự nhiên.", "Vỏ không dập nát, không chảy nước.", "Mùi thơm nhẹ, không lên men." },
                _ => new[] { "Chọn màu tự nhiên, tươi, không héo úa.", "Cầm chắc tay, không nhớt hoặc dập nát.", "Ưu tiên rau củ có nguồn gốc rõ ràng." }
            };

            var avoidSigns = category switch
            {
                "thit" => new[] { "Tránh màu tái xanh, xám hoặc thâm đen.", "Không chọn thực phẩm nhớt, chảy dịch.", "Tránh mùi hôi, chua hoặc bất thường." },
                "ca-hai-san" => new[] { "Tránh mắt đục, thịt mềm nhũn, mang thâm nếu là cá nguyên con.", "Không chọn hải sản có mùi khai hoặc hôi nặng.", "Tránh sản phẩm rã đông nhiều lần." },
                _ => new[] { "Tránh thực phẩm mốc, dập nát hoặc biến màu.", "Không dùng nếu có mùi chua, hôi hoặc lên men lạ.", "Tránh sản phẩm chảy nước, nhớt hoặc bao bì phồng." }
            };

            var storage = category switch
            {
                "thit" or "ca-hai-san" => new[] { "Để ngăn mát nếu dùng trong ngày.", "Đông lạnh nếu chưa dùng ngay.", "Không để thực phẩm sống tiếp xúc rau củ ăn liền." },
                "trung-sua" => new[] { "Bảo quản theo hướng dẫn trên bao bì.", "Sữa đã mở nên dùng sớm và giữ lạnh.", "Trứng nên để nơi mát hoặc ngăn mát tủ lạnh." },
                _ => new[] { "Bảo quản nơi khô ráo, thoáng mát hoặc ngăn mát tùy loại.", "Không để chung với thực phẩm sống chưa đóng gói.", "Dùng sớm sau khi cắt hoặc mở bao bì." }
            };

            return new FoodGuideItem(
                id,
                nameVi,
                nameEn,
                category,
                categoryLabel,
                image,
                shortDesc,
                chooseTips,
                avoidSigns,
                storage,
                "Rửa tay và làm sạch nguyên liệu trước khi chế biến. Không sử dụng thực phẩm có mùi lạ, mốc, dập nát nặng hoặc chảy dịch bất thường.");
        }

        private sealed record FoodGuideItem(
            int Id,
            string NameVi,
            string NameEn,
            string Category,
            string CategoryLabel,
            string Image,
            string ShortDesc,
            string[] ChooseTips,
            string[] AvoidSigns,
            string[] Storage,
            string SafetyNote);
    }
}
