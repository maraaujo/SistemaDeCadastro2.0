using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IPatientIllnessRepository
    {
        Task CreatePatientIllness(PatientIllness patientIllness);
    }
}
