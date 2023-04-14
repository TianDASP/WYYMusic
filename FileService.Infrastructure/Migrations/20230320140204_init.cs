using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileService.Infrastructure.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_FS_UploadedAudios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FileName = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileSHA256Hash = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BackupUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemoteUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_FS_UploadedAudios", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_FS_UploadedLyrics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FileName = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileSHA256Hash = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BackupUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemoteUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_FS_UploadedLyrics", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_FS_UploadedPics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FileName = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileSHA256Hash = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BackupUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemoteUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_FS_UploadedPics", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_T_FS_UploadedAudios_FileSHA256Hash_FileSizeInBytes",
                table: "T_FS_UploadedAudios",
                columns: new[] { "FileSHA256Hash", "FileSizeInBytes" });

            migrationBuilder.CreateIndex(
                name: "IX_T_FS_UploadedLyrics_FileSHA256Hash_FileSizeInBytes",
                table: "T_FS_UploadedLyrics",
                columns: new[] { "FileSHA256Hash", "FileSizeInBytes" });

            migrationBuilder.CreateIndex(
                name: "IX_T_FS_UploadedPics_FileSHA256Hash_FileSizeInBytes",
                table: "T_FS_UploadedPics",
                columns: new[] { "FileSHA256Hash", "FileSizeInBytes" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_FS_UploadedAudios");

            migrationBuilder.DropTable(
                name: "T_FS_UploadedLyrics");

            migrationBuilder.DropTable(
                name: "T_FS_UploadedPics");
        }
    }
}
