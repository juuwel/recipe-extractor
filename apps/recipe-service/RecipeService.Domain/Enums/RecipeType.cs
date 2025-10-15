namespace RecipeService.Domain.Enums;

public enum RecipeType
{
    MainDish,
    SideDish,
    Dessert,
    Breakfast,
    Test // TODO: Remove this
}

public static class RecipeTypeExtensions
{
    public static string ToDisplayString(this RecipeType recipeType)
    {
        return recipeType switch
        {
            RecipeType.MainDish => "Main Dish",
            RecipeType.SideDish => "Side Dish",
            RecipeType.Dessert => "Dessert",
            RecipeType.Breakfast => "Breakfast",
            RecipeType.Test => "Test",
            _ => throw new ArgumentOutOfRangeException(nameof(recipeType), recipeType, "Unknown recipe type")
        };
    }
}