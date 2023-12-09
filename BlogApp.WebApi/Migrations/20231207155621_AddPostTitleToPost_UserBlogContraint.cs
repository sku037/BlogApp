using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPostTitleToPost_UserBlogContraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostTitle",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostTitle",
                table: "Posts");
        }
    }
}
