using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class DatosVillas01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "id", "Amenidad", "Area", "Detalle", "FechaActualizacion", "FechaCreacion", "ImagenUrl", "Nombre", "Tarifa" },
                values: new object[,]
                {
                    { 1, "Zoo", 50, "cabañas, naturaleza", new DateTime(2023, 6, 9, 9, 19, 54, 487, DateTimeKind.Local).AddTicks(2730), new DateTime(2023, 6, 9, 9, 19, 54, 487, DateTimeKind.Local).AddTicks(2715), "", "Villa Real", 150.0 },
                    { 2, "Mar", 70, "cabañas, mar", new DateTime(2023, 6, 9, 9, 19, 54, 487, DateTimeKind.Local).AddTicks(2735), new DateTime(2023, 6, 9, 9, 19, 54, 487, DateTimeKind.Local).AddTicks(2734), "", "Villa Nueva", 180.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "id",
                keyValue: 2);
        }
    }
}
