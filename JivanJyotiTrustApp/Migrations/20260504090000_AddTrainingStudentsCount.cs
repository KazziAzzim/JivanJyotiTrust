using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JivanJyotiTrustApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingStudentsCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentsCount",
                table: "Trainings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentsCount",
                table: "Trainings");
        }
    }
}
