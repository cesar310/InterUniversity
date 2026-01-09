using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentEnrollment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email_verification_token",
                table: "users",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "email_verification_token_expiry",
                table: "users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "email_verified",
                table: "users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "must_change_password",
                table: "users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "password_reset_token",
                table: "users",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "password_reset_token_expiry",
                table: "users",
                type: "datetime(6)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "idx_email_verification_token",
                table: "users",
                column: "email_verification_token");

            migrationBuilder.CreateIndex(
                name: "idx_password_reset_token",
                table: "users",
                column: "password_reset_token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_email_verification_token",
                table: "users");

            migrationBuilder.DropIndex(
                name: "idx_password_reset_token",
                table: "users");

            migrationBuilder.DropColumn(
                name: "email_verification_token",
                table: "users");

            migrationBuilder.DropColumn(
                name: "email_verification_token_expiry",
                table: "users");

            migrationBuilder.DropColumn(
                name: "email_verified",
                table: "users");

            migrationBuilder.DropColumn(
                name: "must_change_password",
                table: "users");

            migrationBuilder.DropColumn(
                name: "password_reset_token",
                table: "users");

            migrationBuilder.DropColumn(
                name: "password_reset_token_expiry",
                table: "users");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 1, 8, 3, 2, 19, 167, DateTimeKind.Utc).AddTicks(1941));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 2,
                column: "created_at",
                value: new DateTime(2026, 1, 8, 3, 2, 19, 167, DateTimeKind.Utc).AddTicks(2187));

            migrationBuilder.UpdateData(
                table: "system_config",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 8, 3, 2, 19, 179, DateTimeKind.Utc).AddTicks(6514), new DateTime(2026, 1, 8, 3, 2, 19, 179, DateTimeKind.Utc).AddTicks(6726) });

            migrationBuilder.UpdateData(
                table: "system_config",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 8, 3, 2, 19, 179, DateTimeKind.Utc).AddTicks(6902), new DateTime(2026, 1, 8, 3, 2, 19, 179, DateTimeKind.Utc).AddTicks(6902) });

            migrationBuilder.UpdateData(
                table: "system_config",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 8, 3, 2, 19, 179, DateTimeKind.Utc).AddTicks(6904), new DateTime(2026, 1, 8, 3, 2, 19, 179, DateTimeKind.Utc).AddTicks(6904) });

            migrationBuilder.UpdateData(
                table: "system_config",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 8, 3, 2, 19, 179, DateTimeKind.Utc).AddTicks(6906), new DateTime(2026, 1, 8, 3, 2, 19, 179, DateTimeKind.Utc).AddTicks(6906) });
        }
    }
}
