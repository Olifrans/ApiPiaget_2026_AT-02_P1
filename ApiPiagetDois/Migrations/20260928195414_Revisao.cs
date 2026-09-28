using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiPiagetDois.Migrations
{
    /// <inheritdoc />
    public partial class Revisao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Escolas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EscolaNome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EscolaTelefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EscolaEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EscolaEndereco = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escolas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdmEscolas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdmNomeFuncionario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdmFuncaoFuncionario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdmMatriculaAluno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdmDataCadastroFuncionario = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdmDataCadastroAluno = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EscolaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmEscolas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmEscolas_Escolas_EscolaId",
                        column: x => x.EscolaId,
                        principalTable: "Escolas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alunos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlunoNome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AlunoTelefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AlunoEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AlunoMatricula = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EscolaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alunos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alunos_Escolas_EscolaId",
                        column: x => x.EscolaId,
                        principalTable: "Escolas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Professores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfessorNome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProfessorTelefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProfessorEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProfessorDisciplina = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EscolaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Professores_Escolas_EscolaId",
                        column: x => x.EscolaId,
                        principalTable: "Escolas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlunoProfessor",
                columns: table => new
                {
                    ProfessorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlunoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlunoProfessor", x => new { x.ProfessorId, x.AlunoId });
                    table.ForeignKey(
                        name: "FK_AlunoProfessor_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlunoProfessor_Professores_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdmEscolas_EscolaId",
                table: "AdmEscolas",
                column: "EscolaId");

            migrationBuilder.CreateIndex(
                name: "IX_AlunoProfessor_AlunoId",
                table: "AlunoProfessor",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_AlunoMatricula",
                table: "Alunos",
                column: "AlunoMatricula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_EscolaId",
                table: "Alunos",
                column: "EscolaId");

            migrationBuilder.CreateIndex(
                name: "IX_Professores_EscolaId",
                table: "Professores",
                column: "EscolaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmEscolas");

            migrationBuilder.DropTable(
                name: "AlunoProfessor");

            migrationBuilder.DropTable(
                name: "Alunos");

            migrationBuilder.DropTable(
                name: "Professores");

            migrationBuilder.DropTable(
                name: "Escolas");
        }
    }
}
