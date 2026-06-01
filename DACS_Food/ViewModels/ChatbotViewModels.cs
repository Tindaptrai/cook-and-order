namespace DACS_Food.ViewModels
{
    public class ChatbotAskRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    public class ChatbotAskResponse
    {
        public string Message { get; set; } = string.Empty;
        public IReadOnlyList<string> SuggestedFoods { get; set; } = Array.Empty<string>();
        public IReadOnlyList<RecipeLinkViewModel> RecipeLinks { get; set; } = Array.Empty<RecipeLinkViewModel>();
        public IReadOnlyList<ShoppingListItemViewModel> ShoppingList { get; set; } = Array.Empty<ShoppingListItemViewModel>();
        public string MessengerUrl { get; set; } = string.Empty;
        public bool NeedHumanSupport { get; set; }
    }

    public class RecipeLinkViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class ShoppingListItemViewModel
    {
        public string Name { get; set; } = string.Empty;
    }
}
