using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeCadastro.Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateMySqlComBloodType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "blood_types",
                columns: table => new
                {
                    id_blood_type = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blood_types", x => x.id_blood_type);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "condicoesclinicas",
                columns: table => new
                {
                    id_condicao = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome_condicao = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_condicoesclinicas", x => x.id_condicao);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "contas_login",
                columns: table => new
                {
                    id_conta_login = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_usuario = table.Column<long>(type: "bigint", nullable: false),
                    email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    senha_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo_usuario = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ultimo_login = table.Column<DateTime>(type: "datetime", nullable: true),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contas_login", x => x.id_conta_login);
                    table.UniqueConstraint("AK_contas_login_id_usuario", x => x.id_usuario);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "departamentos",
                columns: table => new
                {
                    id_departamento = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome_departamento = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departamentos", x => x.id_departamento);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "doencas",
                columns: table => new
                {
                    id_doenca = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cid = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doencas", x => x.id_doenca);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "medicamentos",
                columns: table => new
                {
                    id_medicamento = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dosagem = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    via_administracao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medicamentos", x => x.id_medicamento);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "permissoes_disponiveis",
                columns: table => new
                {
                    id_permissao = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome_permissao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modulo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissoes_disponiveis", x => x.id_permissao);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "acolhidos",
                columns: table => new
                {
                    id_acolhido = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_nascimento = table.Column<DateTime>(type: "date", nullable: false),
                    telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    documento = table.Column<string>(type: "varchar(12)", maxLength: 12, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sexo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cpf = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacoes = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_cadastro = table.Column<DateTime>(type: "datetime", nullable: false),
                    id_blood_type = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acolhidos", x => x.id_acolhido);
                    table.ForeignKey(
                        name: "FK_acolhidos_blood_types_id_blood_type",
                        column: x => x.id_blood_type,
                        principalTable: "blood_types",
                        principalColumn: "id_blood_type",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "log_acesso",
                columns: table => new
                {
                    id_log = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_usuario = table.Column<long>(type: "bigint", nullable: false),
                    acao = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_hora = table.Column<DateTime>(type: "datetime", nullable: false),
                    ip_acesso = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacao = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_log_acesso", x => x.id_log);
                    table.ForeignKey(
                        name: "FK_log_acesso_contas_login_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "contas_login",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "funcionarios",
                columns: table => new
                {
                    id_funcionario = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cpf = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cargo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_admissao = table.Column<DateTime>(type: "date", nullable: false),
                    id_departamento = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_funcionarios", x => x.id_funcionario);
                    table.ForeignKey(
                        name: "FK_funcionarios_departamentos_id_departamento",
                        column: x => x.id_departamento,
                        principalTable: "departamentos",
                        principalColumn: "id_departamento",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "usuarios_permissoes",
                columns: table => new
                {
                    id_usuario_permissao = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_usuario = table.Column<long>(type: "bigint", nullable: false),
                    id_permissao = table.Column<long>(type: "bigint", nullable: false),
                    data_concessao = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios_permissoes", x => x.id_usuario_permissao);
                    table.ForeignKey(
                        name: "FK_usuarios_permissoes_contas_login_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "contas_login",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_usuarios_permissoes_permissoes_disponiveis_id_permissao",
                        column: x => x.id_permissao,
                        principalTable: "permissoes_disponiveis",
                        principalColumn: "id_permissao",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "acolhido_condicaoclinica",
                columns: table => new
                {
                    id_acolhido_condicao = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_acolhido = table.Column<long>(type: "bigint", nullable: false),
                    id_condicao = table.Column<long>(type: "bigint", nullable: false),
                    data_diagnostico = table.Column<DateTime>(type: "date", nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acolhido_condicaoclinica", x => x.id_acolhido_condicao);
                    table.ForeignKey(
                        name: "FK_acolhido_condicaoclinica_acolhidos_id_acolhido",
                        column: x => x.id_acolhido,
                        principalTable: "acolhidos",
                        principalColumn: "id_acolhido",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_acolhido_condicaoclinica_condicoesclinicas_id_condicao",
                        column: x => x.id_condicao,
                        principalTable: "condicoesclinicas",
                        principalColumn: "id_condicao",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "acolhido_doenca",
                columns: table => new
                {
                    id_acolhido_doenca = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_acolhido = table.Column<long>(type: "bigint", nullable: false),
                    id_doenca = table.Column<long>(type: "bigint", nullable: false),
                    data_diagnostico = table.Column<DateTime>(type: "date", nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acolhido_doenca", x => x.id_acolhido_doenca);
                    table.ForeignKey(
                        name: "FK_acolhido_doenca_acolhidos_id_acolhido",
                        column: x => x.id_acolhido,
                        principalTable: "acolhidos",
                        principalColumn: "id_acolhido",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_acolhido_doenca_doencas_id_doenca",
                        column: x => x.id_doenca,
                        principalTable: "doencas",
                        principalColumn: "id_doenca",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "agendamentos",
                columns: table => new
                {
                    id_agendamento = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_acolhido = table.Column<long>(type: "bigint", nullable: false),
                    id_usuario = table.Column<long>(type: "bigint", nullable: false),
                    tipo_atendimento = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_hora = table.Column<DateTime>(type: "datetime", nullable: false),
                    responsavel = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacoes = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agendamentos", x => x.id_agendamento);
                    table.ForeignKey(
                        name: "FK_agendamentos_acolhidos_id_acolhido",
                        column: x => x.id_acolhido,
                        principalTable: "acolhidos",
                        principalColumn: "id_acolhido",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_agendamentos_contas_login_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "contas_login",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "responsaveis",
                columns: table => new
                {
                    id_responsavel = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_acolhido = table.Column<long>(type: "bigint", nullable: false),
                    nome = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    parentesco = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    endereco = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_responsaveis", x => x.id_responsavel);
                    table.ForeignKey(
                        name: "FK_responsaveis_acolhidos_id_acolhido",
                        column: x => x.id_acolhido,
                        principalTable: "acolhidos",
                        principalColumn: "id_acolhido",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "acolhido_funcionario",
                columns: table => new
                {
                    id_acolhido_funcionario = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_acolhido = table.Column<long>(type: "bigint", nullable: false),
                    id_funcionario = table.Column<long>(type: "bigint", nullable: false),
                    funcao_responsavel = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_inicio = table.Column<DateTime>(type: "date", nullable: true),
                    data_fim = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acolhido_funcionario", x => x.id_acolhido_funcionario);
                    table.ForeignKey(
                        name: "FK_acolhido_funcionario_acolhidos_id_acolhido",
                        column: x => x.id_acolhido,
                        principalTable: "acolhidos",
                        principalColumn: "id_acolhido",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_acolhido_funcionario_funcionarios_id_funcionario",
                        column: x => x.id_funcionario,
                        principalTable: "funcionarios",
                        principalColumn: "id_funcionario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "medicamento_acolhido_condicaoclinica",
                columns: table => new
                {
                    id_medicamento_acond = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_medicamento = table.Column<long>(type: "bigint", nullable: false),
                    id_acolhido_condicao = table.Column<long>(type: "bigint", nullable: false),
                    dosagem_prescrita = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    frequencia = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_inicio = table.Column<DateTime>(type: "date", nullable: true),
                    data_fim = table.Column<DateTime>(type: "date", nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medicamento_acolhido_condicaoclinica", x => x.id_medicamento_acond);
                    table.ForeignKey(
                        name: "FK_medicamento_acolhido_condicaoclinica_acolhido_condicaoclinic~",
                        column: x => x.id_acolhido_condicao,
                        principalTable: "acolhido_condicaoclinica",
                        principalColumn: "id_acolhido_condicao",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_medicamento_acolhido_condicaoclinica_medicamentos_id_medicam~",
                        column: x => x.id_medicamento,
                        principalTable: "medicamentos",
                        principalColumn: "id_medicamento",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "atendimentos",
                columns: table => new
                {
                    id_atendimento = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_agendamento = table.Column<long>(type: "bigint", nullable: false),
                    id_acolhido = table.Column<long>(type: "bigint", nullable: false),
                    id_usuario = table.Column<long>(type: "bigint", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_atendimento = table.Column<DateTime>(type: "datetime", nullable: false),
                    encaminhamento = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacoes = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_atendimentos", x => x.id_atendimento);
                    table.ForeignKey(
                        name: "FK_atendimentos_acolhidos_id_acolhido",
                        column: x => x.id_acolhido,
                        principalTable: "acolhidos",
                        principalColumn: "id_acolhido",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_atendimentos_agendamentos_id_agendamento",
                        column: x => x.id_agendamento,
                        principalTable: "agendamentos",
                        principalColumn: "id_agendamento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_atendimentos_contas_login_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "contas_login",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pagamentos",
                columns: table => new
                {
                    id_pagamento = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_agendamento = table.Column<long>(type: "bigint", nullable: false),
                    id_usuario = table.Column<long>(type: "bigint", nullable: false),
                    valor_total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    metodo_pagamento = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_pagamento = table.Column<DateTime>(type: "datetime", nullable: false),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_transacao_externa = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagamentos", x => x.id_pagamento);
                    table.ForeignKey(
                        name: "FK_pagamentos_agendamentos_id_agendamento",
                        column: x => x.id_agendamento,
                        principalTable: "agendamentos",
                        principalColumn: "id_agendamento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pagamentos_contas_login_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "contas_login",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_acolhido_condicaoclinica_id_acolhido",
                table: "acolhido_condicaoclinica",
                column: "id_acolhido");

            migrationBuilder.CreateIndex(
                name: "IX_acolhido_condicaoclinica_id_condicao",
                table: "acolhido_condicaoclinica",
                column: "id_condicao");

            migrationBuilder.CreateIndex(
                name: "IX_acolhido_doenca_id_acolhido",
                table: "acolhido_doenca",
                column: "id_acolhido");

            migrationBuilder.CreateIndex(
                name: "IX_acolhido_doenca_id_doenca",
                table: "acolhido_doenca",
                column: "id_doenca");

            migrationBuilder.CreateIndex(
                name: "IX_acolhido_funcionario_id_acolhido",
                table: "acolhido_funcionario",
                column: "id_acolhido");

            migrationBuilder.CreateIndex(
                name: "IX_acolhido_funcionario_id_funcionario",
                table: "acolhido_funcionario",
                column: "id_funcionario");

            migrationBuilder.CreateIndex(
                name: "IX_acolhidos_cpf",
                table: "acolhidos",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_acolhidos_id_blood_type",
                table: "acolhidos",
                column: "id_blood_type");

            migrationBuilder.CreateIndex(
                name: "IX_agendamentos_id_acolhido",
                table: "agendamentos",
                column: "id_acolhido");

            migrationBuilder.CreateIndex(
                name: "IX_agendamentos_id_usuario",
                table: "agendamentos",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_atendimentos_id_acolhido",
                table: "atendimentos",
                column: "id_acolhido");

            migrationBuilder.CreateIndex(
                name: "IX_atendimentos_id_agendamento",
                table: "atendimentos",
                column: "id_agendamento");

            migrationBuilder.CreateIndex(
                name: "IX_atendimentos_id_usuario",
                table: "atendimentos",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_contas_login_email",
                table: "contas_login",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_funcionarios_cpf",
                table: "funcionarios",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_funcionarios_id_departamento",
                table: "funcionarios",
                column: "id_departamento");

            migrationBuilder.CreateIndex(
                name: "IX_log_acesso_id_usuario",
                table: "log_acesso",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_medicamento_acolhido_condicaoclinica_id_acolhido_condicao",
                table: "medicamento_acolhido_condicaoclinica",
                column: "id_acolhido_condicao");

            migrationBuilder.CreateIndex(
                name: "IX_medicamento_acolhido_condicaoclinica_id_medicamento",
                table: "medicamento_acolhido_condicaoclinica",
                column: "id_medicamento");

            migrationBuilder.CreateIndex(
                name: "IX_pagamentos_id_agendamento",
                table: "pagamentos",
                column: "id_agendamento");

            migrationBuilder.CreateIndex(
                name: "IX_pagamentos_id_transacao_externa",
                table: "pagamentos",
                column: "id_transacao_externa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pagamentos_id_usuario",
                table: "pagamentos",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_responsaveis_id_acolhido",
                table: "responsaveis",
                column: "id_acolhido");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_permissoes_id_permissao",
                table: "usuarios_permissoes",
                column: "id_permissao");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_permissoes_id_usuario",
                table: "usuarios_permissoes",
                column: "id_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "acolhido_doenca");

            migrationBuilder.DropTable(
                name: "acolhido_funcionario");

            migrationBuilder.DropTable(
                name: "atendimentos");

            migrationBuilder.DropTable(
                name: "log_acesso");

            migrationBuilder.DropTable(
                name: "medicamento_acolhido_condicaoclinica");

            migrationBuilder.DropTable(
                name: "pagamentos");

            migrationBuilder.DropTable(
                name: "responsaveis");

            migrationBuilder.DropTable(
                name: "usuarios_permissoes");

            migrationBuilder.DropTable(
                name: "doencas");

            migrationBuilder.DropTable(
                name: "funcionarios");

            migrationBuilder.DropTable(
                name: "acolhido_condicaoclinica");

            migrationBuilder.DropTable(
                name: "medicamentos");

            migrationBuilder.DropTable(
                name: "agendamentos");

            migrationBuilder.DropTable(
                name: "permissoes_disponiveis");

            migrationBuilder.DropTable(
                name: "departamentos");

            migrationBuilder.DropTable(
                name: "condicoesclinicas");

            migrationBuilder.DropTable(
                name: "acolhidos");

            migrationBuilder.DropTable(
                name: "contas_login");

            migrationBuilder.DropTable(
                name: "blood_types");
        }
    }
}
