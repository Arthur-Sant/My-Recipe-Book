namespace MyRecipeBook.Domain.ValueObjects;

public abstract class MyRecipeBookRuleConstants
{
    public const int MAXIMUM_INGREDIENTS_GENERATE_RECIPE = 5;
    public const int MAXIMUM_IMAGE_URL_LIFETIME_IN_MINUTES = 10;
    public const int PASSWORD_RESET_CODE_VALIDITY_MINUTES = 10;
    public const int NUMBER_CHARACTERS_FOR_ACTION_CODE = 6;
}
