namespace RecipeService.Domain.Exceptions;

public abstract class CustomException(string message) : Exception(message)
{
}