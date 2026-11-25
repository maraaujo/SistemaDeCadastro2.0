using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IPatientRepository : IBaseRepository<Patient>
    {
        /// <summary>Busca o acolhido pelo id (lista com 0 ou 1 item, somente da instituicao do usuario logado).</summary>
        Task<List<Patient>> GetPatientById(long id);
        /// <summary>Busca um acolhido pelo CPF dentro da instituicao informada (usado para barrar CPF duplicado).</summary>
        Task<Patient>? FindPatientByCPF(string cpf, long institutionId);
        /// <summary>Cadastra um acolhido.</summary>
        Task CreatePatient(Patient patient);
        /// <summary>Atualiza um acolhido.</summary>
        Task UpdatePatient(Patient patient);
        /// <summary>Exclui um acolhido.</summary>
        Task DeletePatient(Patient patient);
        /// <summary>Verifica se existe acolhido com o nome exato, mas o resultado nao e devolvido (retorno Task sem valor).</summary>
        Task GetPatientByAny(string patient);
        /// <summary>Lista paginada de acolhidos, filtrando por id e por condicoes clinicas.</summary>
        Task<PagedPatientDTO> FilterPatient(PatientFilterDTO filter);
        /// <summary>Ficha completa do acolhido: dados, tipo sanguineo, responsaveis, condicoes, medicamentos e agendamentos.</summary>
        Task<DetailsPatientDTO?> DetailsPatient(long id);
        /// <summary>Busca a entidade do acolhido carregando responsaveis e condicoes clinicas com seus medicamentos programados.</summary>
        Task<Patient?> GetByIdWithRelations(long id);

        // Executa as gravacoes de "action" numa unica transacao: ou tudo e salvo, ou nada.
        Task ExecuteInTransaction(Func<Task> action);

        // Exclui o acolhido e todos os registros que dependem dele, de forma atomica.
        Task DeletePatientWithDependencies(Patient patient);

        // Remove responsaveis, condicoes clinicas e medicamentos programados tirados na edicao
        // do acolhido, junto com as administracoes desses medicamentos.
        Task RemovePatientItems(
            IEnumerable<Responsible> responsibles,
            IEnumerable<PatientClinicalCondition> conditions,
            IEnumerable<MedicinePatientClinicalCondition> medicines);
    }
}
