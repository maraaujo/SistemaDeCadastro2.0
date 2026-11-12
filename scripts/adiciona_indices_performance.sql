-- =====================================================================
-- Indices de performance (colunas de data usadas em filtros e relatorios)
-- =====================================================================
-- Rode manualmente no banco (MySQL 8), UMA vez. Nao apaga nem altera dados:
-- apenas cria indices. Em tabelas grandes a criacao pode levar alguns segundos.
--
-- Por que: lembretes ("Proximos medicamentos"), relatorio do dia, historico e o
-- painel administrativo filtram por data. Sem indice, o MySQL le a tabela inteira
-- a cada consulta. As consultas do backend usam intervalo (>= inicio AND < fim)
-- justamente para poderem aproveitar estes indices.
--
-- ANTES de rodar, confira os nomes reais das colunas de data de cada tabela:
--     SHOW COLUMNS FROM administracoes_medicamento;
--     SHOW COLUMNS FROM log_acesso;
--     SHOW COLUMNS FROM agendamentos;
-- e veja os indices que ja existem (para nao criar duplicado):
--     SHOW INDEX FROM administracoes_medicamento;
--     SHOW INDEX FROM log_acesso;
--     SHOW INDEX FROM agendamentos;
-- Se algum nome de coluna abaixo for diferente no seu banco, ajuste antes de rodar.
-- Se um indice ja existir, o MySQL devolve erro 1061 (Duplicate key name) apenas
-- naquele comando; os demais podem ser executados normalmente.

-- Administracoes: busca por horario previsto (lembretes, relatorio do dia e
-- checagem de dose duplicada no mesmo minuto).
CREATE INDEX ix_adm_medicamento_data_prevista
    ON administracoes_medicamento (data_hora_prevista);

-- Administracoes: busca por horario administrado (relatorio do dia, doses do mes
-- e cuidadores ativos no painel administrativo).
CREATE INDEX ix_adm_medicamento_data_administrada
    ON administracoes_medicamento (data_hora_administrada);

-- Log de acesso: filtro por data na tela de auditoria.
CREATE INDEX ix_log_acesso_data_hora
    ON log_acesso (data_hora);

-- Agendamentos: filtro e ordenacao por data.
CREATE INDEX ix_agendamentos_data_hora
    ON agendamentos (data_hora);
