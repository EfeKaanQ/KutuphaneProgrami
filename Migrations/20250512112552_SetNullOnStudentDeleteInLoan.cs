using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutuphaneProgrami.Migrations
{
    /// <inheritdoc />
    public partial class SetNullOnStudentDeleteInLoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Students_StudentId",
                table: "Loans");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Students_StudentId",
                table: "Loans",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Students_StudentId",
                table: "Loans");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Students_StudentId",
                table: "Loans",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
