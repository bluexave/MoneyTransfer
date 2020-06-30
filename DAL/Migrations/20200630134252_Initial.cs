using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(nullable: false),
                    Balance = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "MTTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DestinationAccountIdAccountId = table.Column<int>(nullable: false),
                    SourceAccountIdAccountId = table.Column<int>(nullable: true),
                    TransferAmount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MTTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_MTTransactions_Accounts_DestinationAccountIdAccountId",
                        column: x => x.DestinationAccountIdAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MTTransactions_Accounts_SourceAccountIdAccountId",
                        column: x => x.SourceAccountIdAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserName",
                table: "Accounts",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MTTransactions_DestinationAccountIdAccountId",
                table: "MTTransactions",
                column: "DestinationAccountIdAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_MTTransactions_SourceAccountIdAccountId",
                table: "MTTransactions",
                column: "SourceAccountIdAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MTTransactions");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
