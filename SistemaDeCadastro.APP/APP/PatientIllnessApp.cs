using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.APP
{
    public class PatientIllnessApp : IPatientIllnessApp
    {
        private readonly IPatientIllnessRepository _patientIllnessRepository;
        public PatientIllnessApp(IPatientIllnessRepository patientIllnessRepository) 
        {
            this._patientIllnessRepository = patientIllnessRepository;
        }

        public async Task<ApiResponse> CreatePatientIllness(PatientIllness patientIllness)
        {
            ApiResponse ret = new();
            try
            {
                PatientIllness newPatientIllness = new();
                newPatientIllness.Id = patientIllness.Id;
                newPatientIllness.IdPatient = patientIllness.IdPatient;
                newPatientIllness.IdIlleness = patientIllness.IdIlleness;
                await this._patientIllnessRepository.CreatePatientIllness(newPatientIllness);
            }
            catch (Exception err)
            {
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }

            return ret;
        }
    }
}
