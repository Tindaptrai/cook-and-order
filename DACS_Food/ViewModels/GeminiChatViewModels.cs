namespace DACS_Food.ViewModels
{
    public class ChatbotRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? SessionId { get; set; }
        public string? PageUrl { get; set; }
        public IReadOnlyList<ChatMessageDto> ClientHistory { get; set; } = Array.Empty<ChatMessageDto>();
    }

    public class ChatbotResponse
    {
        public string Reply { get; set; } = string.Empty;
        public string Intent { get; set; } = "FoodRecommendation";
        public IReadOnlyList<SuggestedFoodDto> SuggestedFoods { get; set; } = Array.Empty<SuggestedFoodDto>();
        public IReadOnlyList<SuggestedRecipeDto> SuggestedRecipes { get; set; } = Array.Empty<SuggestedRecipeDto>();
        public bool NeedHumanSupport { get; set; }
        public string? MessengerUrl { get; set; }
        public string? DebugMessage { get; set; }
    }

    public class ChatHistoryResponse
    {
        public IReadOnlyList<ChatMessageDto> Messages { get; set; } = Array.Empty<ChatMessageDto>();
        public bool LoadedFromDatabase { get; set; }
    }

    public class ChatMessageDto
    {
        public string Role { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Intent { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PageUrl { get; set; }
        public string? MetadataJson { get; set; }
    }

    public class SuggestedFoodDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public int Calories { get; set; }
        public string ServingSize { get; set; } = string.Empty;
        public string SpiceLevel { get; set; } = string.Empty;
        public string Allergens { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsFeatured { get; set; }
        public string DetailUrl { get; set; } = string.Empty;
    }

    public class SuggestedRecipeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
