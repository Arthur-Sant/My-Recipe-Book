namespace MyRecipeBook.Domain.DTOs;

public record GeneratedInstructionDto
{
    public int Step { get; init; }
    public string Text { get; init; } = string.Empty;
}

