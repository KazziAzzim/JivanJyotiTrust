using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JivanJyotiTrustApp.Migrations
{
    /// <inheritdoc />
    public partial class TrainingModuleAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "GalleryItems");

            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "GalleryItems",
                newName: "ImageUrl");

            migrationBuilder.AddColumn<int>(
                name: "TrainingId",
                table: "GalleryItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trainings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trainings_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GalleryItems_TrainingId",
                table: "GalleryItems",
                column: "TrainingId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name",
                table: "Cities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainings_CityId",
                table: "Trainings",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryItems_Trainings_TrainingId",
                table: "GalleryItems",
                column: "TrainingId",
                principalTable: "Trainings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GalleryItems_Trainings_TrainingId",
                table: "GalleryItems");

            migrationBuilder.DropTable(
                name: "Trainings");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_GalleryItems_TrainingId",
                table: "GalleryItems");

            migrationBuilder.DropColumn(
                name: "TrainingId",
                table: "GalleryItems");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "GalleryItems",
                newName: "ImagePath");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "GalleryItems",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);
        }
    }
}
