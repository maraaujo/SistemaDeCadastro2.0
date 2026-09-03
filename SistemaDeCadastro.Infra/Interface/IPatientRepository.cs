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
        Task<List<Patient>> GetPatientById(long id);
        Task<Patient>? FindPatientByCPF(string cpf, long institutionId);
        Task CreatePatient(Patient patient);
        Task UpdatePatient(Patient patient);
        Task DeletePatient(Patient patient);
        Task GetPatientByAny(string patient);
        Task<PagedPatientDTO> FilterPatient(PatientFilterDTO filter);
        Task<DetailsPatientDTO?> DetailsPatient(long id);
        Task<Patient?> GetByIdWithRelations(long id);

        
    }
}
