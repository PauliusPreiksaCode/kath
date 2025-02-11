using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace organization_back_end.Migrations
{
    /// <inheritdoc />
    public partial class fixinghierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_LicencedUsers_LicencedUserId",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_LicenceLedgerEntries_LicencedUsers_UserId",
                table: "LicenceLedgerEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_OrganizationOwners_OwnerId",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationUsers_LicencedUsers_UserId",
                table: "OrganizationUsers");

            migrationBuilder.DropTable(
                name: "OrganizationOwners");

            migrationBuilder.DropTable(
                name: "LicencedUsers");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_AspNetUsers_LicencedUserId",
                table: "Entries",
                column: "LicencedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LicenceLedgerEntries_AspNetUsers_UserId",
                table: "LicenceLedgerEntries",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_AspNetUsers_OwnerId",
                table: "Organizations",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationUsers_AspNetUsers_UserId",
                table: "OrganizationUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_AspNetUsers_LicencedUserId",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_LicenceLedgerEntries_AspNetUsers_UserId",
                table: "LicenceLedgerEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_AspNetUsers_OwnerId",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationUsers_AspNetUsers_UserId",
                table: "OrganizationUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "LicencedUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicencedUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LicencedUsers_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationOwners",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationOwners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationOwners_LicencedUsers_Id",
                        column: x => x.Id,
                        principalTable: "LicencedUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_LicencedUsers_LicencedUserId",
                table: "Entries",
                column: "LicencedUserId",
                principalTable: "LicencedUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LicenceLedgerEntries_LicencedUsers_UserId",
                table: "LicenceLedgerEntries",
                column: "UserId",
                principalTable: "LicencedUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_OrganizationOwners_OwnerId",
                table: "Organizations",
                column: "OwnerId",
                principalTable: "OrganizationOwners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationUsers_LicencedUsers_UserId",
                table: "OrganizationUsers",
                column: "UserId",
                principalTable: "LicencedUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
