using AutoMapper;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;

namespace SistemaDeCadastro.Data.Profiles
{

     public class UsuarioProfile : Profile
    {
        public UsuarioProfile() 
        {
            CreateMap<UsuarioDTO, Usuario>();
        }

    }
}
