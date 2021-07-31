using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace eShopSolution.Data.Migrations
{
    public partial class RemoveSeoAliasInProduct31072021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeoAlias",
                table: "Products");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                nullable: false,
                defaultValue: new DateTime(2021, 7, 31, 12, 37, 11, 107, DateTimeKind.Local).AddTicks(6277),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 6, 13, 9, 54, 42, 961, DateTimeKind.Local).AddTicks(1020));

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "d7f21edf-4e17-4237-8911-0879638a80b7");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "94bb3bd7-75b0-4a13-b4e2-112ce898c78b", "AQAAAAEAACcQAAAAEPv0mq1Tf0glsyUneWTbnxvMpVgFlsTot/dEvuKkEHXK+PERulmGelnYTTlarSqojA==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 7, 31, 12, 37, 11, 121, DateTimeKind.Local).AddTicks(9289));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeoAlias",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 13, 9, 54, 42, 961, DateTimeKind.Local).AddTicks(1020),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2021, 7, 31, 12, 37, 11, 107, DateTimeKind.Local).AddTicks(6277));

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "29c48e22-11be-4333-94af-8bd992e0106f");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "a5627dc2-1aed-4abf-87b0-d4fcca714054", "AQAAAAEAACcQAAAAEP+8QirP8byoBcQFA4CTbhQn3+PH5ABLThbzVGpndOmtjPPZwJFurakqqdwgcRdp3g==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 6, 13, 9, 54, 42, 975, DateTimeKind.Local).AddTicks(8813));
        }
    }
}
