using Microsoft.AspNetCore.Identity;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;
namespace SistemaDeCadastro.APP.Interface
{
    public interface IResponsibleApp
    {
        Task<List<Responsible>> GetAll();
        Task<Responsible?> GetById(long id);
        Task<ApiResponse> Create(CreateResponsibleDTO entity);
        Task<ApiResponse> Update(UpdateResponsibleDTO entity);
        Task<ApiResponse> Delete(long id);
    }
}
