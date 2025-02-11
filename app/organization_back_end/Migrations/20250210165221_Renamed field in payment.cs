using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace organization_back_end.Migrations
{
    /// <inheritdoc />
    public partial class Renamedfieldinpayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CardNumber",
                table: "Payments",
                newName: "PaymentNumberStripe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentNumberStripe",
                table: "Payments",
                newName: "CardNumber");
        }
    }
}
