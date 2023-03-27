using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SignatureStatuses.Migrations
{
    /// <inheritdoc />
    public partial class SolanaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreateEggEvents",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id = table.Column<long>(type: "bigint", nullable: false),
                    Authority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BeeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Collection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateEggEvents", x => x.UserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreateEggEvents");
        }
    }
}
