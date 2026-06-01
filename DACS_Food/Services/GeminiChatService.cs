using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using DACS_Food.Data;
using DACS_Food.Models;
using DACS_Food.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DACS_Food.Services
{
    public class GeminiChatService : IGeminiChatService
    {
        private const string SystemPrompt = """
Bạn là chatbot tư vấn món ăn cho website FoodieLab.
Bạn nói chuyện như một nhân viên tư vấn món ăn thân thiện, tự nhiên và dễ gần.
Nhiệm vụ chính của bạn là giúp khách chọn món để mua.
Mặc định luôn ưu tiên gợi ý món ăn trong menu/database.
Chỉ gợi ý công thức nấu ăn khi người dùng hỏi rõ về cách nấu, công thức, hướng dẫn làm món, nguyên liệu để tự nấu hoặc chế biến.
Bạn chỉ được trả lời dựa trên dữ liệu trong CONTEXT.
Không được bịa món ăn, giá tiền, nguyên liệu, dị ứng, khuyến mãi hoặc công thức.
Nếu người dùng nói họ dị ứng hoặc không ăn được một nguyên liệu, bạn phải tuyệt đối không gợi ý món có nguyên liệu đó.
Không được hiểu câu "tôi dị ứng gà" là yêu cầu tìm món gà. Đó là yêu cầu loại bỏ món có gà.
Chỉ gợi ý món trong SAFE_FOODS. Không được gợi ý món nằm trong EXCLUDED_FOODS.
Nếu không có món phù hợp, hãy nói rõ hiện chưa có món an toàn phù hợp và gợi ý liên hệ nhân viên.
Luôn ưu tiên món còn hàng.
Nếu khách ăn chay, chỉ gợi ý món chay.
Nếu khách hỏi giá, trả lời đúng theo database.
Câu trả lời nên ngắn gọn, tự nhiên, khoảng 3-6 câu.
Không trả lời theo kiểu quá máy móc. Không dùng quá nhiều emoji.
""";

        private readonly ApplicationDbContext _db;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly ILogger<GeminiChatService> _logger;
        private readonly IWebHostEnvironment _environment;

        public GeminiChatService(
            ApplicationDbContext db,
            HttpClient httpClient,
            IConfiguration configuration,
            IMemoryCache cache,
            ILogger<GeminiChatService> logger,
            IWebHostEnvironment environment)
        {
            _db = db;
            _httpClient = httpClient;
            _configuration = configuration;
            _cache = cache;
            _logger = logger;
            _environment = environment;
        }

        public async Task<ChatbotResponse> AskAsync(ChatbotRequest request, string ipAddress)
        {
            var message = (request.Message ?? string.Empty).Trim();
            if (message.Length > 500) message = message[..500];

            var intent = DetectIntent(message);
            if (string.IsNullOrWhiteSpace(message))
            {
                return new ChatbotResponse
                {
                    Intent = intent,
                    Reply = "Bạn muốn ăn no hay ăn nhẹ hơn một chút? Mình sẽ gợi ý món trong menu FoodieLab cho dễ chọn nhé."
                };
            }

            if (intent == "HumanSupport")
            {
                return new ChatbotResponse
                {
                    Intent = intent,
                    Reply = "Được nha, mình chuyển bạn sang nhân viên FoodieLab để tư vấn trực tiếp. Bấm nút Messenger bên dưới là gặp người hỗ trợ ngay.",
                    NeedHumanSupport = true,
                    MessengerUrl = GetMessengerUrl()
                };
            }

            if (intent == "OrderSupport")
            {
                return new ChatbotResponse
                {
                    Intent = intent,
                    Reply = "Phần đơn hàng, giỏ hàng, thanh toán hoặc giao món thì bạn kiểm tra ở khu Giỏ hàng/Tra cứu đơn hàng nhé. Nếu cần xử lý đơn cụ thể, mình có thể chuyển bạn sang nhân viên."
                };
            }

            if (IsRateLimited(ipAddress, request.SessionId))
            {
                _logger.LogWarning("Gemini chatbot rate limited for IP/session.");
                return new ChatbotResponse
                {
                    Intent = intent,
                    Reply = "Bạn nhắn hơi nhanh rồi nè. Thử lại sau ít phút, mình vẫn ưu tiên gợi ý món trong menu FoodieLab cho bạn.",
                    DebugMessage = DevelopmentDebug("Gemini rate limit")
                };
            }

            FoodSearchResult foodSearch = FoodSearchResult.Empty;
            IReadOnlyList<Recipe> recipes;
            try
            {
                var cleanMessage = RemoveSensitiveInfo(message);
                foodSearch = intent == "RecipeHelp" ? FoodSearchResult.Empty : await FindRelevantFoodsAsync(cleanMessage);
                recipes = intent == "RecipeHelp" ? await FindRelevantRecipesAsync(cleanMessage) : Array.Empty<Recipe>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chatbot database query failed.");
                return new ChatbotResponse
                {
                    Intent = intent,
                    Reply = "Hiện FoodieLab chưa trả lời được ngay, bạn có thể thử lại sau ít phút nhé.",
                    DebugMessage = DevelopmentDebug("Database query failed")
                };
            }

            var foodDtos = foodSearch.SafeFoods.Select(ToFoodDto).ToList();
            var recipeDtos = recipes.Select(ToRecipeDto).ToList();
            if (!foodDtos.Any() && !recipeDtos.Any())
            {
                return new ChatbotResponse
                {
                    Intent = intent,
                    Reply = foodSearch.AllergyTerms.Any()
                        ? "Mình chưa tìm được món đủ an toàn với dị ứng bạn vừa nêu. Bạn nên chọn món có nguyên liệu rõ ràng hoặc liên hệ nhân viên để được xác nhận."
                        : intent == "RecipeHelp"
                            ? "Hiện FoodieLab chưa có công thức phù hợp với câu hỏi này. Bạn thử hỏi tên món rõ hơn một chút nhé."
                            : "Hiện menu chưa có món thật khớp với yêu cầu này. Bạn đổi tiêu chí một chút, mình sẽ gợi ý lại ngay."
                };
            }

            var context = BuildContext(foodDtos, recipeDtos, foodSearch.AllergyTerms, foodSearch.ExcludedFoods);
            var gemini = await AskGeminiAsync(message, context);
            if (!string.IsNullOrWhiteSpace(gemini.Reply))
            {
                return new ChatbotResponse
                {
                    Intent = intent,
                    Reply = gemini.Reply,
                    SuggestedFoods = intent == "FoodRecommendation" ? foodDtos : Array.Empty<SuggestedFoodDto>(),
                    SuggestedRecipes = intent == "RecipeHelp" ? recipeDtos : Array.Empty<SuggestedRecipeDto>(),
                    DebugMessage = DevelopmentDebug(gemini.DebugMessage)
                };
            }

            var fallback = BuildRuleBasedReply(intent, foodDtos, recipeDtos, foodSearch.AllergyTerms, foodSearch.ExcludedFoods);
            fallback.DebugMessage = DevelopmentDebug(gemini.DebugMessage);
            return fallback;
        }

        private async Task<FoodSearchResult> FindRelevantFoodsAsync(string message)
        {
            var normalized = Normalize(message);
            var allergyTerms = ExtractAllergyTerms(normalized);
            var maxPrice = ExtractMaxPrice(normalized);
            var vegetarianOnly = ContainsAny(normalized, "chay", "an chay");
            var noSpicy = ContainsAny(normalized, "khong cay", "khong an cay", "it cay");
            var availableOnly = ContainsAny(normalized, "con mon", "con hang");

            var foods = await _db.FoodItems
                .AsNoTracking()
                .Include(x => x.FoodCategory)
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.IsAvailable)
                .ThenByDescending(x => x.IsFeatured || x.IsBestSeller)
                .Take(120)
                .ToListAsync();

            IEnumerable<FoodItem> query = foods;
            if (availableOnly) query = query.Where(x => x.IsAvailable);
            if (vegetarianOnly) query = query.Where(x => x.IsVegetarian || Normalize(x.Category).Contains("chay") || Normalize(x.MainCategory).Contains("chay"));
            if (noSpicy) query = query.Where(IsNotSpicy);
            if (maxPrice.HasValue) query = query.Where(x => (x.DiscountPrice ?? x.Price) <= maxPrice.Value);

            var excludedFoods = new List<ExcludedFood>();
            if (allergyTerms.Any())
            {
                var safeFoods = new List<FoodItem>();
                foreach (var food in query)
                {
                    var unsafeTerm = FindUnsafeAllergyTerm(food, allergyTerms);
                    if (unsafeTerm == null)
                    {
                        safeFoods.Add(food);
                    }
                    else
                    {
                        excludedFoods.Add(new ExcludedFood(food.Name, unsafeTerm.DisplayName));
                    }
                }

                query = safeFoods;
            }

            var keywords = ExtractKeywords(normalized)
                .Where(keyword => !allergyTerms.Any(term => term.SearchTerms.Contains(keyword)))
                .ToList();
            var broadRecommendation = ContainsAny(normalized,
                "goi y", "hom nay", "an gi", "doi", "no", "no bung", "an no", "an nhe", "nhe bung", "mon ngon", "ban chay")
                || allergyTerms.Any();

            var matched = query
                .Select(x => new { Food = x, Score = ScoreFood(x, keywords, normalized) })
                .Where(x => x.Score > 0 || vegetarianOnly || noSpicy || maxPrice.HasValue || broadRecommendation)
                .OrderByDescending(x => x.Food.IsAvailable)
                .ThenByDescending(x => x.Score)
                .ThenByDescending(x => x.Food.IsFeatured || x.Food.IsBestSeller)
                .Select(x => x.Food)
                .Take(8)
                .ToList();

            if (!matched.Any())
            {
                matched = query
                    .OrderByDescending(x => x.IsAvailable)
                    .ThenByDescending(x => x.IsFeatured || x.IsBestSeller)
                    .Take(5)
                    .ToList();
            }

            return new FoodSearchResult(matched, excludedFoods, allergyTerms);
        }

        private async Task<IReadOnlyList<Recipe>> FindRelevantRecipesAsync(string message)
        {
            var normalized = Normalize(message);
            var recipes = await _db.Recipes
                .AsNoTracking()
                .Include(x => x.FoodItem)
                .Where(x => x.IsActive)
                .OrderBy(x => x.Id)
                .Take(100)
                .ToListAsync();

            var keywords = ExtractKeywords(normalized);
            return recipes
                .Select(x => new { Recipe = x, Score = ScoreRecipe(x, keywords, normalized) })
                .Where(x => x.Score > 0 || keywords.Count == 0)
                .OrderByDescending(x => x.Score)
                .Select(x => x.Recipe)
                .Take(5)
                .ToList();
        }

        private async Task<(string Reply, string? DebugMessage)> AskGeminiAsync(string message, string context)
        {
            var apiKey = _configuration["GeminiSettings:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey.Contains("PUT_", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Gemini API key is missing.");
                return (string.Empty, "Gemini API key missing");
            }

            var model = _configuration["GeminiSettings:Model"] ?? "gemini-2.5-flash";
            var baseUrl = (_configuration["GeminiSettings:BaseUrl"] ?? "https://generativelanguage.googleapis.com/v1beta/models").TrimEnd('/');
            var endpoint = $"{baseUrl}/{model}:generateContent";
            var body = new
            {
                systemInstruction = new { parts = new[] { new { text = SystemPrompt } } },
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[] { new { text = $"{context}\n\nCÂU HỎI KHÁCH: {message}" } }
                    }
                },
                generationConfig = new { temperature = 0.35, maxOutputTokens = 700 }
            };

            try
            {
                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
                httpRequest.Headers.Add("x-goog-api-key", apiKey);
                httpRequest.Content = JsonContent.Create(body);

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
                using var response = await _httpClient.SendAsync(httpRequest, cts.Token);
                if (!response.IsSuccessStatusCode)
                {
                    var debug = MapGeminiError(response.StatusCode);
                    _logger.LogWarning("Gemini request failed with status {StatusCode}: {Reason}", (int)response.StatusCode, response.ReasonPhrase);
                    return (string.Empty, debug);
                }

                await using var stream = await response.Content.ReadAsStreamAsync();
                using var json = await JsonDocument.ParseAsync(stream);
                var reply = json.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return (reply ?? string.Empty, null);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "Gemini request timed out.");
                return (string.Empty, "Gemini timeout");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini request failed.");
                return (string.Empty, "Gemini request failed");
            }
        }

        private ChatbotResponse BuildRuleBasedReply(
            string intent,
            IReadOnlyList<SuggestedFoodDto> foods,
            IReadOnlyList<SuggestedRecipeDto> recipes,
            IReadOnlyList<AllergyTerm> allergyTerms,
            IReadOnlyList<ExcludedFood> excludedFoods)
        {
            if (intent == "RecipeHelp" && recipes.Any())
            {
                var names = string.Join(", ", recipes.Take(2).Select(x => x.Title));
                return new ChatbotResponse
                {
                    Intent = intent,
                    Reply = $"Mình tìm được công thức {names} trong FoodieLab. Bạn mở thẻ công thức bên dưới để xem nguyên liệu và cách làm chi tiết nhé.",
                    SuggestedRecipes = recipes
                };
            }

            if (foods.Any())
            {
                var topFoods = foods.Take(3).ToList();
                var names = string.Join(" hoặc ", topFoods.Select(x => x.Name));
                var first = topFoods[0];
                var reason = first.IsVegetarian
                    ? "Món này hợp nếu bạn muốn ăn chay hoặc ăn nhẹ hơn."
                    : "Món này khá hợp nếu bạn muốn ăn no, vị dễ ăn và có trong menu hiện tại.";
                var allergyNote = BuildAllergyReplyNote(allergyTerms, excludedFoods);

                return new ChatbotResponse
                {
                    Intent = intent,
                    Reply = $"Mình gợi ý bạn thử {names} nhé.{allergyNote} {reason} Giá món đầu tiên là {(first.DiscountPrice ?? first.Price):N0}đ. Bạn xem thẻ món bên dưới rồi thêm vào giỏ nếu thấy hợp nha.",
                    SuggestedFoods = foods
                };
            }

            return new ChatbotResponse
            {
                Intent = intent,
                Reply = allergyTerms.Any()
                    ? "Mình chưa tìm được món đủ an toàn với dị ứng bạn vừa nêu. Bạn nên chọn món có nguyên liệu rõ ràng hoặc liên hệ nhân viên để được xác nhận."
                    : "Hiện FoodieLab chưa trả lời được ngay, bạn có thể thử lại hoặc chat với nhân viên.",
                NeedHumanSupport = false
            };
        }

        private static string BuildContext(
            IReadOnlyList<SuggestedFoodDto> foods,
            IReadOnlyList<SuggestedRecipeDto> recipes,
            IReadOnlyList<AllergyTerm> allergyTerms,
            IReadOnlyList<ExcludedFood> excludedFoods)
        {
            var builder = new StringBuilder();
            if (allergyTerms.Any())
            {
                builder.AppendLine("ALLERGY_CONSTRAINT:");
                builder.AppendLine($"Người dùng cần tránh: {string.Join(", ", allergyTerms.Select(x => x.DisplayName))}");
                builder.AppendLine();
            }

            builder.AppendLine("SAFE_FOODS:");
            foreach (var food in foods)
            {
                builder.AppendLine($"- Tên món: {food.Name}");
                builder.AppendLine($"  Giá: {food.Price:N0}đ");
                builder.AppendLine($"  Giá khuyến mãi: {(food.DiscountPrice.HasValue ? $"{food.DiscountPrice.Value:N0}đ" : "Không có")}");
                builder.AppendLine($"  Mô tả: {food.Description}");
                builder.AppendLine($"  Nguyên liệu: {food.Ingredients}");
                builder.AppendLine($"  Calo: {(food.Calories > 0 ? $"{food.Calories} kcal" : "Chưa cập nhật")}");
                builder.AppendLine($"  Khẩu phần: {food.ServingSize}");
                builder.AppendLine($"  Danh mục: {food.Category}");
                builder.AppendLine($"  Độ cay: {food.SpiceLevel}");
                builder.AppendLine($"  Dị ứng: {food.Allergens}");
                builder.AppendLine($"  Còn món: {(food.IsAvailable ? "Có" : "Không")}");
                builder.AppendLine($"  Link chi tiết món: {food.DetailUrl}");
            }

            builder.AppendLine();
            builder.AppendLine("EXCLUDED_FOODS:");
            foreach (var excluded in excludedFoods.Take(12))
            {
                builder.AppendLine($"- {excluded.FoodName}: bị loại vì có {excluded.Reason}");
            }

            builder.AppendLine();
            builder.AppendLine("CONTEXT CÔNG THỨC:");
            foreach (var recipe in recipes)
            {
                builder.AppendLine($"- Tên công thức: {recipe.Title}");
                builder.AppendLine($"  Mô tả: {recipe.Description}");
                builder.AppendLine($"  Nguyên liệu chính: {recipe.Ingredients}");
                builder.AppendLine($"  Link công thức: {recipe.Url}");
            }

            return builder.ToString();
        }

        private static string BuildAllergyReplyNote(IReadOnlyList<AllergyTerm> allergyTerms, IReadOnlyList<ExcludedFood> excludedFoods)
        {
            if (!allergyTerms.Any()) return string.Empty;

            var avoided = string.Join(", ", allergyTerms.Select(x => x.DisplayName));
            var note = $" Mình đã tránh các món có {avoided} cho bạn.";
            if (excludedFoods.Any())
            {
                note += $" Mình không gợi ý {string.Join(", ", excludedFoods.Take(3).Select(x => x.FoodName))} vì có {avoided}.";
            }

            return note;
        }

        private bool IsRateLimited(string ipAddress, string? sessionId)
        {
            var key = $"gemini-chat:{ipAddress}:{sessionId}";
            var count = _cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return 0;
            });

            if (count >= 12) return true;
            _cache.Set(key, count + 1, TimeSpan.FromMinutes(1));
            return false;
        }

        private string? DevelopmentDebug(string? message)
        {
            return _environment.IsDevelopment() ? message : null;
        }

        private static string MapGeminiError(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.BadRequest => "Gemini bad request",
                HttpStatusCode.Unauthorized => "Gemini API unauthorized",
                HttpStatusCode.Forbidden => "Gemini API forbidden",
                (HttpStatusCode)429 => "Gemini rate limit",
                _ => $"Gemini API error {(int)statusCode}"
            };
        }

        private string GetMessengerUrl()
        {
            return _configuration["SupportSettings:MessengerUrl"]
                ?? _configuration["MessengerUrl"]
                ?? string.Empty;
        }

        private static SuggestedFoodDto ToFoodDto(FoodItem food)
        {
            return new SuggestedFoodDto
            {
                Id = food.Id,
                Name = food.Name,
                Slug = food.Slug,
                ImageUrl = string.IsNullOrWhiteSpace(food.ImageUrl) ? "/images/foods/fallback-food.jpg" : food.ImageUrl,
                Description = food.Description,
                Price = food.Price,
                DiscountPrice = food.DiscountPrice,
                Category = food.FoodCategory?.Name ?? food.Category,
                Ingredients = food.Ingredients,
                Calories = food.Calories,
                ServingSize = food.ServingSize,
                SpiceLevel = food.SpiceLevel,
                Allergens = string.IsNullOrWhiteSpace(food.Allergens) ? food.AllergyNote : food.Allergens,
                IsAvailable = food.IsAvailable,
                IsVegetarian = food.IsVegetarian,
                IsFeatured = food.IsFeatured || food.IsBestSeller,
                DetailUrl = $"/menu/{food.Slug}"
            };
        }

        private static SuggestedRecipeDto ToRecipeDto(Recipe recipe)
        {
            return new SuggestedRecipeDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Slug = recipe.Slug,
                Description = recipe.Description,
                Ingredients = recipe.Ingredients,
                Url = $"/Recipe/Detail/{recipe.Slug}"
            };
        }

        private static int ScoreFood(FoodItem food, IReadOnlyList<string> keywords, string normalizedMessage)
        {
            var haystack = GetFoodAllergyHaystack(food);
            var score = keywords.Count(keyword => haystack.Contains(keyword));
            if (food.IsAvailable) score += 2;
            if (food.IsFeatured || food.IsBestSeller) score += 2;
            if (ContainsAny(normalizedMessage, "no", "no bung", "an no", "doi") && ContainsAny(haystack, "com", "bun", "pho", "mi", "burger", "lau")) score += 4;
            if (ContainsAny(normalizedMessage, "an nhe", "nhe bung") && ContainsAny(haystack, "salad", "goi", "soup", "chay", "healthy")) score += 4;
            return score;
        }

        private static int ScoreRecipe(Recipe recipe, IReadOnlyList<string> keywords, string normalizedMessage)
        {
            var haystack = Normalize($"{recipe.Title} {recipe.Description} {recipe.Ingredients} {recipe.CategoryLabel}");
            var score = keywords.Count(keyword => haystack.Contains(keyword));
            if (normalizedMessage.Contains("cach nau") || normalizedMessage.Contains("cong thuc")) score += 2;
            return score;
        }

        private static bool IsNotSpicy(FoodItem food)
        {
            var spice = Normalize($"{food.SpiceLevel} {food.Tag} {food.Description}");
            return !spice.Contains("cay") || spice.Contains("khong cay");
        }

        private static string DetectIntent(string message)
        {
            var normalized = Normalize(message);
            if (ContainsAny(normalized, "gap nhan vien", "tu van truc tiep", "chat nguoi that", "lien he shop", "nhan vien", "hotline", "messenger"))
            {
                return "HumanSupport";
            }

            if (ContainsAny(normalized, "don hang", "gio hang", "thanh toan", "van chuyen", "giao hang", "ma don", "tra cuu", "huy don", "doi dia chi"))
            {
                return "OrderSupport";
            }

            if (ContainsAny(normalized, "cach nau", "cong thuc", "lam mon", "tu nau", "huong dan che bien", "nguyen lieu de nau", "recipe", "che bien"))
            {
                return "RecipeHelp";
            }

            return "FoodRecommendation";
        }

        private static IReadOnlyList<AllergyTerm> ExtractAllergyTerms(string normalized)
        {
            if (!HasAllergyConstraint(normalized)) return Array.Empty<AllergyTerm>();

            var terms = AllergyCatalog()
                .Where(term => term.MessageTerms.Any(normalized.Contains))
                .ToList();

            return terms
                .GroupBy(x => x.DisplayName)
                .Select(x => x.First())
                .ToList();
        }

        private static bool HasAllergyConstraint(string normalized)
        {
            if (ContainsAny(normalized,
                "di ung", "khong an duoc", "khong dung duoc", "bi di ung voi",
                "tranh mon co", "khong an mon co", "khong co", "khong chua"))
            {
                return true;
            }

            return AllergyCatalog().Any(term =>
                term.MessageTerms.Any(alias =>
                    Regex.IsMatch(normalized, $@"\bkhong\s+(?:an|dung|duoc|chua|co)?\s*{Regex.Escape(alias)}\b")));
        }

        private static AllergyTerm? FindUnsafeAllergyTerm(FoodItem food, IReadOnlyList<AllergyTerm> allergyTerms)
        {
            var haystack = GetFoodAllergyHaystack(food);
            return allergyTerms.FirstOrDefault(term => term.SearchTerms.Any(haystack.Contains));
        }

        private static string GetFoodAllergyHaystack(FoodItem food)
        {
            return Normalize($"{food.Name} {food.Description} {food.DetailDescription} {food.Ingredients} {food.Allergens} {food.AllergyNote} {food.Category} {food.MainCategory} {food.Subcategory} {food.Tag} {food.Story}");
        }

        private static IReadOnlyList<AllergyTerm> AllergyCatalog()
        {
            return new[]
            {
                Term("gà", "ga", "thit ga", "chicken", "nuoc dung ga"),
                Term("trứng", "trung", "trung ga", "egg", "sot trung", "op la"),
                Term("sữa", "sua", "bo", "pho mai", "cheese", "milk", "kem", "cream", "butter"),
                Term("hải sản", "hai san", "tom", "cua", "muc", "ca", "seafood", "nuoc mam", "fish sauce"),
                Term("nước mắm", "nuoc mam", "fish sauce"),
                Term("đậu phộng", "dau phong", "lac", "peanut"),
                Term("mè", "me", "vung", "sesame"),
                Term("đậu nành", "dau nanh", "nuoc tuong", "soy", "soy sauce", "tofu", "dau hu"),
                Term("gluten", "gluten", "bot mi", "mi", "banh mi", "bread", "wheat"),
                Term("bò", "bo", "thit bo", "beef"),
                Term("nấm", "nam", "mushroom")
            };
        }

        private static AllergyTerm Term(string displayName, params string[] aliases)
        {
            var normalizedAliases = aliases.Select(Normalize).Distinct().ToArray();
            return new AllergyTerm(displayName, normalizedAliases, normalizedAliases);
        }

        private static IReadOnlyList<string> ExtractKeywords(string normalized)
        {
            var stopWords = new HashSet<string>
            {
                "toi", "minh", "muon", "an", "mon", "co", "khong", "duoi", "voi", "cho",
                "nay", "hom", "gi", "thi", "duoc", "nao", "can", "em", "anh", "chi", "hay",
                "di", "ung", "duoc", "dung", "tranh", "chua"
            };

            return Regex.Matches(normalized, @"[\p{L}\p{N}]+")
                .Select(x => x.Value)
                .Where(x => x.Length >= 2 && !stopWords.Contains(x))
                .Distinct()
                .Take(12)
                .ToList();
        }

        private static decimal? ExtractMaxPrice(string normalized)
        {
            var match = Regex.Match(normalized, @"(?:duoi|<=|nho hon|khong qua)\s*(\d{2,6})");
            if (!match.Success) match = Regex.Match(normalized, @"(\d{2,6})");
            if (!match.Success || !decimal.TryParse(match.Groups[1].Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
            {
                return null;
            }

            return price < 1000 ? price * 1000 : price;
        }

        private static bool ContainsAny(string source, params string[] keywords)
        {
            return keywords.Any(keyword => source.Contains(Normalize(keyword)));
        }

        private static string RemoveSensitiveInfo(string message)
        {
            var cleaned = Regex.Replace(message, @"\b(?:\+?84|0)\d{8,10}\b", "[so-dien-thoai-da-an]");
            cleaned = Regex.Replace(cleaned, @"\b\d{6,}\b", "[thong-tin-so-da-an]");
            cleaned = Regex.Replace(cleaned, @"(?i)(dia chi|địa chỉ|so nha|số nhà).{0,80}", "[dia-chi-da-an]");
            return cleaned;
        }

        private static string Normalize(string value)
        {
            var lower = (value ?? string.Empty).Trim().ToLowerInvariant();
            var chars = lower.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            return new string(chars)
                .Normalize(NormalizationForm.FormC)
                .Replace('đ', 'd');
        }

        private sealed record FoodSearchResult(
            IReadOnlyList<FoodItem> SafeFoods,
            IReadOnlyList<ExcludedFood> ExcludedFoods,
            IReadOnlyList<AllergyTerm> AllergyTerms)
        {
            public static FoodSearchResult Empty { get; } = new(Array.Empty<FoodItem>(), Array.Empty<ExcludedFood>(), Array.Empty<AllergyTerm>());
        }

        private sealed record ExcludedFood(string FoodName, string Reason);

        private sealed record AllergyTerm(string DisplayName, IReadOnlyList<string> MessageTerms, IReadOnlyList<string> SearchTerms);
    }
}
