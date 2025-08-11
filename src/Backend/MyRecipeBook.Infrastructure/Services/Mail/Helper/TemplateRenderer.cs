namespace MyRecipeBook.Infrastructure.Services.Mail.Helper;

public static class TemplateRenderer
{
    public async static Task<string> GetTemplateString(object model, string template)
    {
        var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        var templatePath = Path.Combine(currentDirectory!, "Services", "Mail", "Templates", template);

        if(!File.Exists(templatePath))
            throw new FileNotFoundException($"Template {template} não encontrado.");

        var html = await File.ReadAllTextAsync(templatePath);

        foreach(var prop in model.GetType().GetProperties())
        {
            var placeholder = $"{{{prop.Name}}}";
            var value = prop.GetValue(model)?.ToString() ?? string.Empty;
            html = html.Replace(placeholder, value);
        }

        return html;
    }
}
