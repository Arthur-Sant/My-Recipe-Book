using FluentMigrator;

namespace MyRecipeBook.Infrastructure.Migrations.Versions;

[Migration(DatabaseVersions.TABLE_TOKENS, "Create table to save the token")]
public class Verison0000005 : VersionBase
{
    public override void Up()
    {
        CreateTable("Tokens")
            .WithColumn("Value").AsString().NotNullable()
            .WithColumn("UserId").AsInt64().NotNullable().ForeignKey("FK_Tokens_User_Id", "Users", "Id");
    }
}
