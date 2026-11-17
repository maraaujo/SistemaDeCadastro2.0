-- ============================================================================
-- Adiciona colunas de recuperacao de senha em contas_login — Sistema de Cadastro (MySQL)
--
-- POR QUE:
--   O fluxo "esqueci minha senha" gera um codigo (enviado por e-mail) e precisa
--   guardar, na conta, apenas o HASH desse codigo e a data de expiracao. Nunca o
--   codigo em texto. Ao usar ou expirar, as duas colunas voltam a NULL.
--
-- O QUE FAZ:
--   Cria as colunas token_reset_hash e token_reset_expira na tabela contas_login.
--   Ambas sao NULL por padrao (contas existentes ficam sem token, como esperado).
--
-- ANTES DE RODAR:
--   * FACA BACKUP do banco (ex.: mysqldump).
--   * O banco deste projeto e gerido por SQL manual (NAO rodar dotnet ef database
--     update). Rode este script uma unica vez.
-- ============================================================================

ALTER TABLE contas_login
    ADD COLUMN token_reset_hash   VARCHAR(255) NULL AFTER ultimo_login,
    ADD COLUMN token_reset_expira DATETIME     NULL AFTER token_reset_hash;

-- Conferencia (opcional):
-- SHOW COLUMNS FROM contas_login LIKE 'token_reset%';
