using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JivanJyotiTrustApp.Migrations
{
    /// <inheritdoc />
    public partial class TrainingModuleModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Trainings",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Trainings");
        }
    }
}
