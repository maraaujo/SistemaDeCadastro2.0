using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IPatientIllnessApp
    {
        Task<ApiResponse> CreatePatientIllness(PatientIllness patientIllness);
    }
}
