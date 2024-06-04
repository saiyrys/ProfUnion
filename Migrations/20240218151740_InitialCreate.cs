using Microsoft.EntityFrameworkCore.Migrations;
using Profunion.Models;

#nullable disable

namespace Profunion.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
          table: "Users",
          columns: new[] { "Username", "FirstName", "LastName", "PasswordHash", "userRole" },
          values: new object[] { "Test1", "test1", "test11", "1234", "Admin" });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropTable(
                name: "Users");*/
        }
    }
}
