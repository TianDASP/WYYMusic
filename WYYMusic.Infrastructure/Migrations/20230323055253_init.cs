using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WYYMusic.Infrastructure.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_Albums",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WYYId = table.Column<long>(type: "bigint", nullable: true),
                    PicUrl = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsVisible = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Albums", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_Artists",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Trans = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WYYId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Artists", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_Musics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DurationInMilliSecond = table.Column<int>(type: "int", nullable: false),
                    AudioUrl = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LyricUrl = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    No = table.Column<int>(type: "int", nullable: false),
                    WYYId = table.Column<long>(type: "bigint", nullable: true),
                    AlbumId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Musics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_Musics_T_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "T_Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_Artists_Albums",
                columns: table => new
                {
                    AlbumsId = table.Column<long>(type: "bigint", nullable: false),
                    ArtistsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Artists_Albums", x => new { x.AlbumsId, x.ArtistsId });
                    table.ForeignKey(
                        name: "FK_T_Artists_Albums_T_Albums_AlbumsId",
                        column: x => x.AlbumsId,
                        principalTable: "T_Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_Artists_Albums_T_Artists_ArtistsId",
                        column: x => x.ArtistsId,
                        principalTable: "T_Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_Musics_Artists",
                columns: table => new
                {
                    ArtistsId = table.Column<long>(type: "bigint", nullable: false),
                    MusicsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Musics_Artists", x => new { x.ArtistsId, x.MusicsId });
                    table.ForeignKey(
                        name: "FK_T_Musics_Artists_T_Artists_ArtistsId",
                        column: x => x.ArtistsId,
                        principalTable: "T_Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_Musics_Artists_T_Musics_MusicsId",
                        column: x => x.MusicsId,
                        principalTable: "T_Musics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_T_Albums_IsDeleted",
                table: "T_Albums",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_T_Artists_IsDeleted",
                table: "T_Artists",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_T_Artists_Albums_ArtistsId",
                table: "T_Artists_Albums",
                column: "ArtistsId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Musics_AlbumId",
                table: "T_Musics",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Musics_IsDeleted",
                table: "T_Musics",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_T_Musics_Artists_MusicsId",
                table: "T_Musics_Artists",
                column: "MusicsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Artists_Albums");

            migrationBuilder.DropTable(
                name: "T_Musics_Artists");

            migrationBuilder.DropTable(
                name: "T_Artists");

            migrationBuilder.DropTable(
                name: "T_Musics");

            migrationBuilder.DropTable(
                name: "T_Albums");
        }
    }
}
