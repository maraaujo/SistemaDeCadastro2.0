-- ============================================================================
-- Limpeza de duplicados — Sistema de Cadastro (MySQL)
--
-- O QUE FAZ:
--   1. Medicamentos repetidos (mesma instituicao + mesmo nome)
--   2. Condicoes clinicas repetidas (mesmo nome)
--   3. Vinculos acolhido<->condicao repetidos (mesmo acolhido + condicao)
--   4. Medicamentos programados repetidos (mesmo medicamento + vinculo + horario)
--   5. Agendamentos/administracoes repetidos (mesmo prog. + acolhido + horario previsto)
--
-- Em cada etapa: mantem o registro de MENOR id, aponta as referencias (FKs) dos
-- filhos para o que foi mantido e, so entao, apaga os duplicados. A comparacao de
-- nomes ignora maiusculas/minusculas e espacos nas pontas (TRIM + LOWER).
--
-- ANTES DE RODAR:
--   * FACA BACKUP do banco (ex.: mysqldump).
--   * Rode primeiro a secao "PREVIEW" (somente SELECTs) para conferir o que sera
--     afetado. So depois rode a secao de EXECUCAO.
--   * A secao de EXECUCAO roda dentro de uma transacao; confira os resultados e,
--     se estiver tudo certo, faca COMMIT (ou ROLLBACK para desfazer).
-- ============================================================================


-- ============================================================================
-- PREVIEW — somente leitura. Mostra os grupos com duplicados.
-- ============================================================================

-- 1. Medicamentos duplicados (por instituicao + nome normalizado)
SELECT TRIM(LOWER(nome)) AS nome_normalizado, id_instituicao, COUNT(*) AS qtd,
       GROUP_CONCAT(id_medicamento ORDER BY id_medicamento) AS ids
FROM medicamentos
GROUP BY nome_normalizado, id_instituicao
HAVING COUNT(*) > 1;

-- 2. Condicoes clinicas duplicadas (por nome normalizado)
SELECT TRIM(LOWER(nome_condicao)) AS nome_normalizado, COUNT(*) AS qtd,
       GROUP_CONCAT(id_condicao ORDER BY id_condicao) AS ids
FROM condicoesclinicas
GROUP BY nome_normalizado
HAVING COUNT(*) > 1;

-- 3. Vinculos acolhido<->condicao duplicados
SELECT id_acolhido, id_condicao, COUNT(*) AS qtd,
       GROUP_CONCAT(id_acolhido_condicao ORDER BY id_acolhido_condicao) AS ids
FROM acolhido_condicaoclinica
GROUP BY id_acolhido, id_condicao
HAVING COUNT(*) > 1;

-- 4. Medicamentos programados duplicados
SELECT id_medicamento, id_acolhido_condicao, horario_administracao, COUNT(*) AS qtd,
       GROUP_CONCAT(id_medicamento_acond ORDER BY id_medicamento_acond) AS ids
FROM medicamento_acolhido_condicaoclinica
GROUP BY id_medicamento, id_acolhido_condicao, horario_administracao
HAVING COUNT(*) > 1;

-- 5. Agendamentos/administracoes duplicados
SELECT id_medicamento_acond, id_acolhido, data_hora_prevista, COUNT(*) AS qtd,
       GROUP_CONCAT(id_administracao ORDER BY id_administracao) AS ids
FROM administracoes_medicamento
GROUP BY id_medicamento_acond, id_acolhido, data_hora_prevista
HAVING COUNT(*) > 1;


-- ============================================================================
-- EXECUCAO — altera dados. Rode apos conferir o PREVIEW e com backup feito.
-- ============================================================================

SET @old_safe_updates = @@SQL_SAFE_UPDATES;
SET SQL_SAFE_UPDATES = 0;

START TRANSACTION;

-- ----------------------------------------------------------------------------
-- 1. MEDICAMENTOS (medicamentos) — mantem menor id por (instituicao + nome)
-- ----------------------------------------------------------------------------
-- 1a. Aponta os medicamentos programados para o medicamento mantido.
UPDATE medicamento_acolhido_condicaoclinica mac
JOIN medicamentos dup
  ON mac.id_medicamento = dup.id_medicamento
JOIN (
    SELECT MIN(id_medicamento) AS keep_id,
           TRIM(LOWER(nome))   AS norm,
           id_instituicao
    FROM medicamentos
    GROUP BY norm, id_instituicao
) k
  ON k.norm = TRIM(LOWER(dup.nome))
 AND (k.id_instituicao <=> dup.id_instituicao)
SET mac.id_medicamento = k.keep_id
WHERE mac.id_medicamento <> k.keep_id;

-- 1b. Apaga os medicamentos duplicados.
DELETE dup FROM medicamentos dup
JOIN (
    SELECT MIN(id_medicamento) AS keep_id,
           TRIM(LOWER(nome))   AS norm,
           id_instituicao
    FROM medicamentos
    GROUP BY TRIM(LOWER(nome)), id_instituicao
) k
  ON k.norm = TRIM(LOWER(dup.nome))
 AND (k.id_instituicao <=> dup.id_instituicao)
WHERE dup.id_medicamento <> k.keep_id;

-- ----------------------------------------------------------------------------
-- 2. CONDICOES CLINICAS (condicoesclinicas) — mantem menor id por nome
-- ----------------------------------------------------------------------------
-- 2a. Aponta os vinculos acolhido<->condicao para a condicao mantida.
UPDATE acolhido_condicaoclinica acc
JOIN condicoesclinicas dup
  ON acc.id_condicao = dup.id_condicao
JOIN (
    SELECT MIN(id_condicao) AS keep_id,
           TRIM(LOWER(nome_condicao)) AS norm
    FROM condicoesclinicas
    GROUP BY norm
) k
  ON k.norm = TRIM(LOWER(dup.nome_condicao))
SET acc.id_condicao = k.keep_id
WHERE acc.id_condicao <> k.keep_id;

-- 2b. Apaga as condicoes duplicadas.
DELETE dup FROM condicoesclinicas dup
JOIN (
    SELECT MIN(id_condicao) AS keep_id,
           TRIM(LOWER(nome_condicao)) AS norm
    FROM condicoesclinicas
    GROUP BY TRIM(LOWER(nome_condicao))
) k
  ON k.norm = TRIM(LOWER(dup.nome_condicao))
WHERE dup.id_condicao <> k.keep_id;

-- ----------------------------------------------------------------------------
-- 3. VINCULOS acolhido<->condicao (acolhido_condicaoclinica)
--    mantem menor id por (id_acolhido + id_condicao)
-- ----------------------------------------------------------------------------
-- 3a. Aponta os medicamentos programados para o vinculo mantido.
UPDATE medicamento_acolhido_condicaoclinica mac
JOIN acolhido_condicaoclinica dup
  ON mac.id_acolhido_condicao = dup.id_acolhido_condicao
JOIN (
    SELECT MIN(id_acolhido_condicao) AS keep_id, id_acolhido, id_condicao
    FROM acolhido_condicaoclinica
    GROUP BY id_acolhido, id_condicao
) k
  ON k.id_acolhido = dup.id_acolhido
 AND k.id_condicao = dup.id_condicao
SET mac.id_acolhido_condicao = k.keep_id
WHERE mac.id_acolhido_condicao <> k.keep_id;

-- 3b. Apaga os vinculos duplicados.
DELETE dup FROM acolhido_condicaoclinica dup
JOIN (
    SELECT MIN(id_acolhido_condicao) AS keep_id, id_acolhido, id_condicao
    FROM acolhido_condicaoclinica
    GROUP BY id_acolhido, id_condicao
) k
  ON k.id_acolhido = dup.id_acolhido
 AND k.id_condicao = dup.id_condicao
WHERE dup.id_acolhido_condicao <> k.keep_id;

-- ----------------------------------------------------------------------------
-- 4. MEDICAMENTOS PROGRAMADOS (medicamento_acolhido_condicaoclinica)
--    mantem menor id por (id_medicamento + id_acolhido_condicao + horario)
-- ----------------------------------------------------------------------------
-- 4a. Aponta as administracoes/agendamentos para o programado mantido.
UPDATE administracoes_medicamento adm
JOIN medicamento_acolhido_condicaoclinica dup
  ON adm.id_medicamento_acond = dup.id_medicamento_acond
JOIN (
    SELECT MIN(id_medicamento_acond) AS keep_id,
           id_medicamento, id_acolhido_condicao, horario_administracao
    FROM medicamento_acolhido_condicaoclinica
    GROUP BY id_medicamento, id_acolhido_condicao, horario_administracao
) k
  ON k.id_medicamento = dup.id_medicamento
 AND k.id_acolhido_condicao = dup.id_acolhido_condicao
 AND (k.horario_administracao <=> dup.horario_administracao)
SET adm.id_medicamento_acond = k.keep_id
WHERE adm.id_medicamento_acond <> k.keep_id;

-- 4b. Apaga os programados duplicados.
DELETE dup FROM medicamento_acolhido_condicaoclinica dup
JOIN (
    SELECT MIN(id_medicamento_acond) AS keep_id,
           id_medicamento, id_acolhido_condicao, horario_administracao
    FROM medicamento_acolhido_condicaoclinica
    GROUP BY id_medicamento, id_acolhido_condicao, horario_administracao
) k
  ON k.id_medicamento = dup.id_medicamento
 AND k.id_acolhido_condicao = dup.id_acolhido_condicao
 AND (k.horario_administracao <=> dup.horario_administracao)
WHERE dup.id_medicamento_acond <> k.keep_id;

-- ----------------------------------------------------------------------------
-- 5. AGENDAMENTOS / ADMINISTRACOES (administracoes_medicamento)
--    mantem menor id por (id_medicamento_acond + id_acolhido + data_hora_prevista)
-- ----------------------------------------------------------------------------
DELETE dup FROM administracoes_medicamento dup
JOIN (
    SELECT MIN(id_administracao) AS keep_id,
           id_medicamento_acond, id_acolhido, data_hora_prevista
    FROM administracoes_medicamento
    GROUP BY id_medicamento_acond, id_acolhido, data_hora_prevista
) k
  ON k.id_medicamento_acond = dup.id_medicamento_acond
 AND k.id_acolhido = dup.id_acolhido
 AND (k.data_hora_prevista <=> dup.data_hora_prevista)
WHERE dup.id_administracao <> k.keep_id;

-- ----------------------------------------------------------------------------
-- Confira os resultados acima. Se estiver tudo certo:
--     COMMIT;
-- Para desfazer tudo:
--     ROLLBACK;
-- ----------------------------------------------------------------------------
COMMIT;

SET SQL_SAFE_UPDATES = @old_safe_updates;
