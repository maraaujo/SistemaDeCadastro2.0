# Relatório de Mudanças — Front e Back

> Período: **14/10/2026 a 03/11/2026**. Continua o `RELATORIO-NOVAS-FUNCIONALIDADES.md` (que vai até o commit `b9f7b80`).
> Repositórios: backend `SistemaDeCadastro2.0` (.NET 8) e frontend `frontSistemaDeGest-oAssistencial` (Vue 3, pasta `frontSistemaDeCadastro`).
> Em 03/11/2026 tudo foi juntado: backend `master` em `7cc30d7` (**65 de 65 testes aprovados**, build da API ok) e front `main` em `e7ad886` (build ok).

---

## Situação de cada mudança

| # | Mudança | Repo | Branch / commit | Situação |
|---|---|---|---|---|
| 1 | Horário certo nos próximos medicamentos | Back | `bc66312`, `37cc5e1` | Na `master` |
| 2 | Limpeza e reaproveitamento de duplicados (medicamento, condição, vínculo) | Back | `bc66312` + `scripts/limpeza_duplicados.sql` | Na `master` |
| 3 | Relatório de medicamentos do dia (endpoint) | Back | `bc66312` | Na `master` |
| 4 | Horário previsto e próxima medicação no histórico | Back | `bc66312` | Na `master` |
| 5 | Edição de medicamento usa o `id` da rota | Back | `b9f7b80` | Na `master` |
| 6 | Bloqueio de medicamento duplicado no mesmo horário | Back | `fix/medicamento-duplicado-mesmo-horario` (`9cb2dbb`) | Na `master` |
| 7 | Medicamento e dosagem no histórico | Back | `fix/medicamento-duplicado-mesmo-horario` (`907307f`) | Na `master` |
| 8 | Filtro por medicamento no histórico | Back | `fix/historico-medicacao-filtros-medicamento-periodo` (`2c15c6e`, `c467961`) | Na `master` |
| 9 | Container Docker do backend | Back | `chore/docker-backend` (`5347dae`) | Na `master` |
| 10 | Auditoria: registro automático das alterações | Back | `6c4e9da` | Na `master` |
| 11 | Isolamento entre instituições | Back | `3473e9d`, `404ccfe` | Na `master` |
| 12 | Tela do relatório do dia + PDF | Front | `b738bad`, `7c9fc04` | No `main` |
| 13 | Contagem ao vivo nos lembretes + coluna "Próxima medicação" | Front | `fix/contagem-lembretes-historico` (`ad3d197`) | No `main` |
| 14 | Remoção dos filtros de data do histórico | Front | `fix/historico-remove-filtro-data` (`6070769`) | No `main` |
| 15 | Condições clínicas: só o admin da plataforma edita | Front | `fix/condicoes-clinicas-somente-admin` | No `main` (`384c78f`) |

---

## 1. Próximos medicamentos — horário e contagem

**Problema:** um medicamento marcado para 14:41, consultado às 12:39, aparecia como "falta 1 minuto". A contagem também mostrava sempre 1 minuto a menos.

### Backend
- `Domain/Utils/DateTimeUtils.cs`: fuso `America/Sao_Paulo` explícito, com fallback para `E. South America Standard Time` (Windows) e, se nenhum existir, para -03:00 fixo. Funções `NowSaoPaulo()`, `GetSaoPauloOffset()` e `ToSaoPauloOffset()`.
- `GetMedicineReminders` calcula "agora" no horário de São Paulo, independente do fuso do servidor (Cloud Run roda em UTC).
- `MedicineReminderDTO.NextDoseDateTime` passou de `DateTime` para `DateTimeOffset` (`-03:00`), para o navegador não reinterpretar o horário.
- `MinutesRemaining` é arredondado para cima: faltando 1 min 30 s, mostra "Em 2 min", e não "Em 1 min".

### Frontend (`ad3d197`)
- Novo `src/utils/medicationTime.js`:
  - `minutesUntilDose(item, agora)` conta a partir de `nextDoseDateTime`, arredondando para cima.
  - No primeiro minuto de atraso mostra "Agora".
  - `formatRemaining` gera textos como "Em 2h 02min".
- `PatientReminders/index.vue`: relógio local que atualiza a cada 5 s. A lista continua sendo buscada na API a cada 60 s. Era esse intervalo de 60 s que causava o atraso de 1 minuto.
- `dashboard/index.vue`: usa a mesma função.

---

## 2. Duplicados (agendamentos, condições clínicas e medicamentos)

### Backend
- **Ao programar um medicamento** (`MedicinePatientClinicalConditionApp.Create`), reaproveita o que já existe em vez de criar de novo:
  - medicamento com o mesmo nome (sem diferenciar maiúsculas e minúsculas);
  - condição clínica com o mesmo nome;
  - vínculo acolhido + condição já existente.
- **Script** `scripts/limpeza_duplicados.sql` (MySQL), para limpar o que já está gravado:
  - tem uma seção *PREVIEW* que mostra o que será mexido;
  - depois, dentro de uma transação, junta os duplicados e apaga as sobras.
- **Mesmo medicamento no mesmo horário** (`9cb2dbb`):
  - vale ao registrar ministração e ao programar medicamento;
  - responde `Success=false` com a mensagem **"Já existe esse medicamento cadastrado nesse horário."**

### Frontend
- Nenhuma mudança necessária. As telas já mostram o `errorMessage` da API.

---

## 3. Histórico de medicação

### Backend
- `MedicationAdministrationDTO` ganhou:
  - `AdministrationTime` (horário previsto) e `NextDoseDateTime` (próxima medicação);
  - `MedicineName` e `PrescribedDosage` (`907307f`). Sem eles, as colunas Medicamento e Dosagem ficam vazias.
- Registros antigos com horário 00:00 passam a mostrar o horário programado do medicamento.
- Ao registrar uma ministração sem horário previsto, o backend calcula o horário: dia + horário programado.
- Filtro `MedicineId` em `MedicationAdministrationFilterDTO` (`2c15c6e`, `c467961`).

### Frontend
- Coluna **"Próxima medicação"** e função `formatNextDose` em `history.vue` (`ad3d197`).
- Campos "Data inicial" e "Data final" removidos. Os filtros que ficaram são Paciente, Medicamento e Status (`6070769`).

---

## 4. Relatório de medicamentos do dia

Já descrito na seção 1 do `RELATORIO-NOVAS-FUNCIONALIDADES.md`:
- Endpoint `GET api/MedicationAdministration/GetDailyMedicationReport?date=&patientId=`, com status Administrado / Atrasado / Pendente.
- Telas `/daily-medication-report` e `/daily-medication-report/print` (PDF).

---

## 5. Edição de medicamento

- `PUT api/Medicine/Update/{id}`: se o corpo vier sem `Id`, usa o `id` da rota.
- **Atenção:** o front já mandava o `id`, então essa provavelmente não era a causa. O problema de "não salva" ainda precisa ser confirmado com o status e a resposta da requisição `Update`, vistos no F12 → Network do navegador.

---

## 6. Auditoria — registro automático das alterações (`6c4e9da`)

### Backend
- **Como funciona:**
  - `SistemaCadastroContext.Audit.cs` (novo) sobrescreve `SaveChanges` e `SaveChangesAsync`.
  - Toda criação, alteração ou exclusão feita por um usuário logado gera um registro em `log_acesso`.
- **O que cada registro guarda:**

| Campo | Conteúdo |
|---|---|
| `id_usuario` | usuário do token (claim `UserId`) |
| Ação | `Criou` / `Alterou` / `Excluiu` + nome da entidade em português + `#id` (ex.: "Alterou Medicamento #12") |
| Observação | campos alterados com valor antigo → novo (ex.: "Dosage: 500mg → 1g") |
| Data/hora | horário de São Paulo |
| IP | primeiro valor de `X-Forwarded-For` (Cloud Run) ou IP da conexão |

- **Senhas, hashes e tokens nunca são gravados.** Nesses campos aparece só "(alterado)".
- **Quando não grava:** sem usuário logado (ex.: tarefas internas), nada é registrado.
- **Se o log falhar:** a alteração do usuário é mantida e o erro vai para o console.
- **Logs isolados por instituição:** uma instituição só vê os logs dos próprios usuários.
- **Endpoints (`api/AccessLog`):**
  - `GetAccessLogsByFilter` passou de `GET` para `POST`. O front já chamava com POST e recebia 405.
  - Novo filtro por usuário; o filtro de ação não diferencia maiúsculas.
  - `AccessLogListDTO` ganhou `UserName` e `Observation`.
  - Criar, editar e apagar log manualmente ficou restrito ao admin da plataforma.
- **Banco:** nenhuma alteração. Usa a tabela `log_acesso` que já existia.
- **Testes:** `AuditLogTests.cs` (criar/alterar/excluir, senha nunca registrada, sem usuário não registra).

### Frontend
- Ainda **não há tela de logs**. Existe só `src/api/access-logs-api.js`.

---

## 7. Isolamento entre instituições (`3473e9d`, `404ccfe`)

### Backend
- **Filtro de instituição deixou de ser "fail-open":**
  - só o **admin da plataforma** (papel `Administrador` e sem `institutionId` no token) vê todas as instituições;
  - qualquer outro token sem instituição não vê nem cria nada.
- **Contas de acesso:**
  - O admin da plataforma vê e gerencia todas.
  - O administrador de instituição vê e gerencia só as da própria instituição. Ele só cria conta na própria instituição.
  - O usuário comum só vê e edita a própria conta. Papel e status não mudam: o backend ignora essa parte.
  - Só administradores criam contas. O campo `Nome` agora é gravado na criação.
- **Filtros por instituição adicionados** em `Responsible`, `PatientClinicalCondition`, `MedicinePatientClinicalCondition`, `PatientEmployee` e `Payment`.
- **IDs vindos do front são validados** contra a instituição do usuário: acolhido, funcionário, departamento, medicamento e medicamento programado.
- **Só o admin da plataforma** (policy `SaasAdmin`) pode:
  - criar, editar e excluir Instituição, Plano, Assinatura, Permissão disponível e Condição clínica;
  - acessar tudo de Pagamento de assinatura e Permissão de usuário.
- **Leitura liberada só da própria instituição:** `GetInstitutionById/{id}` e `GetActiveSubscriptionByInstitution/{id}`. Qualquer outra instituição retorna 403.
- **Condições clínicas** continuam sendo um catálogo único. Todos consultam; só o admin da plataforma altera.
- **Testes:** `InstitutionIsolationTests.cs` comprova que a instituição A não lê, edita nem apaga dados da instituição B.

### Frontend
- **Condições clínicas** (`384c78f`):
  - para quem não é admin da plataforma, `ClinicalConditions/index.vue` esconde "Nova condição clínica" e a coluna "Ações";
  - a tela vira só de consulta. Antes, o usuário clicava e recebia erro 403.
  - Cadastrar condição nova pela tela de medicamentos do acolhido continua funcionando.
- **Sem mudança necessária:**
  - contas de acesso e perfil: o front já mandava a instituição certa;
  - instituição e assinatura: já buscavam só a da própria instituição;
  - planos, permissões e pagamentos de assinatura: não têm tela.

---

## 8. Container Docker do backend (`5347dae`)

| Arquivo | Função |
|---|---|
| `Dockerfile` | build multi-stage, fuso `America/Sao_Paulo`, roda sem root, porta 8080 |
| `.dockerignore` | não copia `bin/obj`, testes, `.env` nem arquivos de IDE |
| `docker-compose.yml` | serviço `api` na porta 8080, lendo o `.env` |
| `.env.example` | `ConnectionStrings__DefaultConnection` (Aiven, com SSL), `DB_PASSWORD`, `ASPNETCORE_ENVIRONMENT`, `Jwt__Key` |
| `.gitignore` | passa a ignorar `.env` |

Para subir: copiar `.env.example` para `.env`, preencher e rodar `docker compose up --build`. Teste local respondeu 200.

---

## 9. Pendências e cuidados

| Prioridade | Item |
|---|---|
| Alta | Fazer o deploy do backend (`7cc30d7`) e do front (`e7ad886`) juntos. Com só um deles atualizado, o front pode mostrar botões que o backend recusa, ou o contrário. |
| Média | Confirmar a edição de medicamento pelo F12 → Network. |
| Média | Testar as telas principais com um usuário comum de instituição, para ver se o isolamento não escondeu dados que deveriam aparecer. |
| Média | Criar a tela "Logs de auditoria" no front (filtros por usuário, ação e data). |
| Baixa | `CurrentUserServiceApp.UserId` faz `return UserId;` (recursão infinita). O código não é usado hoje, mas vale apagar ou corrigir. |
| Baixa | O front considera admin da plataforma "qualquer usuário sem instituição"; o backend exige também o papel `Administrador`. Um Colaborador sem instituição veria o menu Administração, mas o backend bloqueia. |

**Resolvidos desde o relatório anterior:** filtro de instituição "fail-open"; criação de conta que não gravava `Nome`; criar e listar contas sem exigir administrador.

**Corrigidos em 03/11/2026 (após a junção):**
- **Criar conta de acesso:**
  - Antes, a conta era gravada com `id_usuario = 0` e o sistema tentava copiar o id depois. O EF recusa essa cópia, porque `UserId` é chave alternativa, então a tela mostrava erro mesmo com a conta criada.
  - Agora `Id` e `UserId` são definidos com o mesmo valor antes de gravar (`LoginAccountRepository.GetNextAccountId`).
  - Nome vazio responde "Nome deve ser preenchido." e não chega ao banco.
  - Contas que já estão no banco com `id_usuario = 0`: `scripts/corrige_id_usuario_contas.sql`.
- **Excluir administração de medicamento:**
  - O `MedicationAdministrationRepository.Delete` chamava a si mesmo. O StackOverflow derrubava o processo inteiro da API.
  - Agora usa o `Delete` do `BaseRepository`.
  - O endpoint recebe o `id` da rota e aceita `GET` e `DELETE`.
  - Responde `Success = true` quando exclui e "Administração de medicamento não encontrada." quando o registro não existe ou é de outra instituição.
- **Testes:** `LoginAccountCreateAndDeleteTests.cs` (4 novos). O teste de isolamento de contas agora também exige `Success`. Total: 69 de 69.
- `CurrentUserServiceApp.UserId` também tem recursão, mas a classe não está registrada nem é usada: não roda.

**Junção em 03/11/2026:** no backend, a única divergência foi em `MedicationAdministrationRepository.cs`. Ficaram as duas partes: o join com o medicamento (nome no histórico) e o filtro por `MedicineId`. As travas de duplicado ficaram depois das validações de instituição. No front, tudo entrou sem conflito.
