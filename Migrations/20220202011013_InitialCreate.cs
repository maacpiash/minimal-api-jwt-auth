using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalJWTAuth.Migrations
{
	public partial class InitialCreate : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "TEXT", nullable: false),
					FullName = table.Column<string>(type: "TEXT", nullable: true),
					Age = table.Column<int>(type: "INTEGER", nullable: false),
					Address = table.Column<string>(type: "TEXT", nullable: true),
					UserName = table.Column<string>(type: "TEXT", nullable: true),
					NormalizedUserName = table.Column<string>(type: "TEXT", nullable: true),
					Email = table.Column<string>(type: "TEXT", nullable: true),
					NormalizedEmail = table.Column<string>(type: "TEXT", nullable: true),
					EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
					PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
					SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
					ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
					PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
					PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
					TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
					LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
					LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
					AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Users", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Todos",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "TEXT", nullable: false),
					Title = table.Column<string>(type: "TEXT", nullable: true),
					IsDone = table.Column<bool>(type: "INTEGER", nullable: false),
					AssignedToId = table.Column<Guid>(type: "TEXT", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Todos", x => x.Id);
					table.ForeignKey(
						name: "FK_Todos_Users_AssignedToId",
						column: x => x.AssignedToId,
						principalTable: "Users",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Todos_AssignedToId",
				table: "Todos",
				column: "AssignedToId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Todos");

			migrationBuilder.DropTable(
				name: "Users");
		}
	}
}
