using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinApiJwtAuth.Migrations
{
	public partial class AddUserRole : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Role",
				table: "Users",
				type: "TEXT",
				nullable: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Role",
				table: "Users");
		}
	}
}
