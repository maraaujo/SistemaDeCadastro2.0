using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class BloodTypeRepository : BaseRepository<BloodType>, IBloodTypeRepository
    {
        /// <summary>Recebe o contexto do banco por injecao de dependencia e repassa ao BaseRepository.</summary>
        public BloodTypeRepository(SistemaDeCadastroContext context) 
            : base(context)
        {
        }

        /// <summary>Busca o tipo sanguineo pelo id usando FindBy; devolve uma lista com 0 ou 1 item.</summary>
        public async Task<List<BloodType>> GetBloodTypeById(long id)
        {
           return await this.FindBy(c => c.Id == id);
            
        }
        /// <summary>Lista todos os tipos sanguineos (tabela comum a todas as instituicoes).</summary>
        public async Task<List<BloodType>> FindAll() 
        {
            return await this.GetAll();
        }

        /// <summary>Grava o tipo sanguineo e devolve a mesma entidade, ja com o id gerado pelo banco.</summary>
        public async Task<BloodType> CreateBooldType(BloodType booldtype)
        {
            await this.Create(booldtype); 
            return booldtype;
        }

        /// <summary>Exclui o tipo sanguineo recebido (o parametro "id" e na verdade a entidade).</summary>
        public async Task DeleteBloodtype(BloodType id) 
        {
            await this.Delete(id);
        }
    }
}
