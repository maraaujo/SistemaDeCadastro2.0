using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Data.Interface;
using SistemaDeCadastro.Data.Repository;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.APP
{
    public class AdminApp : IAdminApp
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IIdosoRepository _idosoRepository;
        public AdminApp(IIdosoRepository idosoRepository, IAdminRepository adminRepository)
        {
         
            _adminRepository = adminRepository;
        }
        public async Task<PagedIdosoDTO> GetIdoso(IdosoFilterDTO filter) => await this._adminRepository.GetIdoso(filter);

        public async Task Delete(Idoso idoso) => await this._idosoRepository.Delete(idoso);

        public async Task Update(Idoso idoso) => await this._idosoRepository.Update(idoso);
        public async Task<List<Idoso>> GetAllIdoso()
        {
            return await this._adminRepository.GetAllIdoso();
        }

        public async Task<List<ElderlyMedicineDTO>> GetElderlyMedicine(int id)
        {
            return await this._adminRepository.GetElderlyMedicine(id);
        }

    

        public async Task<List<Idoso>> GetIdosoById(int id) => await this._adminRepository.GetIdosoById(id);

        public async Task<ApiResponseDTO> Create(IdosoDTO idosoDTO)
        {
            ApiResponseDTO response = new();

            try
            {
                if (string.IsNullOrEmpty(idosoDTO.Nome))
                    throw new Exception("O nome é obrigatório");

                if (string.IsNullOrEmpty(idosoDTO.Sobrenome))
                    throw new Exception("O sobrenome é obrigatório");


                Idoso model = new Idoso();
                model.Nome = idosoDTO.Nome;
                model.Sobrenome = idosoDTO.Sobrenome;


                await _idosoRepository.Create(model);

                response.Success = true;
            }
            catch (Exception err)
            {
                response.Success = false;
                response.ErrorMessage = err.Message;
            }


            return response;

        }


        public async Task<ApiResponseDTO> ValidateFuncionario(string email, string senha)
        {
            ApiResponseDTO response = new();

            try
            {
                if (string.IsNullOrEmpty(email))
                    throw new Exception("O nome é obrigatório");

                if (string.IsNullOrEmpty(senha))
                    throw new Exception("O sobrenome é obrigatório");

                await this._adminRepository.ValidateFuncionario(email, senha);
                response.Success = true;
            }
            catch (Exception err)
            {
                response.Success = false;
                response.ErrorMessage = err.Message;
            }


            return response;

        }

    }

      
    

}
