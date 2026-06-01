using DACS_Food.Models;
using DACS_Food.ViewModels;

namespace DACS_Food.Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly string _messengerUrl;
        private static readonly IReadOnlyList<Recipe> Recipes = new List<Recipe>
        {
            new()
            {
                Id = 1,
                Title = "Trứng chiên hành",
                Slug = "trung-chien-hanh",
                Description = "Món nhanh, thơm hành, hợp khi bạn cần bữa nhẹ dễ nấu.",
                Ingredients = "Trứng gà; Hành lá; Dầu ăn; Nước mắm; Tiêu",
                Steps = "Đánh trứng với nước mắm và tiêu.; Cắt nhỏ hành lá rồi trộn vào trứng.; Làm nóng chảo, cho dầu ăn.; Đổ trứng vào chiên lửa vừa đến khi vàng hai mặt.",
                ImageUrl = "https://images.unsplash.com/photo-1525351484163-7529414344d8?auto=format&fit=crop&w=900&q=80",
                Difficulty = "Dễ",
                CookingTimeMinutes = 10
            },
            new()
            {
                Id = 2,
                Title = "Cơm rang trứng",
                Slug = "com-rang-trung",
                Description = "Món no bụng, tận dụng cơm nguội rất tiện cho bữa nhanh.",
                Ingredients = "Cơm nguội; Trứng gà; Hành lá; Cà rốt; Dầu ăn; Nước mắm; Tiêu",
                Steps = "Đánh tan trứng, cắt nhỏ hành lá và cà rốt.; Phi thơm hành, cho cà rốt vào đảo nhanh.; Cho cơm nguội vào rang tơi.; Thêm trứng, nêm nước mắm và tiêu.; Đảo đều đến khi hạt cơm săn lại.",
                ImageUrl = "https://images.unsplash.com/photo-1603133872878-684f208fb84b?auto=format&fit=crop&w=900&q=80",
                Difficulty = "Dễ",
                CookingTimeMinutes = 15
            },
            new()
            {
                Id = 3,
                Title = "Canh nấm chay",
                Slug = "canh-nam-chay",
                Description = "Món chay thanh nhẹ, hợp ngày rằm hoặc khi muốn ăn nhẹ bụng.",
                Ingredients = "Nấm rơm; Nấm kim châm; Đậu hũ non; Cà rốt; Hành lá; Muối; Tiêu",
                Steps = "Rửa sạch nấm và cắt miếng vừa ăn.; Đun nước, cho cà rốt vào trước.; Thêm nấm và đậu hũ non.; Nêm muối, tiêu vừa ăn.; Tắt bếp, thêm hành lá.",
                ImageUrl = "https://images.unsplash.com/photo-1547592166-23ac45744acd?auto=format&fit=crop&w=900&q=80",
                Difficulty = "Dễ",
                CookingTimeMinutes = 20
            },
            new()
            {
                Id = 4,
                Title = "Salad ức gà",
                Slug = "salad-uc-ga",
                Description = "Món healthy nhẹ bụng, nhiều rau và đủ protein.",
                Ingredients = "Ức gà; Xà lách; Cà chua bi; Dưa leo; Bắp ngọt; Sốt mè rang; Tiêu",
                Steps = "Áp chảo ức gà với ít tiêu.; Rửa sạch rau củ và để ráo.; Cắt ức gà thành lát mỏng.; Trộn rau, gà và sốt mè rang.; Dùng ngay khi rau còn tươi.",
                ImageUrl = "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=900&q=80",
                Difficulty = "Dễ",
                CookingTimeMinutes = 18
            },
            new()
            {
                Id = 5,
                Title = "Mì Ý sốt cà",
                Slug = "mi-y-sot-ca",
                Description = "Món nóng dễ ăn, sốt cà chua chua ngọt tự nhiên.",
                Ingredients = "Mì Ý; Cà chua; Thịt bằm; Hành tây; Tỏi; Dầu olive; Muối; Tiêu",
                Steps = "Luộc mì vừa chín tới.; Phi thơm tỏi và hành tây.; Cho thịt bằm vào đảo chín.; Thêm cà chua băm, nêm muối và tiêu.; Trộn mì với sốt rồi dùng nóng.",
                ImageUrl = "https://images.unsplash.com/photo-1551183053-bf91a1d81141?auto=format&fit=crop&w=900&q=80",
                Difficulty = "Vừa",
                CookingTimeMinutes = 25
            }
        };

        public ChatbotService(IConfiguration configuration)
        {
            _messengerUrl = configuration["SupportSettings:MessengerUrl"]
                ?? configuration["MessengerUrl"]
                ?? string.Empty;
        }

        public ChatbotAskResponse Ask(string message)
        {
            var text = Normalize(message);

            if (string.IsNullOrWhiteSpace(text))
            {
                return BuildResponse("Dạ bạn muốn FoodieBot gợi ý món theo tâm trạng, nguyên liệu hay nhu cầu ăn uống nè?", Array.Empty<string>());
            }

            if (ContainsAny(text, "tu van truc tiep", "gap nhan vien", "noi chuyen voi nguoi that", "can ho tro", "ho tro truc tiep"))
            {
                return BuildResponse(
                    "Dạ được nha! FoodieBot sẽ chuyển bạn sang nhân viên tư vấn của FoodieTTTM ngay.",
                    Array.Empty<string>(),
                    needHumanSupport: true);
            }

            if (ContainsAny(text, "di cho", "mua gi", "nguyen lieu", "nau"))
            {
                var recipe = FindRecipe(text);
                if (recipe != null)
                {
                    return BuildResponse(
                        $"Dạ được nha! Để nấu {recipe.Title}, bạn chuẩn bị danh sách này là ổn đó.",
                        new[] { recipe.Title },
                        new[] { ToRecipeLink(recipe) },
                        ToShoppingList(recipe));
                }
            }

            if (ContainsAny(text, "trung"))
            {
                return BuildResponse(
                    "Dạ có trứng thì dễ biến tấu lắm nha. FoodieBot gợi ý trứng chiên hành hoặc cơm rang trứng, nhanh mà vẫn ngon.",
                    new[] { "Trứng chiên hành", "Cơm rang trứng" },
                    RecipeLinks("trung-chien-hanh", "com-rang-trung"));
            }

            if (ContainsAny(text, "com", "doi", "no bung"))
            {
                return BuildResponse(
                    "Dạ nếu bạn đang đói thì cơm rang trứng hợp với bạn đó. Món này no bụng, dễ ăn và làm khá nhanh.",
                    new[] { "Cơm rang trứng", "Cơm gà sốt tiêu đen" },
                    RecipeLinks("com-rang-trung"));
            }

            if (ContainsAny(text, "rau", "nam", "chay", "ram"))
            {
                return BuildResponse(
                    "Dạ nếu bạn muốn nhẹ bụng hơn, mình gợi ý món chay nha. Canh nấm chay rất hợp ngày rằm và những hôm muốn ăn thanh đạm.",
                    new[] { "Canh nấm chay", "Cơm chay nấm" },
                    RecipeLinks("canh-nam-chay"));
            }

            if (ContainsAny(text, "healthy", "giam can", "it beo", "an nhe", "nhe bung"))
            {
                return BuildResponse(
                    "Dạ được nha! Nếu muốn healthy hoặc nhẹ bụng, salad ức gà là lựa chọn hợp lý vì có rau xanh và protein.",
                    new[] { "Salad ức gà", "Poke bowl cá hồi" },
                    RecipeLinks("salad-uc-ga"));
            }

            if (ContainsAny(text, "met", "buon", "nong", "am bung"))
            {
                return BuildResponse(
                    "Dạ hôm nay hơi mệt thì mình gợi ý món nóng, dễ ăn nha. Mì Ý sốt cà hoặc cơm gà sốt tiêu đen sẽ hợp với bạn đó.",
                    new[] { "Mì Ý sốt cà", "Cơm gà sốt tiêu đen" },
                    RecipeLinks("mi-y-sot-ca"));
            }

            if (ContainsAny(text, "cay"))
            {
                return BuildResponse(
                    "Dạ thích cay thì bạn thử mì bò cay hoặc bánh gạo cay nha. Nếu muốn tự nấu món nóng dễ ăn, mình gửi thêm công thức mì Ý sốt cà để đổi vị.",
                    new[] { "Mì bò cay", "Bánh gạo cay", "Mì Ý sốt cà" },
                    RecipeLinks("mi-y-sot-ca"));
            }

            if (ContainsAny(text, "ga", "thit ga"))
            {
                return BuildResponse(
                    "Dạ có thịt gà thì FoodieBot gợi ý salad ức gà nếu muốn healthy, hoặc cơm gà sốt tiêu đen nếu muốn no hơn.",
                    new[] { "Salad ức gà", "Cơm gà sốt tiêu đen" },
                    RecipeLinks("salad-uc-ga"));
            }

            if (ContainsAny(text, "gio vang", "khuyen mai", "uu dai"))
            {
                return BuildResponse(
                    "Dạ giờ vàng thì bạn nên chọn các món best seller để dễ áp dụng ưu đãi nha. FoodieBot gợi ý cơm rang trứng, salad ức gà hoặc canh nấm chay tùy khẩu vị.",
                    new[] { "Cơm rang trứng", "Salad ức gà", "Canh nấm chay" },
                    RecipeLinks("com-rang-trung", "salad-uc-ga", "canh-nam-chay"));
            }

            return BuildResponse(
                "Dạ FoodieBot gợi ý bạn thử cơm rang trứng nếu muốn no nhanh, canh nấm chay nếu muốn nhẹ bụng, hoặc salad ức gà nếu muốn healthy nha.",
                new[] { "Cơm rang trứng", "Canh nấm chay", "Salad ức gà" },
                RecipeLinks("com-rang-trung", "canh-nam-chay", "salad-uc-ga"));
        }

        public IReadOnlyList<Recipe> GetRecipes()
        {
            return Recipes;
        }

        public Recipe? GetRecipeBySlug(string slug)
        {
            return Recipes.FirstOrDefault(x => string.Equals(x.Slug, slug, StringComparison.OrdinalIgnoreCase));
        }

        private ChatbotAskResponse BuildResponse(
            string message,
            IReadOnlyList<string> suggestedFoods,
            IReadOnlyList<RecipeLinkViewModel>? recipeLinks = null,
            IReadOnlyList<ShoppingListItemViewModel>? shoppingList = null,
            bool needHumanSupport = false)
        {
            return new ChatbotAskResponse
            {
                Message = message,
                SuggestedFoods = suggestedFoods,
                RecipeLinks = recipeLinks ?? Array.Empty<RecipeLinkViewModel>(),
                ShoppingList = shoppingList ?? Array.Empty<ShoppingListItemViewModel>(),
                MessengerUrl = _messengerUrl,
                NeedHumanSupport = needHumanSupport
            };
        }

        private static Recipe? FindRecipe(string normalizedText)
        {
            return Recipes.FirstOrDefault(x => normalizedText.Contains(Normalize(x.Title)) || normalizedText.Contains(Normalize(x.Slug)));
        }

        private static IReadOnlyList<RecipeLinkViewModel> RecipeLinks(params string[] slugs)
        {
            return Recipes
                .Where(x => slugs.Contains(x.Slug))
                .Select(ToRecipeLink)
                .ToList();
        }

        private static RecipeLinkViewModel ToRecipeLink(Recipe recipe)
        {
            return new RecipeLinkViewModel
            {
                Title = recipe.Title,
                Url = $"/Recipe/Detail/{recipe.Slug}"
            };
        }

        private static IReadOnlyList<ShoppingListItemViewModel> ToShoppingList(Recipe recipe)
        {
            return recipe.Ingredients
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => new ShoppingListItemViewModel { Name = x })
                .ToList();
        }

        private static bool ContainsAny(string source, params string[] keywords)
        {
            return keywords.Any(source.Contains);
        }

        private static string Normalize(string value)
        {
            var lower = value.Trim().ToLowerInvariant();
            var chars = lower.Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                .ToArray();

            return new string(chars)
                .Normalize(System.Text.NormalizationForm.FormC)
                .Replace('đ', 'd');
        }
    }
}

