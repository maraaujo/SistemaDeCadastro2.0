using Microsoft.Identity.Client;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.APP.APP
{
    public class MedicineApp : IMedicineApp
    {
        private readonly IMedicineRepository _medicineRepository;
        private readonly ICurrentUserServiceContext _currentUserService;
        /// <summary>
        /// Recebe por injecao de dependencia o repositorio de medicamentos e o usuario logado.
        /// </summary>
        public MedicineApp(IMedicineRepository medicineRepository, ICurrentUserServiceContext currentUserService)
        {
            this._medicineRepository = medicineRepository;  
            this._currentUserService = currentUserService;
        }

        /// <summary>
        /// Devolve todos os medicamentos visiveis para a instituicao do usuario.
        /// </summary>
        public async Task<List<Medicine>> GetAll() =>
            await this._medicineRepository.GetAll();
        /// <summary>
        /// Devolve uma lista com o medicamento do Id informado (vazia se nao existir).
        /// </summary>
        public async Task<List<Medicine>> GetMedicineId(long id) =>
            await this._medicineRepository.GetMedicineById(id);
        /// <summary>
        /// Chama a busca de medicamentos por texto no repositorio, mas descarta o resultado (o metodo nao devolve nada
        /// e nao faz parte da interface IMedicineApp).
        /// </summary>
        public async Task GetMedicineByAnyValorString(string medicine) =>
            await this._medicineRepository.GetMedicineByAnyValorString(medicine);

        /// <summary>
        /// Cadastra um medicamento (nome, descricao, dosagem, via de administracao) na instituicao do usuario logado.
        /// Recusa se nao for possivel identificar a instituicao do usuario.
        /// </summary>
        public async Task<ApiResponse> CreateMedicine(CreateMedicineDTO medicine)
        {
            ApiResponse ret = new();
            try
            {
                Medicine newMedicine = new();
              
                newMedicine.Description = medicine.Description ?? string.Empty;
                newMedicine.Dosage = medicine.Dosage;
                newMedicine.Name = medicine.Name;
                newMedicine.AdministrationRoute = medicine.AdministrationRoute;
                var institutionId = _currentUserService.InstitutionId;

                if (!institutionId.HasValue)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Não foi possível identificar a instituição do usuário logado.";
                    return ret;
                }

                newMedicine.InstitutionId = institutionId.Value;
                await this._medicineRepository.CreateMedicine(newMedicine);
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
        /// Atualiza nome, descricao, dosagem e via de administracao de um medicamento existente.
        /// Retorna "Medicamento nao encontrado." se o Id nao existir.
        /// </summary>
        public async Task<ApiResponse> UpdateMedicine(UpdateMedicineDTO medicine)
        {
            ApiResponse ret = new();

            try
            {
                Medicine updateMedicine = (await this._medicineRepository.GetMedicineById(medicine.Id)).FirstOrDefault();

                if (updateMedicine == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Medicamento não encontrado.";
                    return ret;
                }

                updateMedicine.Name = medicine.Name;
                updateMedicine.Description = medicine.Description ?? string.Empty;
                updateMedicine.Dosage = medicine.Dosage;
               
                updateMedicine.AdministrationRoute = medicine.AdministrationRoute;
                await this._medicineRepository.UpdateMedicine(updateMedicine);
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
        /// Exclui o medicamento cujo Id vem no DTO; retorna "Medicamento nao encontrado." se nao existir.
        /// </summary>
        public async Task<ApiResponse> DeleteMedicine(MedicineDTO medicine)
        {
            ApiResponse ret = new();
            try
            {
                Medicine idToDelete = (await this._medicineRepository.GetMedicineById(medicine.Id)).FirstOrDefault();

                if (idToDelete == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Medicamento não encontrado.";
                    return ret;
                }

                await this._medicineRepository.DeleteMedicine(idToDelete);
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
        /// Repassa o filtro ao repositorio e devolve a lista paginada de medicamentos.
        /// </summary>
        public async Task<PagedMedicineDTO> GetMedicineByFilter(MedicineFilterDTO filter)
        { return await this._medicineRepository.GetMedicineByFilter(filter); }
        
    }
}
