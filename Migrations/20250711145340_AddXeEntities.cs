using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Migrations
{
    public partial class AddXeEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoaDonDatXes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenXe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BienSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenTaiXe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CCCDTaiXe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SDTTaiXe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenKhach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CCCDKhach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SDTKhach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiemA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiemB = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KhoangCach = table.Column<double>(type: "float", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDonDatXes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoaiXes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoaiXe = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiXes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrangThaiXes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrangThaiXes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Xes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenXe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BienSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaTien1Km = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenTaiXe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CCCDTaiXe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SDTTaiXe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiXeId = table.Column<int>(type: "int", nullable: false),
                    TrangThaiXeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Xes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Xes_LoaiXes_LoaiXeId",
                        column: x => x.LoaiXeId,
                        principalTable: "LoaiXes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Xes_TrangThaiXes_TrangThaiXeId",
                        column: x => x.TrangThaiXeId,
                        principalTable: "TrangThaiXes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Xes_LoaiXeId",
                table: "Xes",
                column: "LoaiXeId");

            migrationBuilder.CreateIndex(
                name: "IX_Xes_TrangThaiXeId",
                table: "Xes",
                column: "TrangThaiXeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoaDonDatXes");

            migrationBuilder.DropTable(
                name: "Xes");

            migrationBuilder.DropTable(
                name: "LoaiXes");

            migrationBuilder.DropTable(
                name: "TrangThaiXes");
        }
    }
}
