using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Migrations
{
    public partial class AddHoaDonDoHong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hoa_Don_Do_Hong",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHoaDon = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TenDoHong = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<float>(type: "real", nullable: false),
                    ThanhTien = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Hoa_Don_Do_Hong__Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoaDonDoHong_HoaDon",
                        column: x => x.MaHoaDon,
                        principalTable: "Hoa_Don",
                        principalColumn: "MaHoaDon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hoa_Don_Do_Hong_MaHoaDon",
                table: "Hoa_Don_Do_Hong",
                column: "MaHoaDon");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hoa_Don_Do_Hong");
        }
    }
}
