-- ============================================================================
-- Corrige contas de acesso gravadas com id_usuario = 0 — Sistema de Cadastro (MySQL)
--
-- POR QUE:
--   Ate a correcao do LoginAccountApp.Create, a tela "Criar conta de acesso"
--   gravava a conta com id_usuario = 0 e so depois tentava copiar
--   id_conta_login -> id_usuario. O EF recusava essa copia (id_usuario e chave
--   alternativa), a tela mostrava erro e a conta ficava com id_usuario = 0.
--   Quem entra com uma dessas contas recebe UserId = 0 no token: o perfil nao
--   abre ("Conta nao encontrada") e a auditoria grava as alteracoes no usuario 0.
--
-- O QUE FAZ:
--   Para cada conta com id_usuario = 0, passa id_usuario = id_conta_login
--   (o padrao do sistema). Se houver UMA unica conta com id_usuario = 0, os
--   registros filhos com id_usuario = 0 (log_acesso, agendamentos, pagamentos,
--   usuarios_permissoes) passam a apontar para ela. Com VARIAS contas em 0 nao
--   da para saber de quem e cada registro: eles ficam como estao e aparecem no
--   PREVIEW para revisao manual.
--
-- ANTES DE RODAR:
--   * FACA BACKUP do banco (ex.: mysqldump).
--   * Rode primeiro a secao "PREVIEW" (somente SELECTs).
--   * A secao de EXECUCAO roda dentro de uma transacao; confira os resultados e
--     faca COMMIT (ou ROLLBACK para desfazer).
-- ============================================================================


-- ============================================================================
-- PREVIEW — somente leitura.
-- ============================================================================

-- 1. Contas afetadas
SELECT id_conta_login, id_usuario, nome, email, tipo_usuario
FROM contas_login
WHERE id_usuario = 0;

-- 2. Conflito: alguma OUTRA conta ja usa como id_usuario o id_conta_login de
--    uma conta afetada? (Se aparecer algo aqui, essa conta precisa de ajuste manual
--    e e ignorada pela execucao abaixo.)
SELECT afetada.id_conta_login, afetada.email, outra.id_conta_login AS conflita_com, outra.email AS email_conflito
FROM contas_login afetada
JOIN contas_login outra
  ON outra.id_usuario = afetada.id_conta_login
 AND outra.id_conta_login <> afetada.id_conta_login
WHERE afetada.id_usuario = 0;

-- 3. Registros filhos ligados ao usuario 0
SELECT 'log_acesso' AS tabela, COUNT(*) AS qtd FROM log_acesso WHERE id_usuario = 0
UNION ALL SELECT 'agendamentos', COUNT(*) FROM agendamentos WHERE id_usuario = 0
UNION ALL SELECT 'pagamentos', COUNT(*) FROM pagamentos WHERE id_usuario = 0
UNION ALL SELECT 'usuarios_permissoes', COUNT(*) FROM usuarios_permissoes WHERE id_usuario = 0;


-- ============================================================================
-- EXECUCAO
-- ============================================================================

START TRANSACTION;

-- Os filhos e a conta mudam de valor na mesma transacao; a checagem de FK volta
-- logo em seguida.
SET FOREIGN_KEY_CHECKS = 0;

-- Contas que podem ser corrigidas (sem o conflito do PREVIEW 2).
DROP TEMPORARY TABLE IF EXISTS tmp_contas_corrigir;
CREATE TEMPORARY TABLE tmp_contas_corrigir AS
SELECT afetada.id_conta_login
FROM contas_login afetada
WHERE afetada.id_usuario = 0
  AND NOT EXISTS (
      SELECT 1 FROM contas_login outra
      WHERE outra.id_usuario = afetada.id_conta_login
        AND outra.id_conta_login <> afetada.id_conta_login
  );

-- Filhos: so quando ha exatamente UMA conta com id_usuario = 0 (dono inequivoco).
SET @unica_conta = (
    SELECT CASE WHEN COUNT(*) = 1 THEN MIN(id_conta_login) END
    FROM contas_login WHERE id_usuario = 0
);

UPDATE log_acesso          SET id_usuario = @unica_conta WHERE id_usuario = 0 AND @unica_conta IN (SELECT id_conta_login FROM tmp_contas_corrigir);
UPDATE agendamentos        SET id_usuario = @unica_conta WHERE id_usuario = 0 AND @unica_conta IN (SELECT id_conta_login FROM tmp_contas_corrigir);
UPDATE pagamentos          SET id_usuario = @unica_conta WHERE id_usuario = 0 AND @unica_conta IN (SELECT id_conta_login FROM tmp_contas_corrigir);
UPDATE usuarios_permissoes SET id_usuario = @unica_conta WHERE id_usuario = 0 AND @unica_conta IN (SELECT id_conta_login FROM tmp_contas_corrigir);

-- Contas
UPDATE contas_login
SET id_usuario = id_conta_login
WHERE id_usuario = 0
  AND id_conta_login IN (SELECT id_conta_login FROM tmp_contas_corrigir);

SET FOREIGN_KEY_CHECKS = 1;

-- Conferencia: deve vir vazio (ou so as contas com conflito do PREVIEW 2).
SELECT id_conta_login, id_usuario, email FROM contas_login WHERE id_usuario = 0;

-- Se estiver tudo certo:
-- COMMIT;
-- Para desfazer:
-- ROLLBACK;
