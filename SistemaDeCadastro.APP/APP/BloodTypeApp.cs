using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;


namespace SistemaDeCadastro.APP.APP
{
    public class BloodTypeApp : IBloodTypeApp
    {
        private readonly IBloodTypeRepository _bloodTypeRepository;
        /// <summary>
        /// Recebe por injecao de dependencia o repositorio de tipos sanguineos.
        /// </summary>
        public BloodTypeApp(IBloodTypeRepository bloodTypeRepository) 
        {
          this._bloodTypeRepository = bloodTypeRepository;
        }
        /// <summary>
        /// Devolve uma lista com o tipo sanguineo do Id informado (vazia se nao existir).
        /// </summary>
        public async Task<List<BloodType>> GetBloodTypeById(long id) => 
            await this._bloodTypeRepository.GetBloodTypeById(id);

        /// <summary>
        /// Devolve em Data todos os tipos sanguineos cadastrados (apesar do nome "Illness", lista tipos sanguineos).
        /// </summary>
        public async Task<ApiResponse> GetAllBloodIllness ()
        {
            ApiResponse ret = new();
            try
            {
                ret.Data = await _bloodTypeRepository.FindAll();
                ret.Success = true;
            }


            catch (Exception err)
            {
                ret.ErrorMessage = ApiErrors.From(err);
                ret.Success = false;
            }

            return ret;
        }
        /// <summary>
        /// Cadastra um tipo sanguineo com o Id e o nome vindos do DTO.
        /// Observacao: mesmo quando grava, o retorno nao marca Success = true (fica false sem mensagem).
        /// </summary>
        public async Task<ApiResponse> CreateBloodType(BloodTypeDTO bloodType)
        {
            ApiResponse ret = new(); try
            {
                BloodType newBloodType = new();
                newBloodType.Id = bloodType.Id;
                newBloodType.Name = bloodType.Name;
                await this._bloodTypeRepository.CreateBooldType(newBloodType);
            }
            catch (Exception err)
            {
                ret.ErrorMessage = ApiErrors.From(err);
                ret.Success = false;
            }

            return ret;
        }
        /// <summary>
        /// Exclui o tipo sanguineo cujo Id vem no DTO (apesar do nome "Illness").
        /// Observacao: nao marca Success = true ao excluir e, se o Id nao existir, repassa null ao repositorio.
        /// </summary>
        public async Task<ApiResponse> DeleteIllness(BloodTypeDTO bloodType)
        {
            ApiResponse ret = new();
            try
            {
                BloodType deletebloodType = (await _bloodTypeRepository.GetBloodTypeById(bloodType.Id)).FirstOrDefault();
                await this._bloodTypeRepository.DeleteBloodtype(deletebloodType);
            }

            catch (Exception err)
            {
                ret.ErrorMessage = ApiErrors.From(err);
                ret.Success = false;
            }

            return ret;
        }


    }
}
