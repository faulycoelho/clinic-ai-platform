using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace Clinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PgVectorEmbedding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "KnowledgeDocumentChunks",
                type: "vector(3072)",
                nullable: false,
                oldClrType: typeof(float[]),
                oldType: "real[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.AlterColumn<float[]>(
                name: "Embedding",
                table: "KnowledgeDocumentChunks",
                type: "real[]",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector(3072)");
        }
    }
}
