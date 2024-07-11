using AutoMapper;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Domain.Profiles
{

     public class UsuarioProfile : Profile
    {
        public UsuarioProfile() 
        {
            CreateMap<UsuarioDTO, Usuario>();
        }

    }
}
