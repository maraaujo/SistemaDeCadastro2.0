using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IPatientRepository
    {
        Task<List<Patient>> GetPatientById(long id);
        Task CreatePatient(Patient patient);
        Task UpdatePatient(Patient patient);
        Task DeletePatient(Patient patient);
        Task GetPatientByAny(string patient);
        Task<List<PatientFilterDTO>> FilterPatient(PatientFilterDTO filter);
    }
}
