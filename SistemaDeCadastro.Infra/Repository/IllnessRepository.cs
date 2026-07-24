using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class IllnessRepository : BaseRepository<Illness>, IIllnessRepository
    {
        private readonly SistemaDeCadastroContext _context;
        public IllnessRepository(SistemaDeCadastroContext context) 
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Illness>> GetIllnessById(long id)
        {
            return await this.FindBy(c => c.Id == id);
        }

        public async Task<List<Illness>> GetAllIllness()
        {
            return await this.GetAll(); 
        }

        public async Task DeleteIlless(Illness illness)
        {
            await this.Delete(illness);
        }
        public async Task CreateIllness(Illness illness) 
        {
            await this.Create(illness);
        }
        public async Task UpdateIllness(Illness illness)
        {
            await this.Update(illness);
        }

        public async Task GetAnyIllnessIfTheValor(string illness)
        {
            await this.Any(c => c.Name == illness);
        }
        public async Task<PagedIllnessDTO> GetIllnessByFilter(IllnessFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;
            var query = from i in _context.Illnesses
                        select new IllnessListDTO
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Cid = i.Cid,
                           Description = i.Description
                        };

            if(filter.Id.HasValue)
            {
                query = query.Where(c => c.Id == filter.Id.Value);
            }
            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(c => c.Name.Contains(filter.Name));
            }
            if(!string.IsNullOrEmpty(filter.Cid))
            {
                query = query.Where(c => c.Cid.Contains(filter.Cid));
            }
            if(!string.IsNullOrEmpty(filter.Description))
            {
                query = query.Where(c => c.Description.Contains(filter.Description));
            }
            var ret = new PagedIllnessDTO();

            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.Illnesses = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;

        }
        

        }

    }
