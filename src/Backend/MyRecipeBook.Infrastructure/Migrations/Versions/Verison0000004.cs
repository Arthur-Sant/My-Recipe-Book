using FluentMigrator;

namespace MyRecipeBook.Infrastructure.Migrations.Versions;

[Migration(DatabaseVersions.TABLE_CODE_TO_PERFORM_ACTIONS, "Create table to save a code when the user forgot password")]
public class Verison0000004 : VersionBase
{
    public override void Up()
    {
        CreateTable("CodeToPerformActions")
            .WithColumn("Value").AsString().NotNullable()
            .WithColumn("UserId").AsInt64().NotNullable().ForeignKey("FK_CodeToPerformActions_User_Id", "Users", "Id");
    }
}
