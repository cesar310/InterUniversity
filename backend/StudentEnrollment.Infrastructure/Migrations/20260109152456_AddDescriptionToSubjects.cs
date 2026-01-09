using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentEnrollment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToSubjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "subjects",
                type: "TEXT",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 1, 9, 15, 24, 54, 985, DateTimeKind.Utc).AddTicks(3595));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 2,
                column: "created_at",
                value: new DateTime(2026, 1, 9, 15, 24, 54, 985, DateTimeKind.Utc).AddTicks(4141));

            migrationBuilder.UpdateData(
                table: "system_config",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 9, 15, 24, 55, 0, DateTimeKind.Utc).AddTicks(3099), new DateTime(2026, 1, 9, 15, 24, 55, 0, DateTimeKind.Utc).AddTicks(3429) });

            migrationBuilder.UpdateData(
                table: "system_config",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 9, 15, 24, 55, 0, DateTimeKind.Utc).AddTicks(3664), new DateTime(2026, 1, 9, 15, 24, 55, 0, DateTimeKind.Utc).AddTicks(3664) });

            migrationBuilder.UpdateData(
                table: "system_config",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 9, 15, 24, 55, 0, DateTimeKind.Utc).AddTicks(3666), new DateTime(2026, 1, 9, 15, 24, 55, 0, DateTimeKind.Utc).AddTicks(3667) });

            migrationBuilder.UpdateData(
                table: "system_config",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 9, 15, 24, 55, 0, DateTimeKind.Utc).AddTicks(3668), new DateTime(2026, 1, 9, 15, 24, 55, 0, DateTimeKind.Utc).AddTicks(3668) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "subjects");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 1, 8, 4, 3, 17, 963, DateTimeKind.Utc).AddTicks(8259));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 2,
                column: "created_at",
                value: new DateTime(2026, 1, 8, 4, 3, 17, 963, DateTimeKind.Utc).AddTicks(8490));

            migrationBuilder.UpdateData(
                table: "system_config",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 8, 4, 3, 17, 976, DateTimeKind.Utc).AddTicks(3943), new DateTime(2026, 1, 8, 4, 3, 17, 976, DateTimeKind.Utc).AddTicks(4149) });

            migrationBuilder.UpdateData(
                table: "system_config",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 8, 4, 3, 17, 976, DateTimeKind.Utc).AddTicks(4330), new DateTime(2026, 1, 8, 4, 3, 17, 976, DateTimeKind.Utc).AddTicks(4330) });

            migrationBuilder.UpdateData(
                table: "system_config",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 8, 4, 3, 17, 976, DateTimeKind.Utc).AddTicks(4332), new DateTime(2026, 1, 8, 4, 3, 17, 976, DateTimeKind.Utc).AddTicks(4332) });

            migrationBuilder.UpdateData(
                table: "system_config",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 8, 4, 3, 17, 976, DateTimeKind.Utc).AddTicks(4333), new DateTime(2026, 1, 8, 4, 3, 17, 976, DateTimeKind.Utc).AddTicks(4333) });
        }
    }
}
