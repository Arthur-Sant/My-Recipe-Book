namespace MyRecipeBook.Domain.DTOs;

public record GeneratedInstructionDTO
{
    public int Step { get; init; }
    public string Text { get; init; } = string.Empty;
}

