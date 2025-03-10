using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace organization_back_end.Migrations
{
    /// <inheritdoc />
    public partial class Romovednotebooksaddedtitletoentry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_NoteBooks_NoteBookId",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "NotebookEntries");

            migrationBuilder.DropTable(
                name: "NoteBooks");

            migrationBuilder.DropIndex(
                name: "IX_Groups_NoteBookId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "NoteBookId",
                table: "Groups");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Entries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Entries");

            migrationBuilder.AddColumn<Guid>(
                name: "NoteBookId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NoteBooks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoteBooks_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotebookEntries",
                columns: table => new
                {
                    No = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoteBookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotebookEntries", x => x.No);
                    table.ForeignKey(
                        name: "FK_NotebookEntries_NoteBooks_NoteBookId",
                        column: x => x.NoteBookId,
                        principalTable: "NoteBooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_NoteBookId",
                table: "Groups",
                column: "NoteBookId",
                unique: true,
                filter: "[NoteBookId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NotebookEntries_NoteBookId",
                table: "NotebookEntries",
                column: "NoteBookId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteBooks_GroupId",
                table: "NoteBooks",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_NoteBooks_NoteBookId",
                table: "Groups",
                column: "NoteBookId",
                principalTable: "NoteBooks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
